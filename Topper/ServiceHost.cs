using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Threading;
using Topper.Internals;
using Topper.Logging;
using Topshelf;
using Topshelf.LibLog;
using Timer = System.Timers.Timer;

namespace Topper
{
    /// <summary>
    /// Call the <see cref="Run"/> method to start the host
    /// </summary>
    public static class ServiceHost
    {
        static readonly ILog Log = LogProvider.GetLogger(typeof(ServiceHost));
        static readonly ConcurrentStack<IDisposable> Disposables = new ConcurrentStack<IDisposable>();

        /// <summary>
        /// Starts the service host with the given <see cref="ServiceConfiguration"/>
        /// </summary>
        public static void Run(ServiceConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            if (IsAzureWebJob)
            {
                RunAsAzureWebJob(configuration);
                return;
            }

            RunAsTopShelf(configuration);
        }

        static void RunAsAzureWebJob(ServiceConfiguration configuration)
        {
            try
            {
                var topperService = new TopperService(configuration);
                var keepRunning = true;

                topperService.StartupFailed += exception =>
                {
                    Log.ErrorException("Startup failed", exception);
                    Volatile.Write(ref keepRunning, false);
                };

                DetectShutdownInAzureWebJobs(() =>
                {
                    Volatile.Write(ref keepRunning, false);
                });

                try
                {
                    Log.Info("Starting topper service(s)");
                    topperService.Start();

                    Log.Info("Running...");

                    while (Volatile.Read(ref keepRunning))
                    {
                        Thread.Sleep(100);
                    }

                    Log.Info("Exiting...");
                }
                finally
                {
                    Log.Info("Stopping topper service(s)");
                    topperService.Stop();
                }
            }
            catch (Exception exception)
            {
                Log.ErrorException("Unhandled exception in", exception);
            }
        }

        static void RunAsTopShelf(ServiceConfiguration configuration)
        {
            var name = Assembly.GetEntryAssembly().GetName().Name;

            HostFactory.Run(factory =>
            {
                factory.SetServiceName(name);
                factory.UseLibLog();
                factory.OnException(exception => { Log.ErrorException("Unhandled exception", exception); });
                factory.Service<TopperService>(config =>
                {
                    config.WhenStarted((service, control) =>
                    {
                        service.StartupFailed += exception =>
                        {
                            Log.ErrorException("Startup failed", exception);
                            control.Stop();
                        };
                        service.Start();
                        return true;
                    });
                    config.WhenStopped(service => service.Stop());
                    config.ConstructUsing(() => new TopperService(configuration));
                });
            });
        }

        static bool IsAzureWebJob => !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("WEBJOBS_SHUTDOWN_FILE"));

        static void DetectShutdownInAzureWebJobs(Action stopAction)
        {
            var filePath = Environment.GetEnvironmentVariable("WEBJOBS_SHUTDOWN_FILE");

            if (string.IsNullOrWhiteSpace(filePath))
            {
                // not an Azure Web Job
                return;
            }

            Log.Info($"Will monitor for Azure Web Job shutdown file at {filePath}");

            var timer = Using(new Timer(200));
            timer.Elapsed += (o, ea) =>
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        Log.Info($"Detected Azure Web Job file {filePath} - shutting down");
                        stopAction();
                    }
                }
                catch { }
            };
            timer.Start();
        }

        static TDisposable Using<TDisposable>(TDisposable disposable) where TDisposable : IDisposable
        {
            Disposables.Push(disposable);
            return disposable;
        }
    }
}
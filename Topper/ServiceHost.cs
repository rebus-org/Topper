using System;
using Topper.Internals;
using Topper.Logging;
using Topshelf;
using Topshelf.LibLog;

namespace Topper
{
    /// <summary>
    /// Call the <see cref="Run"/> method to start the host
    /// </summary>
    public static class ServiceHost
    {
        static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        /// <summary>
        /// Starts the service host with the given <see cref="ServiceConfiguration"/>
        /// </summary>
        public static void Run(ServiceConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            HostFactory.Run(factory =>
            {
                factory.UseLibLog();

                factory.OnException(exception =>
                {
                    Log.ErrorException("Unhandled exception", exception);
                });

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
                    config.WhenStopped(service =>
                    {
                        service.Stop();
                    });
                    config.ConstructUsing(() => new TopperService(configuration));
                });
            });
        }
    }
}
using System;
using Serilog;
using Topper.Internals;
using Topshelf;

namespace Topper
{
    /// <summary>
    /// Call the <see cref="Run"/> method to start the host
    /// </summary>
    public static class ServiceHost
    {
        /// <summary>
        /// Starts the service host with the given <see cref="ServiceConfiguration"/>
        /// </summary>
        public static void Run(ServiceConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            HostFactory.Run(factory =>
            {
                factory.UseSerilog();

                factory.OnException(exception =>
                {
                    Log.Error(exception, "Unhandled exception");
                });

                factory.Service<TopperService>(config =>
                {
                    config.WhenStarted((service, control) =>
                    {
                        service.StartupFailed += exception =>
                        {
                            Log.Error(exception, "Startup failed");
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
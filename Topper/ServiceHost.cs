using System;
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

            HostFactory.Run(f =>
            {
                f.UseSerilog();
                f.Service<TopperService>(c =>
                {
                    c.WhenStarted(s => s.Start());
                    c.WhenStopped(s => s.Stop());
                    c.ConstructUsing(() => new TopperService(configuration));
                });
            });
        }
    }
}
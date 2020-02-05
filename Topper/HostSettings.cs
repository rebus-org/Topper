// ReSharper disable RedundantDefaultMemberInitializer
// ReSharper disable UnusedMember.Global

using System;
using Topshelf.HostConfigurators;

namespace Topper
{
    /// <summary>
    /// Represents additional settings
    /// </summary>
    public class HostSettings
    {
        internal bool ParallelStartup { get; set; } = false;
        internal bool ParallelShutdown { get; set; } = false;

        /// <summary>
        /// Optional Topshelf host configuration customizer
        /// </summary>
        Action<HostConfigurator> _hostConfigurator;

        /// <summary>
        /// Enables parallel execution of service initializers. By default, services are serially initialized in the order in which they're added.
        /// Calling this method makes all initialization functions run in parallel.
        /// </summary>
        public void EnableParallelStartup()
        {
            ParallelStartup = true;
        }

        /// <summary>
        /// Enables parallel execution of service disposal. By default, services are serially disposed in the opposite order of which they're added.
        /// Calling this method makes all dispose functions run in parallel.
        /// </summary>
        public void EnableParallelShutdown()
        {
            ParallelShutdown = true;
        }

        /// <summary>
        /// Enables customization of Topshelf's <see cref="HostConfigurator"/> by providing a <paramref name="hostConfigurator"/> callback, which will be invoked when the service is installed.
        /// </summary>
        public HostSettings Topshelf(Action<HostConfigurator> hostConfigurator)
        {
            if (_hostConfigurator != null)
            {
                throw new InvalidOperationException("A Topshelf host configuration customization function has already been added - please make only one call to Topshelf");
            }
            _hostConfigurator = hostConfigurator;
            return this;
        }

        internal Action<HostConfigurator> GetHostConfigurator() => _hostConfigurator;
    }
}
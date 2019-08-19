// ReSharper disable RedundantDefaultMemberInitializer
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
    }
}
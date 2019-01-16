using Topshelf.HostConfigurators;
using Topshelf.Logging;

namespace Topper.Internals
{
    static class LibLogConfiguratorExtensions
    {
        public static void UseLibLog(this HostConfigurator configurator)
        {
            HostLogger.UseLogger(new LibLogLoggerConfigurator());
        }
    }
}

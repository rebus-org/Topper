using Topshelf.Logging;

namespace Topper.Internals
{
    class LibLogLoggerConfigurator : HostLoggerConfigurator
    {
        public LogWriterFactory CreateLogWriterFactory()
        {
            return new LibLogLogWriterFactory();
        }
    }
}
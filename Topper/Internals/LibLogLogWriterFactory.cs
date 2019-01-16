using Topshelf.Logging;

namespace Topper.Internals
{
    class LibLogLogWriterFactory : LogWriterFactory
    {
        public LogWriter Get(string name)
        {
            return new LibLogLogWriter(name);
        }

        public void Shutdown()
        {
        }
    }
}
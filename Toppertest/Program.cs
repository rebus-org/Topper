using System;
using Serilog;
using Topper;

namespace Toppertest
{
    class Program
    {
        static void Main()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .MinimumLevel.Verbose()
                .CreateLogger();

            var configuration = new ServiceConfiguration()
                .Add("test1", () => new TestService1())
                .Add("test2", () => new TestService2());

            ServiceHost.Run(configuration);
        }
    }

    class TestService2 : IDisposable
    {
        public void Dispose()
        {
        }
    }

    class TestService1 : IDisposable
    {
        public void Dispose()
        {
        }
    }
}

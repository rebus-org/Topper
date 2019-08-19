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
                //.MinimumLevel.Warning()
                .CreateLogger();

            var configuration = new ServiceConfiguration()
                .Configure(c =>
                {
                    c.EnableParallelStartup();
                    c.EnableParallelShutdown();
                })
                .Add("test1", () => new TestService1())
                .Add("test2", () => new TestService2());

            ServiceHost.Run(configuration);
        }
    }
}

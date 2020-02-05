using Serilog;
using Topper;
using Topshelf;

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

                    c.Topshelf(config =>
                    {
                        config.SetDescription("Some description");
                        config.EnableServiceRecovery(r => r.RestartService(5));
                    });
                })
                .Add("test1", () => new TestService1())
                .Add("test2", () => new TestService2());

            ServiceHost.Run(configuration);
        }
    }
}

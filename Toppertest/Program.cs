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
                .Add("test1", () => new TestService1())
                .Add("test2", () => new TestService2())
                .ExposeTopShelfConfiguration(config =>
                {
                    config.SetDescription("Some description");
                    config.EnableServiceRecovery(r =>
                    {
                        r.RestartService(5);
                    });
                });

            ServiceHost.Run(configuration);
        }
    }
}

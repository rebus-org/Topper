using System;
using System.Threading.Tasks;
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
                .WriteTo.File(@"C:\logs\toppertest\log.txt", rollOnFileSizeLimit: true, fileSizeLimitBytes: 1024 * 1024)
                .MinimumLevel.Verbose()
                .CreateLogger();

            var configuration = new ServiceConfiguration()
                .Configure(c =>
                {
                    //c.EnableParallelStartup();
                    //c.EnableParallelShutdown();

                    c.Topshelf(config =>
                    {
                        config.SetDescription("Some description");
                        config.EnableServiceRecovery(r => r.RestartService(5));
                    });
                })
                .Add("crashtest", async () =>
                {
                    var service = new CrashingTestService();
                    await service.CrashAsync();
                    return service;
                })
                //.Add("test1", () => new TestService1())
                //.Add("test2", () => new TestService2())
                ;

            ServiceHost.Run(configuration);
        }
    }

    class CrashingTestService : IDisposable
    {
        public async Task CrashAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(2));

            throw new InvalidOperationException("OH NO!");
        }

        public void Dispose()
        {
        }
    }
}

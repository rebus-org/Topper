using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Serilog;

namespace Topper.Internals
{
    class TopperService
    {
        readonly ConcurrentStack<Service> _services = new ConcurrentStack<Service>();
        readonly ILogger _logger = Log.ForContext<TopperService>();
        readonly ServiceConfiguration _configuration;

        public TopperService(ServiceConfiguration configuration)
        {
            _configuration = configuration;
        }

        public event Action<Exception> StartupFailed;

        public void Start()
        {
            Task.Run(async () =>
            {
                try
                {
                    await StartServices();
                }
                catch (Exception exception)
                {
                    StartupFailed?.Invoke(exception);
                }
            });
        }

        public void Stop()
        {
            Service service;

            while (_services.TryPop(out service))
            {
                _logger.Debug("Stopping service {ServiceName}", service.Name);

                service.Dispose();
            }
        }

        async Task StartServices()
        {
            _logger.Information("Starting Topper service");

            var functions = _configuration.GetFunctions();

            foreach (var service in functions)
            {
                try
                {
                    _logger.Debug("Starting service {ServiceName}", service.Name);

                    await service.Initialize();

                    _services.Push(service);
                }
                catch (Exception exception)
                {
                    throw new ApplicationException($"Could not start service '{service.Name}'", exception);
                }
            }
        }
    }
}
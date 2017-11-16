using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Topper.Logging;

namespace Topper.Internals
{
    class TopperService
    {
        readonly ConcurrentStack<Service> _services = new ConcurrentStack<Service>();
        readonly ILog _logger = LogProvider.GetCurrentClassLogger();
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
            while (_services.TryPop(out var service))
            {
                _logger.Debug($"Stopping service {service.Name}");

                service.Dispose();
            }
        }

        async Task StartServices()
        {
            _logger.Info("Starting Topper service");

            var functions = _configuration.GetFunctions();

            foreach (var service in functions)
            {
                try
                {
                    _logger.Debug($"Starting service {service.Name}");

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
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Topper.Logging;

namespace Topper.Internals
{
    class TopperService
    {
        readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        readonly ConcurrentStack<Service> _services = new ConcurrentStack<Service>();
        readonly ILog _logger = LogProvider.GetCurrentClassLogger();
        readonly ServiceConfiguration _configuration;

        public TopperService(ServiceConfiguration configuration)
        {
            _configuration = configuration;

            StartupFailed += _ => _cancellationTokenSource.Cancel();
        }

        HostSettings Settings => _configuration.GetSettings();

        public event Action<Exception> StartupFailed;

        public void Start()
        {
            Task.Run(async () =>
            {
                try
                {
                    await StartServices(_cancellationTokenSource.Token);
                }
                catch (Exception exception)
                {
                    StartupFailed?.Invoke(exception);
                }
            });
        }

        async Task StartServices(CancellationToken cancellationToken)
        {
            var functions = _configuration.GetFunctions();

            if (Settings.ParallelStartup)
            {
                _logger.Info("Starting Topper service (parallel startup activated)");
                await Task.WhenAll(functions.Select(service => StartService(service, cancellationToken)));
                return;
            }

            foreach (var service in functions)
            {
                _logger.Info("Starting Topper service");
                await StartService(service, cancellationToken);
            }
        }

        public void Stop()
        {
            if (Settings.ParallelShutdown)
            {
                _logger.Info("Stopping Topper service (parallel shutdown activated)");
                Parallel.ForEach(_services, service =>
                {
                    _logger.Debug($"Stopping service {service.Name}");

                    service.Dispose();
                });
                return;
            }

            _logger.Info("Stopping Topper service");

            while (_services.TryPop(out var service))
            {
                _logger.Debug($"Stopping service {service.Name}");

                service.Dispose();
            }
        }

        async Task StartService(Service service, CancellationToken cancellationToken)
        {
            try
            {
                _services.Push(service);

                _logger.Debug($"Starting service {service.Name}");

                await service.Initialize(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _logger.Debug($"Service {service.Name} startup cancelled");
            }
            catch (Exception exception)
            {
                throw new ApplicationException($"Could not start service '{service.Name}'", exception);
            }
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using Topper.Logging;

namespace Topper.Internals
{
    class Service : IDisposable
    {
        public string Name { get; }

        readonly Func<CancellationToken, Task<IDisposable>> _function;
        readonly ILog _logger = LogProvider.GetCurrentClassLogger();

        IDisposable _disposable;

        volatile Task<IDisposable> _initializationTask;

        public Service(Func<CancellationToken, Task<IDisposable>> function, string name)
        {
            _function = function;
            Name = name;
        }

        public async Task Initialize(CancellationToken cancellationToken)
        {
            _logger.Debug($"Initializing service {Name}");

            _initializationTask = _function(cancellationToken);
            _disposable = await _initializationTask;
        }

        public void Dispose()
        {
            // if the initialization task is null, this service was never initialized... therefore, there's not dispoable to dispose either
            if (_initializationTask == null) return;

            if (!_initializationTask.Wait(TimeSpan.FromSeconds(10)))
            {
                _logger.Warn($"Service {Name} was disposed before initialization finished, and initialization did not finish within 10s timeout");
            }

            _disposable?.Dispose();
        }
    }
}
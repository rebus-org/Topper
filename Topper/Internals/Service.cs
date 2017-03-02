using System;
using System.Threading.Tasks;

namespace Topper.Internals
{
    class Service : IDisposable
    {
        public string Name { get; }

        readonly Func<Task<IDisposable>> _function;
        IDisposable _disposable;

        public Service(Func<Task<IDisposable>> function, string name)
        {
            _function = function;
            Name = name;
        }

        public async Task Initialize()
        {
            _disposable = await _function();
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}
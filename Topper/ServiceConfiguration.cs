using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Topper.Internals;
// ReSharper disable UnusedMember.Global
// ReSharper disable EmptyConstructor
#pragma warning disable 1998

namespace Topper
{
    /// <summary>
    /// Create an instance of this one and add services to it by calling <see cref="Add(string,System.Func{System.IDisposable})"/>,
    /// <see cref="Add(string,System.Func{System.Threading.Tasks.Task{System.IDisposable}})"/>, or
    /// <see cref="Add(string,System.Func{CancellationToken, System.Threading.Tasks.Task{System.IDisposable}})"/>.
    /// When you have added enough services to it, call <see cref="ServiceHost.Run"/> with the configuration
    /// </summary>
    public class ServiceConfiguration
    {
        readonly List<Service> _serviceFunctions = new List<Service>();
        readonly HostSettings _hostSettings = new HostSettings();

        /// <summary>
        /// Creates the service configuration object. Start out by calling this, then add services to it, then call
        /// <see cref="ServiceHost.Run"/> with it.
        /// </summary>
        public ServiceConfiguration()
        {
        }

        /// <summary>
        /// Invokes configuration callback that makes it possible to further customize things
        /// </summary>
        public ServiceConfiguration Configure(Action<HostSettings> customizeHostSettings)
        {
            if (customizeHostSettings == null) throw new ArgumentNullException(nameof(customizeHostSettings));
            customizeHostSettings(_hostSettings);
            return this;
        }

        /// <summary>
        /// Adds the given service function with the given name
        /// </summary>
        public ServiceConfiguration Add(string name, Func<IDisposable> serviceFunction)
        {
            _serviceFunctions.Add(new Service(async _ => serviceFunction(), name));
            return this;
        }

        /// <summary>
        /// Adds the given async service function with the given name
        /// </summary>
        public ServiceConfiguration Add(string name, Func<Task<IDisposable>> serviceFunction)
        {
            _serviceFunctions.Add(new Service(_ => serviceFunction(), name));
            return this;
        }

        /// <summary>
        /// Adds the given async service function with the given name, passing a cancellation token in
        /// </summary>
        public ServiceConfiguration Add(string name, Func<CancellationToken, Task<IDisposable>> serviceFunction)
        {
            _serviceFunctions.Add(new Service(serviceFunction, name));
            return this;
        }

        internal HostSettings GetSettings() => _hostSettings;

        internal IEnumerable<Service> GetFunctions() => _serviceFunctions;
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Topper.Internals;
using Topshelf.HostConfigurators;
#pragma warning disable 1998

namespace Topper
{
    /// <summary>
    /// Create an instance of this one and add services to it by calling <see cref="Add(string,System.Func{System.IDisposable})"/>
    /// or <see cref="Add(string,System.Func{System.Threading.Tasks.Task{System.IDisposable}})"/>. When you have added enough
    /// services to it, call <see cref="ServiceHost.Run"/>
    /// </summary>
    public class ServiceConfiguration
    {
        Action<HostConfigurator> _hostConfigurator;
        readonly List<Service> _serviceFunctions = new List<Service>();

        /// <summary>
        /// Adds the given service function with the given name
        /// </summary>
        public ServiceConfiguration Add(string name, Func<IDisposable> serviceFunction)
        {
            _serviceFunctions.Add(new Service(async () => serviceFunction(), name));
            return this;
        }

        /// <summary>
        /// Adds the given async service function with the given name
        /// </summary>
        public ServiceConfiguration Add(string name, Func<Task<IDisposable>> serviceFunction)
        {
            _serviceFunctions.Add(new Service(serviceFunction, name));
            return this;
        }

        public ServiceConfiguration ExposeTopShelfConfiguration(Action<HostConfigurator> hostConfigurator)
        {
            _hostConfigurator = hostConfigurator;
            return this;
        }

        internal Action<HostConfigurator> GetHostConfigurator() => _hostConfigurator;
        internal IEnumerable<Service> GetFunctions() => _serviceFunctions;
    }
}

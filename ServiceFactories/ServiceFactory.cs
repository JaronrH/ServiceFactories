using System;
using System.Collections.Generic;
using System.Linq;
using ServiceFactories.Interfaces;

namespace ServiceFactories
{
    internal class ServiceFactory<TService, TKey> : IServiceFactory<TService, TKey>
    {
        private readonly IEnumerable<IServiceAccessor<TService, TKey>> _registrations;

        public ServiceFactory(IEnumerable<IServiceAccessor<TService, TKey>> registrations)
        {
            _registrations = registrations;
        }

        /// <summary>
        /// Does this factory contain a service for the provided key?
        /// </summary>
        /// <param name="serviceKey">Service Key</param>
        /// <returns>If the provided key can resolve a service or not.</returns>
        public bool CanResolve(TKey serviceKey)
        {
            return _registrations.Any(s => s.CanResolve(serviceKey));
        }

        /// <summary>
        /// Get a Service Registration for the given key.
        /// </summary>
        /// <param name="serviceKey">Service Key to resolve.</param>
        /// <returns>Service Accessor</returns>
        public IServiceAccessor<TService, TKey> GetAccessor(TKey serviceKey)
        {
            var service = _registrations.FirstOrDefault(s => s.CanResolve(serviceKey));
            if (service == null) throw new Exception($"No service accessor defined for {serviceKey}.");
            return service;
        }
    }
}

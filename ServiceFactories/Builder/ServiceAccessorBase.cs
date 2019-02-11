using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiceFactories.Interfaces;

namespace ServiceFactories.Builder
{
    internal abstract class ServiceAccessorBase<TService, TKey> : IServiceAccessor<TService, TKey>
        where TService : class
    {
        /// <summary>
        /// Create a Service Accessor.
        /// </summary>
        /// <param name="serviceKeys">Keys to associate with accessor.</param>
        /// <param name="serviceProvider">Service Provider reference from DI.</param>
        /// <param name="creatorFunc">Function used to create Singleton service.</param>
        /// <param name="creatorFuncAsync">Function used to create Singleton service.</param>
        /// <param name="serviceKeyResolver">Service Key Resolver for CanResolve()</param>
        protected ServiceAccessorBase(IEnumerable<TKey> serviceKeys, IServiceProvider serviceProvider, Func<IServiceProvider, object[], TService> creatorFunc, Func<IServiceProvider, object[], Task<TService>> creatorFuncAsync, Func<TKey, IEnumerable<TKey>, bool> serviceKeyResolver)
        {
            ServiceKeys = serviceKeys ?? throw new ArgumentNullException(nameof(serviceKeys));
            ServiceKeyResolver = serviceKeyResolver ?? throw new ArgumentNullException(nameof(serviceKeyResolver));
            AsyncServiceResolver = creatorFuncAsync ?? throw new ArgumentNullException(nameof(creatorFuncAsync));
            ServiceResolver = creatorFunc ?? throw new ArgumentNullException(nameof(creatorFunc));
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// Async Service Resolver Function
        /// </summary>
        protected Func<IServiceProvider, object[], Task<TService>> AsyncServiceResolver { get; }

        /// <summary>
        /// Sync Service Resolver Function
        /// </summary>
        protected Func<IServiceProvider, object[], TService> ServiceResolver { get; }

        /// <summary>
        /// Service Provider
        /// </summary>
        protected IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Service Keys Enumerable
        /// </summary>
        protected IEnumerable<TKey> ServiceKeys { get; }

        /// <summary>
        /// Service Key Resolver Function
        /// </summary>
        protected Func<TKey, IEnumerable<TKey>, bool> ServiceKeyResolver { get; }

        /// <summary>
        /// Can this Service Accessor resolve a service key?
        /// </summary>
        /// <param name="serviceKey">Key to check.</param>
        /// <returns>If an Accessor can resolve this key or not.</returns>
        public bool CanResolve(TKey serviceKey)
        {
            return ServiceKeyResolver(serviceKey, ServiceKeys);
        }


        /// <summary>
        /// Resolve Service
        /// </summary>
        /// <param name="args">Arguments for Service Resolver.</param>
        /// <returns>Service or null</returns>
        public abstract TService Resolve(params object[] args);

        /// <summary>
        /// Resolve Service
        /// </summary>
        /// <param name="args">Arguments for Service Resolver.</param>
        /// <returns>Service or null</returns>
        public abstract Task<TService> ResolveAsync(params object[] args);
    }
}
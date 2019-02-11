using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceFactories.Builder
{
    internal class TransientServiceAccessor<TService, TKey> : ServiceAccessorBase<TService, TKey> where TService : class
    {
        /// <summary>
        /// Create a Transient Service Accessor.  Either Async or Sync can create the service but that same service will be used for the remainder of the lifecycle.
        /// </summary>
        /// <param name="serviceKeys">Keys to associate with accessor.</param>
        /// <param name="serviceProvider">Service Provider reference from DI.</param>
        /// <param name="creatorFunc">Function used to create Singleton service.</param>
        /// <param name="creatorFuncAsync">Function used to create Singleton service.</param>
        /// <param name="serviceKeyResolver">Service Key Resolver for CanResolve()</param>
        public TransientServiceAccessor(IEnumerable<TKey> serviceKeys, IServiceProvider serviceProvider, Func<IServiceProvider, object[], TService> creatorFunc, Func<IServiceProvider, object[], Task<TService>> creatorFuncAsync, Func<TKey, IEnumerable<TKey>, bool> serviceKeyResolver)
            : base(serviceKeys, serviceProvider, creatorFunc, creatorFuncAsync, serviceKeyResolver)
        {
        }

        /// <summary>
        /// Resolve Service
        /// </summary>
        /// <param name="args">Arguments for Service Resolver.</param>
        /// <returns>Service or null</returns>
        public override TService Resolve(params object[] args)
        {
            return ServiceResolver(ServiceProvider, args);
        }

        /// <summary>
        /// Resolve Service
        /// </summary>
        /// <param name="args">Arguments for Service Resolver.</param>
        /// <returns>Service or null</returns>
        public override async Task<TService> ResolveAsync(params object[] args)
        {
            return await AsyncServiceResolver(ServiceProvider, args);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceFactories.Builder
{
    internal class SingletonServiceAccessor<TService, TKey> : ServiceAccessorBase<TService, TKey> where TService : class
    {
        /// <summary>
        /// Create a Singleton Service Accessor.  Either Async or Sync can create the service but that same service will be used for the remainder of the lifecycle.
        /// </summary>
        /// <param name="serviceKeys">Key to associate with accessor.</param>
        /// <param name="serviceProvider">Service Provider reference from DI.</param>
        /// <param name="creatorFunc">Function used to create Singleton service.</param>
        /// <param name="creatorFuncAsync">Function used to create Singleton service.</param>
        /// <param name="serviceKeyResolver">Service Key Resolver for CanResolve()</param>
        public SingletonServiceAccessor(IEnumerable<TKey> serviceKeys, IServiceProvider serviceProvider, Func<IServiceProvider, object[], TService> creatorFunc, Func<IServiceProvider, object[], Task<TService>> creatorFuncAsync, Func<TKey, IEnumerable<TKey>, bool> serviceKeyResolver)
            : base(serviceKeys, serviceProvider, creatorFunc, creatorFuncAsync, serviceKeyResolver)
        {
            ServiceResolverCache = null;
            Semaphore = new SemaphoreSlim(1, 1);;
        }

        /// <summary>
        /// Service Cache for when the service has been created.
        /// </summary>
        protected TService ServiceResolverCache { get; set; }

        /// <summary>
        /// Semaphore to make sure the service is only ever created once.
        /// </summary>
        protected SemaphoreSlim Semaphore { get; }

        /// <summary>
        /// Resolve Service
        /// </summary>
        /// <param name="args">Arguments for Service Resolver.</param>
        /// <returns>Service or null</returns>
        public override TService Resolve(params object[] args)
        {
            if (ServiceResolverCache != null) return ServiceResolverCache;
            Semaphore.Wait();
            try
            {
                if (ServiceResolverCache != null) return ServiceResolverCache;
                ServiceResolverCache = ServiceResolver(ServiceProvider, args);
                return ServiceResolverCache;
            }
            finally
            {
                Semaphore.Release();
            }
        }

        /// <summary>
        /// Resolve Service
        /// </summary>
        /// <param name="args">Arguments for Service Resolver.</param>
        /// <returns>Service or null</returns>
        public override async Task<TService> ResolveAsync(params object[] args)
        {
            if (ServiceResolverCache != null) return ServiceResolverCache;
            await Semaphore.WaitAsync();
            try
            {
                if (ServiceResolverCache != null) return ServiceResolverCache;
                ServiceResolverCache = await AsyncServiceResolver(ServiceProvider, args);
                return ServiceResolverCache;
            }
            finally
            {
                Semaphore.Release();
            }
        }
    }
}
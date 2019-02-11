using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceFactories.Interfaces
{
    /// <summary>
    /// Fluently build an Accessor.
    /// </summary>
    /// <typeparam name="TService">Service Implementation.</typeparam>
    /// <typeparam name="TKey">Key to use for services.</typeparam>
    public interface IFluentAccessorBuilder<TService, TKey>
    {
        /// <summary>
        /// Register the IServiceAccessor interface with specific <see cref="ServiceLifetime"/>.
        /// </summary>
        /// <returns>Accessor Builder</returns>
        IFluentAccessorBuilder<TService, TKey> ServiceLifetime(ServiceLifetime lifetime);

        /// <summary>
        /// Function used to build the service asynchronously.
        ///
        /// Params:
        ///   - ServiceProvider
        ///   - Arguments passed in to ResolveAsync.
        /// </summary>
        /// <returns>Accessor Builder</returns>
        IFluentAccessorBuilder<TService, TKey> AsyncResolver(Func<IServiceProvider, object[], Task<TService>> asyncResolverFunc);

        /// <summary>
        /// Function used to build the service synchronously.
        ///
        /// Params:
        ///   - ServiceProvider
        ///   - Arguments passed in to ResolveAsync.
        /// </summary>
        /// <returns>Accessor Builder</returns>
        IFluentAccessorBuilder<TService, TKey> SyncResolver(Func<IServiceProvider, object[], TService> resolverFunc);

        /// <summary>
        /// Accessor will use the Async or Sync Resolver to build the service once then re-use that service with every other Resolve/ResolveAsync request.
        /// </summary>
        /// <returns>Accessor Builder</returns>
        IFluentAccessorBuilder<TService, TKey> SingletonResolver();

        /// <summary>
        /// Accessor will use the Async or Sync Resolver each time Resolve/ResolveAsync is called.
        /// </summary>
        /// <returns>Accessor Builder</returns>
        IFluentAccessorBuilder<TService, TKey> TransientResolver();

        /// <summary>
        /// Key(s) associated with this accessor.
        /// </summary>
        /// <param name="serviceKeys">Key(s) used to match this accessor.</param>
        /// <returns>Accessor Builder</returns>
        IFluentAccessorBuilder<TService, TKey> WithKey(params TKey[] serviceKeys);

        /// <summary>
        /// Function used to validate a key.
        ///
        /// Params:
        ///   - Key to check.
        ///   - Keys associated with this Accessor.
        /// </summary>
        /// <param name="serviceKeyResolver">Function used to check and see if a key matches with Accessor.</param>
        /// <returns>Accessor Builder</returns>
        IFluentAccessorBuilder<TService, TKey> CanResolveKey(Func<TKey, IEnumerable<TKey>, bool> serviceKeyResolver);
    }
}
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceFactories.Interfaces
{
    /// <summary>
    /// Fluently build multiple Accessors.
    /// </summary>
    /// <typeparam name="TService">Service Implementation.</typeparam>
    /// <typeparam name="TKey">Key to use for services.</typeparam>
    public interface IFluentAccessorBuilders<TService, TKey>
    {
        /// <summary>
        /// Add an Accessor.
        /// </summary>
        /// <param name="builderAction">Builder Action</param>
        /// <returns>Fluent Accessor Builders</returns>
        IFluentAccessorBuilders<TService, TKey> AddAccessor(Action<IFluentAccessorBuilder<TService, TKey>> builderAction);

        /// <summary>
        /// Function used to validate a key for Accessors.
        ///
        /// Params:
        ///   - Key to check.
        ///   - Keys associated with this Accessor.
        /// </summary>
        /// <param name="serviceKeyResolver">Function used to check and see if a key matches an Accessor.</param>
        /// <returns>Accessor Builder</returns>
        IFluentAccessorBuilders<TService, TKey> CanResolveKey(Func<TKey, IEnumerable<TKey>, bool> serviceKeyResolver);

        /// <summary>
        /// Accessors will use the Async or Sync Resolver to build the service once then re-use that service with every other Resolve/ResolveAsync request.
        /// </summary>
        /// <returns>Accessor Builder</returns>
        IFluentAccessorBuilders<TService, TKey> SingletonResolvers();

        /// <summary>
        /// Accessors will use the Async or Sync Resolver each time Resolve/ResolveAsync is called.
        /// </summary>
        /// <returns>Accessor Builder</returns>
        IFluentAccessorBuilders<TService, TKey> TransientResolvers();

        /// <summary>
        /// Register the IServiceAccessor interfaces with specific <see cref="ServiceLifetime"/>.
        /// </summary>
        /// <returns>Accessor Builder</returns>
        IFluentAccessorBuilders<TService, TKey> ServiceLifetimes(ServiceLifetime lifetime);

        /// <summary>
        /// Register an <see cref="IServiceFactory{TService,TKey}"/> provider for these accessors.
        /// </summary>
        /// <returns>Accessor Builder</returns>
        IFluentAccessorBuilders<TService, TKey> AddFactory(ServiceLifetime lifetime);
    }
}
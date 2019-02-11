using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using ServiceFactories.Interfaces;

namespace ServiceFactories.Builder
{
    internal class FluentAccessorBuilders<TService, TKey> : IFluentAccessorBuilders<TService, TKey>, IAddFluentAccessor where TService : class
    {
        public FluentAccessorBuilders()
        {
            Builders = new List<IAddFluentAccessor>();
        }

        /// <summary>
        /// Builders to Add
        /// </summary>
        public IList<IAddFluentAccessor> Builders { get; }

        /// <summary>
        /// Service Lifetime to register Accessor as in DI
        /// </summary>
        public ServiceLifetime? Lifetime { get; set; }

        /// <summary>
        /// Resolver lifecycle for created service. 
        /// </summary>
        public ServiceLifetime? ResolverType { get; set; }

        /// <summary>
        /// Key Resolver
        /// </summary>
        public Func<TKey, IEnumerable<TKey>, bool> ServiceKeyResolver { get; set; }

        /// <summary>
        /// Should a factory be added?
        /// </summary>
        public ServiceLifetime? FactoryServiceLifetime{ get; set; }

        #region Implementation of IAddFluentAccessor<TService,TKey>

        /// <summary>
        /// Add Accessor to <see cref="IServiceCollection"/>
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <returns>Service Provider</returns>
        public IServiceCollection AddAccessor(IServiceCollection services)
        {
            if (FactoryServiceLifetime.HasValue)
                services.AddServiceFactory<TService, TKey>(FactoryServiceLifetime.Value);
            foreach (var builder in Builders)
                builder.AddAccessor(services);
            return services;
        }

        #endregion

        #region Implementation of IFluentAccessorBuilders<TService,TKey>

        /// <summary>
        /// Add an Accessor.
        /// </summary>
        /// <param name="builderAction">Builder Action</param>
        /// <returns>Fluent Accessor Builders</returns>
        public IFluentAccessorBuilders<TService, TKey> AddAccessor(Action<IFluentAccessorBuilder<TService, TKey>> builderAction)
        {
            var builder = new FluentAccessorBuilder<TService, TKey>(this);
            builderAction(builder);
            Builders.Add(builder);
            return this;
        }

        /// <summary>
        /// Function used to validate a key.
        ///
        /// Params:
        ///   - Key to check.
        ///   - Keys associated with this Accessor.
        /// </summary>
        /// <param name="serviceKeyResolver">Function used to check and see if a key matches with Accessor.</param>
        /// <returns>Accessor Builder</returns>
        public IFluentAccessorBuilders<TService, TKey> CanResolveKey(Func<TKey, IEnumerable<TKey>, bool> serviceKeyResolver)
        {
            ServiceKeyResolver = serviceKeyResolver ?? throw new ArgumentNullException(nameof(serviceKeyResolver));
            return this;
        }

        /// <summary>
        /// Accessors will use the Async or Sync Resolver to build the service once then re-use that service with every other Resolve/ResolveAsync request.
        /// </summary>
        /// <returns>Accessor Builder</returns>
        public IFluentAccessorBuilders<TService, TKey> SingletonResolvers()
        {
            ResolverType = ServiceLifetime.Singleton;
            return this;
        }

        /// <summary>
        /// Accessors will use the Async or Sync Resolver each time Resolve/ResolveAsync is called.
        /// </summary>
        /// <returns>Accessor Builder</returns>
        public IFluentAccessorBuilders<TService, TKey> TransientResolvers()
        {
            ResolverType = ServiceLifetime.Transient;
            return this;
        }

        /// <summary>
        /// Register the IServiceAccessor interface with specific <see cref="ServiceLifetime"/>.
        /// </summary>
        /// <returns>Accessor Builder</returns>
        public IFluentAccessorBuilders<TService, TKey> ServiceLifetimes(ServiceLifetime lifetime)
        {
            Lifetime = lifetime;
            return this;
        }

        /// <summary>
        /// Register an <see cref="IServiceFactory{TService,TKey}"/> provider for these accessors.
        /// </summary>
        /// <returns>Accessor Builder</returns>
        public IFluentAccessorBuilders<TService, TKey> AddFactory(ServiceLifetime lifetime)
        {
            FactoryServiceLifetime = lifetime;
            return this;
        }

        #endregion
    }
}
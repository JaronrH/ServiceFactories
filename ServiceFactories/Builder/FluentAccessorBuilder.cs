using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ServiceFactories.Interfaces;

namespace ServiceFactories.Builder
{
    internal class FluentAccessorBuilder<TService, TKey> : IFluentAccessorBuilder<TService, TKey>, IAddFluentAccessor where TService : class
    {
        public FluentAccessorBuilder(FluentAccessorBuilders<TService, TKey> parentAccessorBuilders): this()
        {
            ParentAccessorBuilders = parentAccessorBuilders;
        }

        public FluentAccessorBuilder()
        {
            AsyncServiceResolver = null;
            ServiceResolver = null;
            ServiceKeys = Enumerable.Empty<TKey>();
            Lifetime = null;
            ResolverType = null;
            ServiceKeyResolver = null;
            HasCustomResolver = false;
        }

        /// <summary>
        /// Is a custom resolver defined?
        /// </summary>
        private bool HasCustomResolver { get; set; }

        /// <summary>
        /// Parent Accessor Builder to get defaults from.
        /// </summary>
        private FluentAccessorBuilders<TService, TKey> ParentAccessorBuilders { get; }

        /// <summary>
        /// Async Resolver
        /// </summary>
        private Func<IServiceProvider, object[], Task<TService>> AsyncServiceResolver { get; set; }

        /// <summary>
        /// Sync Resolver
        /// </summary>
        private Func<IServiceProvider, object[], TService> ServiceResolver { get; set; }

        /// <summary>
        /// Keys for Accessor
        /// </summary>
        private IEnumerable<TKey> ServiceKeys { get; set; }

        /// <summary>
        /// Service Lifetime to register Accessor as in DI
        /// </summary>
        private ServiceLifetime? Lifetime { get; set; }

        /// <summary>
        /// Resolver lifecycle for created service. 
        /// </summary>
        private ServiceLifetime? ResolverType { get; set; }

        /// <summary>
        /// Key Resolver
        /// </summary>
        private Func<TKey, IEnumerable<TKey>, bool> ServiceKeyResolver { get; set; }


        #region Implementation of IAddFluentAccessor<TService,TKey>

        /// <summary>
        /// Add Accessor to <see cref="IServiceCollection"/>
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <returns>Service Provider</returns>
        public IServiceCollection AddAccessor(IServiceCollection services)
        {
            // Get global values for undefined variables
            if (ParentAccessorBuilders != null)
            {
                if (Lifetime == null) Lifetime = ParentAccessorBuilders.Lifetime;
                if (ResolverType == null) ResolverType = ParentAccessorBuilders.ResolverType;
                if (ServiceKeyResolver == null) ServiceKeyResolver = ParentAccessorBuilders.ServiceKeyResolver;
            }

            // Apply default Service Key Resolver?
            if (ServiceKeyResolver == null)
                ServiceKeyResolver = (key, keys) =>
                {
                    return key is IComparable
                        ? keys.Any(i => ((IComparable) key).CompareTo(i) == 0)
                        : keys.Contains(key);
                };

            // Validate Accessor Builder
            if (Lifetime == null) throw new Exception("Accessor Service Lifetime is not defined.");
            if (ResolverType == null) throw new Exception("Accessor's Resolver type is not defined.");
            if (!HasCustomResolver && !ServiceKeys.Any()) throw new Exception("No Accessor Key(s) defined.");
            if (ServiceResolver == null && AsyncServiceResolver == null)
                throw new Exception("Neither a synchronous or asynchronous Service Resolver is defined.");

            // Define Service Resolvers
            if (ServiceResolver == null)
                ServiceResolver = (p,a) => AsyncHelper.RunSync(() => AsyncServiceResolver(p,a));
            else if (AsyncServiceResolver == null)
                AsyncServiceResolver = (p, a) => Task.Factory.StartNew(() => ServiceResolver(p, a));

            // Create Factory
            Func<IServiceProvider, object> factory;
            if (ResolverType.Value == Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient)
                factory = p => new TransientServiceAccessor<TService, TKey>(ServiceKeys, p, ServiceResolver, AsyncServiceResolver, ServiceKeyResolver);
            else
                factory = p => new SingletonServiceAccessor<TService, TKey>(ServiceKeys, p, ServiceResolver, AsyncServiceResolver, ServiceKeyResolver);

            // Add Service to Collection
            services.Add(new ServiceDescriptor(typeof(IServiceAccessor<TService, TKey>), factory, Lifetime.Value));

            // Return Collection
            return services;
        }

        #endregion

        #region Implementation of IFluentAccessorBuilder<TService,TKey>

        /// <summary>
        /// Register the IServiceAccessor interface with specific <see cref="ServiceLifetime"/>.
        /// </summary>
        /// <returns>Accessor Builder</returns>
        public IFluentAccessorBuilder<TService, TKey> ServiceLifetime(ServiceLifetime lifetime)
        {
            Lifetime = lifetime;
            return this;
        }

        /// <summary>
        /// Function used to build the service asynchronously.
        ///
        /// Params:
        ///   - ServiceProvider
        ///   - Arguments passed in to ResolveAsync.
        /// </summary>
        /// <returns>Accessor Builder</returns>
        public IFluentAccessorBuilder<TService, TKey> AsyncResolver(Func<IServiceProvider, object[], Task<TService>> asyncResolverFunc)
        {
            AsyncServiceResolver = asyncResolverFunc ?? throw new ArgumentNullException(nameof(asyncResolverFunc));
            return this;
        }

        /// <summary>
        /// Function used to build the service synchronously.
        ///
        /// Params:
        ///   - ServiceProvider
        ///   - Arguments passed in to ResolveAsync.
        /// </summary>
        /// <returns>Accessor Builder</returns>
        public IFluentAccessorBuilder<TService, TKey> SyncResolver(Func<IServiceProvider, object[], TService> resolverFunc)
        {
            ServiceResolver = resolverFunc ?? throw new ArgumentNullException(nameof(resolverFunc));
            return this;
        }

        /// <summary>
        /// Accessor will use the Async or Sync Resolver to build the service once then re-use that service with every other Resolve/ResolveAsync request.
        /// </summary>
        /// <returns>Accessor Builder</returns>
        public IFluentAccessorBuilder<TService, TKey> SingletonResolver()
        {
            ResolverType = Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton;
            return this;
        }

        /// <summary>
        /// Accessor will use the Async or Sync Resolver each time Resolve/ResolveAsync is called.
        /// </summary>
        /// <returns>Accessor Builder</returns>
        public IFluentAccessorBuilder<TService, TKey> TransientResolver()
        {
            ResolverType = Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient;
            return this;
        }

        /// <summary>
        /// Key(s) associated with this accessor.
        /// </summary>
        /// <param name="serviceKeys">Key(s) used to match this accessor.</param>
        /// <returns>Accessor Builder</returns>
        public IFluentAccessorBuilder<TService, TKey> WithKey(params TKey[] serviceKeys)
        {
            ServiceKeys = serviceKeys;
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
        public IFluentAccessorBuilder<TService, TKey> CanResolveKey(Func<TKey, IEnumerable<TKey>, bool> serviceKeyResolver)
        {
            ServiceKeyResolver = serviceKeyResolver ?? throw new ArgumentNullException(nameof(serviceKeyResolver));
            HasCustomResolver = true;
            return this;
        }
        
        #endregion
    }
}
using System;
using ServiceFactories.Builder;
using ServiceFactories.Interfaces;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class AccessorDependencyInjectionExtensions
    {
        /// <summary>
        /// Register a Scoped Service Accessor.
        /// </summary>
        /// <typeparam name="TServiceAccessor">Service Accessor Implementation which includes IServiceAccessor.</typeparam>
        /// <typeparam name="TKey">Key used for service resolution.</typeparam>
        /// <typeparam name="TService">Service this accessor provides.</typeparam>
        /// <param name="services">Service Collection</param>
        /// <returns>Service Collection</returns>
        public static IServiceCollection AddScopedServiceAccessor<TService, TKey, TServiceAccessor>(this IServiceCollection services) where TServiceAccessor : class, IServiceAccessor<TService, TKey>
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            return services.AddServiceAccessor<TService, TKey, TServiceAccessor>(ServiceLifetime.Scoped);
        }

        /// <summary>
        /// Register a Singleton Service Accessor.
        /// </summary>
        /// <typeparam name="TServiceAccessor">Service Accessor Implementation which includes IServiceAccessor.</typeparam>
        /// <typeparam name="TKey">Key used for service resolution.</typeparam>
        /// <typeparam name="TService">Service this accessor provides.</typeparam>
        /// <param name="services">Service Collection</param>
        /// <returns>Service Collection</returns>
        public static IServiceCollection AddSingletonServiceAccessor<TService, TKey, TServiceAccessor>(this IServiceCollection services) where TServiceAccessor : class, IServiceAccessor<TService, TKey>
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            return services.AddServiceAccessor<TService, TKey, TServiceAccessor>(ServiceLifetime.Singleton);
        }

        /// <summary>
        /// Register a Transient Service Accessor.
        /// </summary>
        /// <typeparam name="TServiceAccessor">Service Accessor Implementation which includes IServiceAccessor.</typeparam>
        /// <typeparam name="TKey">Key used for service resolution.</typeparam>
        /// <typeparam name="TService">Service this accessor provides.</typeparam>
        /// <param name="services">Service Collection</param>
        /// <returns>Service Collection</returns>
        public static IServiceCollection AddTransientServiceAccessor<TService, TKey, TServiceAccessor>(this IServiceCollection services) where TServiceAccessor : class, IServiceAccessor<TService, TKey>
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            return services.AddServiceAccessor<TService, TKey, TServiceAccessor>(ServiceLifetime.Transient);
        }

        /// <summary>
        /// Register a Service Accessor.
        /// </summary>
        /// <typeparam name="TServiceAccessor">Service Accessor Implementation which includes IServiceAccessor.</typeparam>
        /// <typeparam name="TKey">Key used for service resolution.</typeparam>
        /// <typeparam name="TService">Service this accessor provides.</typeparam>
        /// <param name="services">Service Collection</param>
        /// <param name="lifetime">Service Lifetime</param>
        /// <returns>Service Collection</returns>
        public static IServiceCollection AddServiceAccessor<TService, TKey, TServiceAccessor>(this IServiceCollection services, ServiceLifetime lifetime) where TServiceAccessor : class, IServiceAccessor<TService, TKey>
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            services.Add(new ServiceDescriptor(typeof(IServiceAccessor<TService, TKey>), typeof(TServiceAccessor), lifetime));
            return services;
        }

        /// <summary>
        /// Register a Service Accessor.
        /// </summary>
        /// <typeparam name="TKey">Key used for service resolution.</typeparam>
        /// <typeparam name="TService">Service this accessor provides.</typeparam>
        /// <param name="services">Service Collection</param>
        /// <param name="lifetime">Service Lifetime</param>
        /// <param name="accessorFactory">Accessor Factory</param>
        /// <returns>Service Collection</returns>
        public static IServiceCollection AddServiceAccessor<TService, TKey>(this IServiceCollection services, ServiceLifetime lifetime, Func<IServiceProvider, IServiceAccessor<TService, TKey>> accessorFactory)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            services.Add(new ServiceDescriptor(typeof(IServiceAccessor<TService, TKey>), accessorFactory, lifetime));
            return services;
        }
        /// <summary>
        /// Register a Scoped Service Accessor.
        /// </summary>
        /// <typeparam name="TKey">Key used for service resolution.</typeparam>
        /// <typeparam name="TService">Service this accessor provides.</typeparam>
        /// <param name="services">Service Collection</param>
        /// <param name="accessorFactory">Accessor Factory</param>
        /// <returns>Service Collection</returns>
        public static IServiceCollection AddScopedServiceAccessor<TService, TKey>(this IServiceCollection services, Func<IServiceProvider, IServiceAccessor<TService, TKey>> accessorFactory)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            return services.AddServiceAccessor(ServiceLifetime.Scoped, accessorFactory);
        }

        /// <summary>
        /// Register a Singleton Service Accessor.
        /// </summary>
        /// <typeparam name="TKey">Key used for service resolution.</typeparam>
        /// <typeparam name="TService">Service this accessor provides.</typeparam>
        /// <param name="services">Service Collection</param>
        /// <param name="accessorFactory">Accessor Factory</param>
        /// <returns>Service Collection</returns>
        public static IServiceCollection AddSingletonServiceAccessor<TService, TKey>(this IServiceCollection services, Func<IServiceProvider, IServiceAccessor<TService, TKey>> accessorFactory)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            return services.AddServiceAccessor(ServiceLifetime.Singleton, accessorFactory);
        }

        /// <summary>
        /// Register a Transient Service Accessor.
        /// </summary>
        /// <typeparam name="TKey">Key used for service resolution.</typeparam>
        /// <typeparam name="TService">Service this accessor provides.</typeparam>
        /// <param name="services">Service Collection</param>
        /// <param name="accessorFactory">Accessor Factory</param>
        /// <returns>Service Collection</returns>
        public static IServiceCollection AddTransientServiceAccessor<TService, TKey>(this IServiceCollection services, Func<IServiceProvider, IServiceAccessor<TService, TKey>> accessorFactory)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            return services.AddServiceAccessor(ServiceLifetime.Transient, accessorFactory);
        }

        /// <summary>
        /// Register a Service Accessor.
        /// </summary>
        /// <typeparam name="TKey">Key used for service resolution.</typeparam>
        /// <typeparam name="TService">Service this accessor provides.</typeparam>
        /// <param name="services">Service Collection</param>
        /// <param name="lifetime">Service Lifetime</param>
        /// <param name="accessorFactory">Accessor Factory</param>
        /// <returns>Service Collection</returns>
        public static IServiceCollection AddServiceAccessor<TService, TKey>(this IServiceCollection services, ServiceLifetime lifetime, Func<IServiceAccessor<TService, TKey>> accessorFactory)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            services.Add(new ServiceDescriptor(typeof(IServiceAccessor<TService, TKey>), p => accessorFactory(), lifetime));
            return services;
        }
        /// <summary>
        /// Register a Scoped Service Accessor.
        /// </summary>
        /// <typeparam name="TKey">Key used for service resolution.</typeparam>
        /// <typeparam name="TService">Service this accessor provides.</typeparam>
        /// <param name="services">Service Collection</param>
        /// <param name="accessorFactory">Accessor Factory</param>
        /// <returns>Service Collection</returns>
        public static IServiceCollection AddScopedServiceAccessor<TService, TKey>(this IServiceCollection services, Func<IServiceAccessor<TService, TKey>> accessorFactory)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            return services.AddServiceAccessor(ServiceLifetime.Scoped, accessorFactory);
        }

        /// <summary>
        /// Register a Singleton Service Accessor.
        /// </summary>
        /// <typeparam name="TKey">Key used for service resolution.</typeparam>
        /// <typeparam name="TService">Service this accessor provides.</typeparam>
        /// <param name="services">Service Collection</param>
        /// <param name="accessorFactory">Accessor Factory</param>
        /// <returns>Service Collection</returns>
        public static IServiceCollection AddSingletonServiceAccessor<TService, TKey>(this IServiceCollection services, Func<IServiceAccessor<TService, TKey>> accessorFactory)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            return services.AddServiceAccessor(ServiceLifetime.Singleton, accessorFactory);
        }

        /// <summary>
        /// Register a Transient Service Accessor.
        /// </summary>
        /// <typeparam name="TKey">Key used for service resolution.</typeparam>
        /// <typeparam name="TService">Service this accessor provides.</typeparam>
        /// <param name="services">Service Collection</param>
        /// <param name="accessorFactory">Accessor Factory</param>
        /// <returns>Service Collection</returns>
        public static IServiceCollection AddTransientServiceAccessor<TService, TKey>(this IServiceCollection services, Func<IServiceAccessor<TService, TKey>> accessorFactory)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            return services.AddServiceAccessor(ServiceLifetime.Transient, accessorFactory);
        }

        /// <summary>
        /// Create an Accessor using a fluent builder.
        /// </summary>
        /// <typeparam name="TKey">Key used for service resolution.</typeparam>
        /// <typeparam name="TService">Service this accessor provides.</typeparam>
        /// <param name="services">Service Collection</param>
        /// <param name="builderAction">Fluent Accessor builder.</param>
        /// <returns>Service Collection</returns>
        public static IServiceCollection AddServiceAccessor<TService, TKey>(this IServiceCollection services, Action<IFluentAccessorBuilder<TService, TKey>> builderAction) where TService : class
        {
            if (builderAction == null) throw new ArgumentNullException(nameof(builderAction));
            var builder = new FluentAccessorBuilder<TService, TKey>();
            builderAction(builder);
            return builder.AddAccessor(services);
        }

        /// <summary>
        /// Create multiple Accessors using a fluent builder.
        /// </summary>
        /// <typeparam name="TKey">Key used for service resolution.</typeparam>
        /// <typeparam name="TService">Service this accessor provides.</typeparam>
        /// <param name="services">Service Collection</param>
        /// <param name="buildersAction">Fluent Accessors builder.</param>
        /// <returns>Service Collection</returns>
        public static IServiceCollection AddServiceAccessors<TService, TKey>(this IServiceCollection services, Action<IFluentAccessorBuilders<TService, TKey>> buildersAction) where TService : class
        {
            if (buildersAction == null) throw new ArgumentNullException(nameof(buildersAction));
            var builder = new FluentAccessorBuilders<TService, TKey>();
            buildersAction(builder);
            return builder.AddAccessor(services);
        }
    }
}
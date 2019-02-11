using System;
using System.Linq;
using ServiceFactories;
using ServiceFactories.Interfaces;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceFactoryDependencyInjectionExtensions
    {
        /// <summary>
        /// Add Singleton Service Factory for Sync and Async access via <see cref="TKey"/> and <see cref="TService"/>
        /// </summary>
        /// <param name="services">Service Collection</param>
        /// <returns>Service Collection</returns>
        public static IServiceCollection AddSingletonServiceFactory<TService, TKey>(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            return services.AddServiceFactory<TService, TKey>(ServiceLifetime.Singleton);
        }

        /// <summary>
        /// Add Transient Service Factory for Sync and Async access via <see cref="TKey"/> and <see cref="TService"/>
        /// </summary>
        /// <param name="services">Service Collection</param>
        /// <returns>Service Collection</returns>
        public static IServiceCollection AddTransientServiceFactory<TService, TKey>(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            return services.AddServiceFactory<TService, TKey>(ServiceLifetime.Transient);
        }

        /// <summary>
        /// Add Scoped Service Factory for Sync and Async access via <see cref="TKey"/> and <see cref="TService"/>
        /// </summary>
        /// <param name="services">Service Collection</param>
        /// <returns>Service Collection</returns>
        public static IServiceCollection AddScopedServiceFactory<TService, TKey>(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            return services.AddServiceFactory<TService, TKey>(ServiceLifetime.Scoped);
        }

        /// <summary>
        /// Add Singleton Service Factory for Sync and Async access via <see cref="TKey"/> and <see cref="TService"/>
        /// </summary>
        /// <param name="services">Service Collection</param>
        /// <param name="lifetime">Service Lifetime</param>
        /// <returns>Service Collection</returns>
        public static IServiceCollection AddServiceFactory<TService, TKey>(this IServiceCollection services, ServiceLifetime lifetime)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (services.ServiceFactoryAlreadyExists<TService, TKey>()) throw new Exception($"Factory for Key={typeof(TKey)} returning Service {typeof(TService)} already exists.");
            services.Add(new ServiceDescriptor(typeof(IServiceFactory<TService, TKey>), typeof(ServiceFactory<TService, TKey>), lifetime));
            return services;
        }

        /// <summary>
        /// Check to see if a Service Collection already contains a factory or not.
        /// </summary>
        /// <typeparam name="TKey">Key to use for services.</typeparam>
        /// <typeparam name="TService">Service Implementation.</typeparam>
        /// <param name="services">Service Collection</param>
        /// <returns>If Service Collection contains a service factory or not.</returns>
        private static bool ServiceFactoryAlreadyExists<TService, TKey>(this IServiceCollection services)
        {
            var syncType = typeof(IServiceFactory<TService, TKey>);
            var asyncType = typeof(IServiceFactory<TService, TKey>);
            return services.Any(d => d.ServiceType == syncType || d.ServiceType == asyncType);
        }
    }
}
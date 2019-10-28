using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace ServiceFactories.Interfaces
{
    public static class ServiceFactoryExtensions
    {
        /// <summary>
        /// Resolve Service Synchronously.
        /// </summary>
        /// <param name="factory">Service Factory</param>
        /// <param name="serviceKey">Service Key to resolve.</param>
        /// <param name="args">Arguments for Service Resolver.</param>
        /// <returns>Service or null</returns>
        public static TService Resolve<TService, TKey>(this IServiceFactory<TService, TKey> factory, TKey serviceKey, params object[] args)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return factory.GetAccessor(serviceKey).Resolve(args);
        }

        /// <summary>
        /// Resolve Service Asynchronously.
        /// </summary>
        /// <param name="factory">Service Factory</param>
        /// <param name="serviceKey">Service Key to resolve.</param>
        /// <param name="args">Arguments for Service Resolver.</param>
        /// <returns>Service or null</returns>
        public static async Task<TService> ResolveAsync<TService, TKey>(this IServiceFactory<TService, TKey> factory, TKey serviceKey, params object[] args)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return await factory.GetAccessor(serviceKey).ResolveAsync(args);
        }
        /// <summary>
        /// Resolve Services Synchronously.
        /// </summary>
        /// <param name="factory">Service Factory</param>
        /// <param name="serviceKey">Service Key to resolve.</param>
        /// <param name="args">Arguments for Service Resolver.</param>
        /// <returns>Service or null</returns>
        public static IEnumerable<TService> ResolveAll<TService, TKey>(this IServiceFactory<TService, TKey> factory, TKey serviceKey, params object[] args)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return factory.GetAccessors(serviceKey).Select(a => a.Resolve(args));
        }

        /// <summary>
        /// Resolve all Services Asynchronously.
        /// </summary>
        /// <param name="factory">Service Factory</param>
        /// <param name="serviceKey">Service Key to resolve.</param>
        /// <param name="args">Arguments for Service Resolver.</param>
        /// <returns>Service or null</returns>
        public static async Task<IEnumerable<TService>> ResolveAllAsync<TService, TKey>(this IServiceFactory<TService, TKey> factory, TKey serviceKey, params object[] args)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return await Task.WhenAll(factory.GetAccessors(serviceKey).Select(a => a.ResolveAsync(args)));
        }
    }
}

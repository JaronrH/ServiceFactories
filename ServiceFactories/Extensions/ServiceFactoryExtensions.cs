using System;
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
    }
}

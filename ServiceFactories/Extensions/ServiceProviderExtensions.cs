using System.Threading.Tasks;
using ServiceFactories.Interfaces;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Gets the service factory for TService with a key of TKey.
        /// </summary>
        /// <typeparam name="TService">Type of service object to get.</typeparam>
        /// <typeparam name="TKey">Key to use for services.</typeparam>
        /// <param name="provider">Service Provider</param>
        /// <returns>A Factory object of type TService and key TKey. -or- null if there is no service factory object of type TService.</returns>
        public static IServiceFactory<TService, TKey> GetFactory<TService, TKey>(this IServiceProvider provider)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));
            return (IServiceFactory<TService, TKey>)provider.GetService(typeof(IServiceFactory<TService, TKey>));
        }

        /// <summary>
        /// Gets the service factory for TService with a key of TKey.  Throws an exception if service cannot be resolved.
        /// </summary>
        /// <typeparam name="TService">Type of service object to get.</typeparam>
        /// <typeparam name="TKey">Key to use for services.</typeparam>
        /// <param name="provider">Service Provider</param>
        /// <returns>A Factory object of type TService and key TKey.</returns>
        public static IServiceFactory<TService, TKey> GetRequiredFactory<TService, TKey>(this IServiceProvider provider)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));
            var factory = provider.GetFactory<TService, TKey>();
            if (factory == null)
                throw new InvalidOperationException(
                    $"There is no service of type {typeof(TService)} defined that can be resolved by the provided service key.");
            return factory;
        }

        /// <summary>
        /// Gets the service object of the specified type Synchronously.
        /// </summary>
        /// <typeparam name="TService">Type of service object to get.</typeparam>
        /// <typeparam name="TKey">Key to use for services.</typeparam>
        /// <param name="provider">Service Provider</param>
        /// <param name="serviceKey">Service Key</param>
        /// <param name="args">Arguments for Service Resolver.</param>
        /// <returns>A service object of type serviceType. -or- null if there is no service factory object of type serviceType or serviceKey could not be resolved.</returns>
        public static TService GetService<TService, TKey>(this IServiceProvider provider, TKey serviceKey, params object[] args)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));
            var factory = provider.GetFactory<TService, TKey>();
            return factory == null || !factory.CanResolve(serviceKey)
                ? default 
                : factory.Resolve(serviceKey, args);
        }

        /// <summary>
        /// Gets the service object of the specified type Asynchronously.
        /// </summary>
        /// <typeparam name="TService">Type of service object to get.</typeparam>
        /// <typeparam name="TKey">Key to use for services.</typeparam>
        /// <param name="provider">Service Provider</param>
        /// <param name="serviceKey">Service Key</param>
        /// <param name="args">Arguments for Service Resolver.</param>
        /// <returns>A service object of type serviceType. -or- null if there is no service factory object of type serviceType or serviceKey could not be resolved.</returns>
        public static async Task<TService> GetServiceAsync<TService, TKey>(this IServiceProvider provider, TKey serviceKey, params object[] args)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));
            var factory = provider.GetFactory<TService, TKey>();
            return factory == null || !factory.CanResolve(serviceKey)
                ? default
                : await factory.ResolveAsync(serviceKey, args);
        }
        /// <summary>
        /// Gets the service object of the specified type Synchronously.  Throws an exception if service cannot be resolved.
        /// </summary>
        /// <typeparam name="TService">Type of service object to get.</typeparam>
        /// <typeparam name="TKey">Key to use for services.</typeparam>
        /// <param name="provider">Service Provider</param>
        /// <param name="serviceKey">Service Key</param>
        /// <param name="args">Arguments for Service Resolver.</param>
        /// <returns>A service object of type serviceType.</returns>
        public static TService GetRequiredService<TService, TKey>(this IServiceProvider provider, TKey serviceKey, params object[] args)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));
            var service = provider.GetService<TService, TKey>(serviceKey, args);
            if (service == null)
                throw new InvalidOperationException(
                    $"There is no service of type {typeof(TService)} defined that can be resolved by the provided service key.");
            return service;
        }

        /// <summary>
        /// Gets the service object of the specified type Asynchronously.  Throws an exception if service cannot be resolved.
        /// </summary>
        /// <typeparam name="TService">Type of service object to get.</typeparam>
        /// <typeparam name="TKey">Key to use for services.</typeparam>
        /// <param name="provider">Service Provider</param>
        /// <param name="serviceKey">Service Key</param>
        /// <param name="args">Arguments for Service Resolver.</param>
        /// <returns>A service object of type serviceType.</returns>
        public static async Task<TService> GetRequiredServiceAsync<TService, TKey>(this IServiceProvider provider, TKey serviceKey, params object[] args)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));
            var service = await provider.GetServiceAsync<TService, TKey>(serviceKey, args);
            if (service == null)
                throw new InvalidOperationException(
                    $"There is no service of type {typeof(TService)} defined that can be resolved by the provided service key.");
            return service;
        }
    }
}

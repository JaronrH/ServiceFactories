using System.Collections.Generic;

namespace ServiceFactories.Interfaces
{
    /// <summary>
    /// Service Factory for <see cref="IServiceAccessor{TKey, TService}"/>
    /// </summary>
    /// <typeparam name="TKey">Key to use for services.</typeparam>
    /// <typeparam name="TService">Service Implementation.</typeparam>
    public interface IServiceFactory<TService, in TKey>
    {
        /// <summary>
        /// Does this factory contain a service for the provided key?
        /// </summary>
        /// <param name="serviceKey">Service Key</param>
        /// <returns>If the provided key can resolve a service or not.</returns>
        bool CanResolve(TKey serviceKey);

        /// <summary>
        /// Get the first Service Registration found for the given key.
        /// </summary>
        /// <param name="serviceKey">Service Key to resolve.</param>
        /// <returns>Service Accessor</returns>
        IServiceAccessor<TService, TKey> GetAccessor(TKey serviceKey);

        /// <summary>
        /// Get all Service Registrations for a provided key
        /// </summary>
        /// <param name="serviceKey">Service Key to resolve.</param>
        /// <returns>Service Accessor</returns>
        IEnumerable<IServiceAccessor<TService, TKey>> GetAccessors(TKey serviceKey);
    }
}
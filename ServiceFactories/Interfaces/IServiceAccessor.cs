using System.Threading.Tasks;

namespace ServiceFactories.Interfaces
{
    /// <summary>
    /// Service Accessor for <see cref="TService"/> defined by a unique <see cref="TKey"/>
    /// </summary>
    /// <typeparam name="TKey">Key to use for services.</typeparam>
    /// <typeparam name="TService">Service Implementation.</typeparam>
    public interface IServiceAccessor<TService, in TKey>
    {
        /// <summary>
        /// Can this Service Accessor resolve a service key?
        /// </summary>
        /// <param name="serviceKey">Key to check.</param>
        /// <returns>If an Accessor can resolve this key or not.</returns>
        bool CanResolve(TKey serviceKey);

        /// <summary>
        /// Resolve Service Synchronously.
        /// </summary>
        /// <param name="args">Arguments for Service Resolver.</param>
        /// <returns>Service or null</returns>
        TService Resolve(params object[] args);

        /// <summary>
        /// Resolve Service Asynchronously.
        /// </summary>
        /// <param name="args">Arguments for Service Resolver.</param>
        /// <returns>Service or null</returns>
        Task<TService> ResolveAsync(params object[] args);
    }
}
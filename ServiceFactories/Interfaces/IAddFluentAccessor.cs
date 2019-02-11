using Microsoft.Extensions.DependencyInjection;

namespace ServiceFactories.Interfaces
{
    internal interface IAddFluentAccessor
    {
        /// <summary>
        /// Add Accessor to <see cref="IServiceCollection"/>
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <returns>Service Provider</returns>
        IServiceCollection AddAccessor(IServiceCollection services);
    }
}
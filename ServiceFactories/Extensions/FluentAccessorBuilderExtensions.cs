using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace ServiceFactories.Interfaces
{
    public static class FluentAccessorBuilderExtensions
    {
        /// <summary>
        /// Register the IServiceAccessor interface with a scoped lifetime.
        /// </summary>
        /// <param name="builder">Fluent Accessor Builder</param>
        /// <returns>Accessor Builder</returns>
        public static IFluentAccessorBuilder<TService, TKey> AsScopedAccessor<TService, TKey>(
            this IFluentAccessorBuilder<TService, TKey> builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            return builder.ServiceLifetime(ServiceLifetime.Scoped);
        }

        /// <summary>
        /// Register the IServiceAccessor interface with a transient lifetime.
        /// </summary>
        /// <param name="builder">Fluent Accessor Builder</param>
        /// <returns>Accessor Builder</returns>
        public static IFluentAccessorBuilder<TService, TKey> AsTransientAccessor<TService, TKey>(
            this IFluentAccessorBuilder<TService, TKey> builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            return builder.ServiceLifetime(ServiceLifetime.Transient);
        }

        /// <summary>
        /// Register the IServiceAccessor interface with a singleton lifetime.
        /// </summary>
        /// <param name="builder">Fluent Accessor Builder</param>
        /// <returns>Accessor Builder</returns>
        public static IFluentAccessorBuilder<TService, TKey> AsSingletonAccessor<TService, TKey>(
            this IFluentAccessorBuilder<TService, TKey> builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            return builder.ServiceLifetime(ServiceLifetime.Singleton);
        }

        /// <summary>
        /// Function used to build the service asynchronously.
        /// 
        /// Params:
        ///   - ServiceProvider
        /// </summary>
        /// <param name="builder">Fluent Accessor Builder</param>
        /// <param name="asyncResolverFunc">Async Resolver Function</param>
        /// <returns>Accessor Builder</returns>
        public static IFluentAccessorBuilder<TService, TKey> AsyncResolver<TService, TKey>(
            this IFluentAccessorBuilder<TService, TKey> builder,
            Func<IServiceProvider, Task<TService>> asyncResolverFunc)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            return builder.AsyncResolver((p, a) => asyncResolverFunc(p));
        }

        /// <summary>
        /// Function used to build the service asynchronously.
        /// 
        /// Params:
        ///   - ServiceProvider
        /// </summary>
        /// <param name="builder">Fluent Accessor Builder</param>
        /// <param name="asyncResolverFunc">Async Resolver Function</param>
        /// <returns>Accessor Builder</returns>
        public static IFluentAccessorBuilder<TService, TKey> AsyncResolver<TService, TKey>(
            this IFluentAccessorBuilder<TService, TKey> builder,
            Func<object[], Task<TService>> asyncResolverFunc)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            return builder.AsyncResolver((p, a) => asyncResolverFunc(a));
        }

        /// <summary>
        /// Function used to build the service asynchronously.
        /// </summary>
        /// <param name="builder">Fluent Accessor Builder</param>
        /// <param name="asyncResolverFunc">Async Resolver Function</param>
        /// <returns>Accessor Builder</returns>
        public static IFluentAccessorBuilder<TService, TKey> AsyncResolver<TService, TKey>(
            this IFluentAccessorBuilder<TService, TKey> builder, Func<Task<TService>> asyncResolverFunc)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            return builder.AsyncResolver((p, a) => asyncResolverFunc());
        }

        /// <summary>
        /// Function used to build the service synchronously.
        ///
        /// Params:
        ///   - ServiceProvider
        /// </summary>
        /// <param name="builder">Fluent Accessor Builder</param>
        /// <param name="resolverFunc">Resolver Function</param>
        /// <returns>Accessor Builder</returns>
        public static IFluentAccessorBuilder<TService, TKey> SyncResolver<TService, TKey>(
            this IFluentAccessorBuilder<TService, TKey> builder, Func<IServiceProvider, TService> resolverFunc)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            return builder.SyncResolver((p, a) => resolverFunc(p));
        }

        /// <summary>
        /// Function used to build the service synchronously.
        ///
        /// Params:
        ///   - ServiceProvider
        /// </summary>
        /// <param name="builder">Fluent Accessor Builder</param>
        /// <param name="resolverFunc">Resolver Function</param>
        /// <returns>Accessor Builder</returns>
        public static IFluentAccessorBuilder<TService, TKey> SyncResolver<TService, TKey>(
            this IFluentAccessorBuilder<TService, TKey> builder, Func<object[], TService> resolverFunc)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            return builder.SyncResolver((p, a) => resolverFunc(a));
        }

        /// <summary>
        /// Function used to build the service synchronously.
        /// </summary>
        /// <param name="builder">Fluent Accessor Builder</param>
        /// <param name="resolverFunc">Resolver Function</param>
        /// <returns>Accessor Builder</returns>
        public static IFluentAccessorBuilder<TService, TKey> SyncResolver<TService, TKey>(
            this IFluentAccessorBuilder<TService, TKey> builder, Func<TService> resolverFunc)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            return builder.SyncResolver((p, a) => resolverFunc());
        }

        /// <summary>
        /// Function used to validate a key.
        /// 
        /// Params:
        ///   - Key to check.
        /// </summary>
        /// <param name="builder">Fluent Accessor Builder</param>
        /// <param name="serviceKeyResolver">Function used to check and see if a key matches with Accessor.</param>
        /// <returns>Accessor Builder</returns>
        public static IFluentAccessorBuilder<TService, TKey> CanResolveKey<TService, TKey>(
            this IFluentAccessorBuilder<TService, TKey> builder, Func<TKey, bool> serviceKeyResolver)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            return builder.CanResolveKey((k, s) => serviceKeyResolver(k));
        }
    }
}

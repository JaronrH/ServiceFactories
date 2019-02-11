using System;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace ServiceFactories.Interfaces
{
    public static class FluentAccessorBuildersExtensions
    {
        /// <summary>
        /// Register the IServiceAccessor interface with a scoped lifetime.
        /// </summary>
        /// <param name="builder">Fluent Accessor Builder</param>
        /// <returns>Accessor Builder</returns>
        public static IFluentAccessorBuilders<TService, TKey> AsScopedAccessors<TService, TKey>(
            this IFluentAccessorBuilders<TService, TKey> builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            return builder.ServiceLifetimes(ServiceLifetime.Scoped);
        }

        /// <summary>
        /// Register the IServiceAccessor interface with a transient lifetime.
        /// </summary>
        /// <param name="builder">Fluent Accessor Builder</param>
        /// <returns>Accessor Builder</returns>
        public static IFluentAccessorBuilders<TService, TKey> AsTransientAccessors<TService, TKey>(this IFluentAccessorBuilders<TService, TKey> builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            return builder.ServiceLifetimes(ServiceLifetime.Transient);
        }

        /// <summary>
        /// Register the IServiceAccessor interface with a singleton lifetime.
        /// </summary>
        /// <param name="builder">Fluent Accessor Builder</param>
        /// <returns>Accessor Builder</returns>
        public static IFluentAccessorBuilders<TService, TKey> AsSingletonAccessors<TService, TKey>(this IFluentAccessorBuilders<TService, TKey> builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            return builder.ServiceLifetimes(ServiceLifetime.Singleton);
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
        public static IFluentAccessorBuilders<TService, TKey> CanResolveKey<TService, TKey>(
            this IFluentAccessorBuilders<TService, TKey> builder, Func<TKey, bool> serviceKeyResolver)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            return builder.CanResolveKey((k, s) => serviceKeyResolver(k));
        }

        /// <summary>
        /// Register a Scoped Factory for Accessors
        /// </summary>
        /// <param name="builder">Fluent Accessor Builder</param>
        /// <returns>Accessor Builder</returns>
        public static IFluentAccessorBuilders<TService, TKey> AddScopedFactory<TService, TKey>(
            this IFluentAccessorBuilders<TService, TKey> builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            return builder.AddFactory(ServiceLifetime.Scoped);
        }

        /// <summary>
        /// Register a Singleton Factory for Accessors
        /// </summary>
        /// <param name="builder">Fluent Accessor Builder</param>
        /// <returns>Accessor Builder</returns>
        public static IFluentAccessorBuilders<TService, TKey> AddSingletonFactory<TService, TKey>(
            this IFluentAccessorBuilders<TService, TKey> builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            return builder.AddFactory(ServiceLifetime.Singleton);
        }

        /// <summary>
        /// Register a Transient Factory for Accessors
        /// </summary>
        /// <param name="builder">Fluent Accessor Builder</param>
        /// <returns>Accessor Builder</returns>
        public static IFluentAccessorBuilders<TService, TKey> AddTransientFactory<TService, TKey>(
            this IFluentAccessorBuilders<TService, TKey> builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            return builder.AddFactory(ServiceLifetime.Transient);
        }
    }
}
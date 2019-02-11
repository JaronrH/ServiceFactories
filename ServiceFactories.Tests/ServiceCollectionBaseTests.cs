using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ServiceAccessor.Interfaces;
using ServiceFactories.Tests.Components;

namespace ServiceFactories.Tests
{
    public abstract class ServiceCollectionTestsBase
    {
        public enum ServiceType
        {
            Custom,
            SyncSingletonResolverAccessor,
            AsyncSingletonResolverAccessor,
            SyncAsyncSingletonResolverAccessor
        }

        public static string ServiceKey(ServiceLifetime lifetime, ServiceType serviceType, ServiceLifetime? resolverLifetime = null)
        {
            if (serviceType != ServiceType.Custom && resolverLifetime == null) throw new ArgumentNullException(nameof(resolverLifetime), "resolverLifetime can only be null when the ServiceType is Custom.");
            if (resolverLifetime.HasValue && resolverLifetime == ServiceLifetime.Scoped) throw new ArgumentOutOfRangeException(nameof(resolverLifetime), "resolverLifetime can not be Scoped.");
            switch (serviceType)
            {
                case ServiceType.Custom:
                    return $"{lifetime}CustomAccessor";
                case ServiceType.SyncSingletonResolverAccessor:
                    return $"Sync{lifetime}{resolverLifetime}ResolverAccessor";
                case ServiceType.AsyncSingletonResolverAccessor:
                    return $"Async{lifetime}{resolverLifetime}ResolverAccessor";
                case ServiceType.SyncAsyncSingletonResolverAccessor:
                    return $"SyncAsync{lifetime}{resolverLifetime}ResolverAccessor";
                default:
                    throw new ArgumentOutOfRangeException(nameof(serviceType), serviceType, null);
            }
        }

        public static Func<IServiceCollection, ServiceLifetime, IServiceCollection> AddTestComponents = (s, l) =>
        {
            s.AddServiceFactory<ITestService, string>(l);
            AddLifetimeAccessors(s, ServiceLifetime.Scoped);
            AddLifetimeAccessors(s, ServiceLifetime.Singleton);
            AddLifetimeAccessors(s, ServiceLifetime.Transient);
            return s;
        };


        public static Func<IServiceCollection, ServiceLifetime, IServiceCollection>  AddLifetimeAccessors= (s, l) => s
            .AddServiceAccessor(l, () => new TestAccessor<string>(ServiceKey(l, ServiceType.Custom)))
            .AddServiceAccessors<ITestService, string>(b => b
                .ServiceLifetimes(l)
                .SingletonResolvers()
                .AddAccessor(d => d
                    .WithKey(ServiceKey(l, ServiceType.SyncSingletonResolverAccessor, ServiceLifetime.Singleton))
                    .SyncResolver(a => new TestImplementation($"{a[0]}-Sync{l}SingletonResolverAccessor-Sync"))
                )
                .AddAccessor(d => d
                    .WithKey(ServiceKey(l, ServiceType.AsyncSingletonResolverAccessor, ServiceLifetime.Singleton))
                    .AsyncResolver(a => Task.FromResult((ITestService) new TestImplementation($"{a[0]}-Async{l}SingletonResolverAccessor-Async")))
                )
                .AddAccessor(d => d
                    .WithKey(ServiceKey(l, ServiceType.SyncAsyncSingletonResolverAccessor, ServiceLifetime.Singleton))
                    .SyncResolver(a => new TestImplementation($"{a[0]}-SyncAsync{l}SingletonResolverAccessor-Sync"))
                    .AsyncResolver(a => Task.FromResult((ITestService) new TestImplementation($"{a[0]}-SyncAsync{l}SingletonResolverAccessor-Async")))
                )
            )
            .AddServiceAccessors<ITestService, string>(b => b
                .ServiceLifetimes(l)
                .TransientResolvers()
                .AddAccessor(d => d
                    .WithKey(ServiceKey(l, ServiceType.SyncSingletonResolverAccessor, ServiceLifetime.Transient))
                    .SyncResolver(a => new TestImplementation($"{a[0]}-Sync{l}TransientResolverAccessor-Sync"))
                )
                .AddAccessor(d => d
                    .WithKey(ServiceKey(l, ServiceType.AsyncSingletonResolverAccessor, ServiceLifetime.Transient))
                    .AsyncResolver(a => Task.FromResult((ITestService) new TestImplementation($"{a[0]}-Async{l}TransientResolverAccessor-Async")))
                )
                .AddAccessor(d => d
                    .WithKey(ServiceKey(l, ServiceType.SyncAsyncSingletonResolverAccessor, ServiceLifetime.Transient))
                    .SyncResolver(a => new TestImplementation($"{a[0]}-SyncAsync{l}TransientResolverAccessor-Sync"))
                    .AsyncResolver(a => Task.FromResult((ITestService) new TestImplementation($"{a[0]}-SyncAsync{l}TransientResolverAccessor-Async")))
                )
            );

        protected ServiceCollectionTestsBase(Action<IServiceCollection> serviceCollectionAction)
        {
            Collection = new ServiceCollection();
            serviceCollectionAction(Collection);
        }

        protected IServiceCollection Collection { get; }
    }
}
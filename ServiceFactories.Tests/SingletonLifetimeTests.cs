using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ServiceFactories.Interfaces;
using ServiceFactories.Tests.Components;
using Xunit;

namespace ServiceFactories.Tests
{
    public class SingletonLifetimeTests : ServiceCollectionTestsBase
    {
        public SingletonLifetimeTests(): base(s => AddTestComponents(s, ServiceLifetime.Singleton)) { }

        [Fact]
        public void CanResolveAll()
        {
            var provider = Collection.BuildServiceProvider();
            var factory = provider.GetRequiredFactory<ITestService, string>();
            foreach (var lifetime in Enum.GetValues(typeof(ServiceLifetime)).Cast<ServiceLifetime>())
            {
                foreach (var serviceType in Enum.GetValues(typeof(ServiceType)).Cast<ServiceType>())
                {
                    if (serviceType == ServiceType.Custom)
                        Assert.True(factory.CanResolve(ServiceKey(lifetime, serviceType)));
                    else
                        foreach (var resolverLifetime in new[] { ServiceLifetime.Singleton, ServiceLifetime.Transient })
                            Assert.True(factory.CanResolve(ServiceKey(lifetime, serviceType, resolverLifetime)));
                }
            }
        }

        [Fact]
        public void FactoryScopeTest()
        {
            var provider = Collection.BuildServiceProvider();
            var factory1 = provider.GetRequiredFactory<ITestService, string>();
            var factory2 = provider.GetRequiredFactory<ITestService, string>();
            Assert.Equal(factory1, factory2);
        }

        [Fact]
        public async Task AsyncCallsSyncResolve()
        {
            var provider = Collection.BuildServiceProvider();
            var factory = provider.GetRequiredFactory<ITestService, string>();

            foreach (var lifetime in Enum.GetValues(typeof(ServiceLifetime)).Cast<ServiceLifetime>())
            {
                foreach (var resolverLifetime in new[] { ServiceLifetime.Singleton, ServiceLifetime.Transient})
                {
                    var key = ServiceKey(lifetime, ServiceType.SyncSingletonResolverAccessor, resolverLifetime);
                    var service = await factory.ResolveAsync(key, "Test");
                    Assert.Equal($"Test-{key}-Sync", service.Name);
                    service = factory.Resolve(key, "Test");
                    Assert.Equal($"Test-{key}-Sync", service.Name);
                }
            }
        }

        [Fact]
        public async Task SyncCallsAsyncResolve()
        {
            var provider = Collection.BuildServiceProvider();
            var factory = provider.GetRequiredFactory<ITestService, string>();

            foreach (var lifetime in Enum.GetValues(typeof(ServiceLifetime)).Cast<ServiceLifetime>())
            {
                foreach (var resolverLifetime in new[] { ServiceLifetime.Singleton, ServiceLifetime.Transient })
                {
                    var key = ServiceKey(lifetime, ServiceType.AsyncSingletonResolverAccessor, resolverLifetime);
                    var service = factory.Resolve(key, "Test");
                    Assert.Equal($"Test-{key}-Async", service.Name);
                    service = await factory.ResolveAsync(key, "Test");
                    Assert.Equal($"Test-{key}-Async", service.Name);
                }
            }
        }

        [Fact]
        public async Task SyncAsyncResolvesIndividually()
        {
            var provider = Collection.BuildServiceProvider();
            var factory = provider.GetRequiredFactory<ITestService, string>();

            foreach (var lifetime in Enum.GetValues(typeof(ServiceLifetime)).Cast<ServiceLifetime>())
            {
                foreach (var resolverLifetime in new[] { ServiceLifetime.Singleton, ServiceLifetime.Transient })
                {
                    var key = ServiceKey(lifetime, ServiceType.SyncAsyncSingletonResolverAccessor, resolverLifetime);
                    var service = factory.Resolve(key, "Test");
                    Assert.Equal($"Test-{key}-Sync", service.Name);
                    service = await factory.ResolveAsync(key, "Test");
                    Assert.Equal(
                        resolverLifetime == ServiceLifetime.Transient 
                            ? $"Test-{key}-Async" 
                            : $"Test-{key}-Sync",
                        service.Name);
                }
            }
        }

        [Fact]
        public void AccessorLifetimeChecks()
        {
            var provider = Collection.BuildServiceProvider();

            // Check within one factory
            var factory1 = provider.GetRequiredFactory<ITestService, string>();
            foreach (var lifetime in Enum.GetValues(typeof(ServiceLifetime)).Cast<ServiceLifetime>())
            {
                foreach (var serviceType in Enum.GetValues(typeof(ServiceType)).Cast<ServiceType>())
                {
                    if (serviceType == ServiceType.Custom)
                        Assert.Equal(factory1.GetAccessor(ServiceKey(lifetime, serviceType)), factory1.GetAccessor(ServiceKey(lifetime, serviceType)));
                    else
                        foreach (var resolverLifetime in new[] { ServiceLifetime.Singleton, ServiceLifetime.Transient })
                            Assert.Equal(factory1.GetAccessor(ServiceKey(lifetime, serviceType, resolverLifetime)), factory1.GetAccessor(ServiceKey(lifetime, serviceType, resolverLifetime)));
                }
            }

            // Check between two factories
            var factory2 = provider.GetRequiredFactory<ITestService, string>();
            foreach (var lifetime in Enum.GetValues(typeof(ServiceLifetime)).Cast<ServiceLifetime>())
            {
                foreach (var serviceType in Enum.GetValues(typeof(ServiceType)).Cast<ServiceType>())
                {
                    if (serviceType == ServiceType.Custom)
                        Assert.Equal(factory1.GetAccessor(ServiceKey(lifetime, serviceType)), factory2.GetAccessor(ServiceKey(lifetime, serviceType)));
                    else
                        foreach (var resolverLifetime in new[] { ServiceLifetime.Singleton, ServiceLifetime.Transient })
                            Assert.Equal(factory1.GetAccessor(ServiceKey(lifetime, serviceType, resolverLifetime)), factory2.GetAccessor(ServiceKey(lifetime, serviceType, resolverLifetime)));
                }
            }
        }

        [Fact]
        public async Task ResolverLifetimeChecksAsync()
        {
            var provider = Collection.BuildServiceProvider();

            // Check within one factory
            var factory1 = provider.GetRequiredFactory<ITestService, string>();
            foreach (var lifetime in Enum.GetValues(typeof(ServiceLifetime)).Cast<ServiceLifetime>())
            {
                foreach (var serviceType in Enum.GetValues(typeof(ServiceType)).Cast<ServiceType>())
                {
                    if (serviceType == ServiceType.Custom)
                        Assert.NotEqual(factory1.Resolve(ServiceKey(lifetime, serviceType), "Test"), factory1.Resolve(ServiceKey(lifetime, serviceType), "Test"));
                    else
                    {
                        Assert.Equal(factory1.Resolve(ServiceKey(lifetime, serviceType, ServiceLifetime.Singleton), "Test"), factory1.Resolve(ServiceKey(lifetime, serviceType, ServiceLifetime.Singleton), "Test"));
                        Assert.NotEqual(factory1.Resolve(ServiceKey(lifetime, serviceType, ServiceLifetime.Transient), "Test"), factory1.Resolve(ServiceKey(lifetime, serviceType, ServiceLifetime.Transient), "Test"));
                    }
                }
            }

            // Check between two factories
            var factory2 = provider.GetRequiredFactory<ITestService, string>();
            foreach (var lifetime in Enum.GetValues(typeof(ServiceLifetime)).Cast<ServiceLifetime>())
            {
                foreach (var serviceType in Enum.GetValues(typeof(ServiceType)).Cast<ServiceType>())
                {
                    if (serviceType == ServiceType.Custom)
                        Assert.NotEqual(await factory1.ResolveAsync(ServiceKey(lifetime, serviceType), "Test"), await factory2.ResolveAsync(ServiceKey(lifetime, serviceType), "Test"));
                    else
                    {
                        Assert.Equal(await factory1.ResolveAsync(ServiceKey(lifetime, serviceType, ServiceLifetime.Singleton), "Test"), await factory2.ResolveAsync(ServiceKey(lifetime, serviceType, ServiceLifetime.Singleton), "Test"));
                        Assert.NotEqual(await factory1.ResolveAsync(ServiceKey(lifetime, serviceType, ServiceLifetime.Transient), "Test"), await factory2.ResolveAsync(ServiceKey(lifetime, serviceType, ServiceLifetime.Transient), "Test"));
                    }
                }
            }
        }

        [Fact]
        public void ResolverLifetimeChecksSync()
        {
            var provider = Collection.BuildServiceProvider();

            // Check within one factory
            var factory1 = provider.GetRequiredFactory<ITestService, string>();
            foreach (var lifetime in Enum.GetValues(typeof(ServiceLifetime)).Cast<ServiceLifetime>())
            {
                foreach (var serviceType in Enum.GetValues(typeof(ServiceType)).Cast<ServiceType>())
                {
                    if (serviceType == ServiceType.Custom)
                        Assert.NotEqual(factory1.Resolve(ServiceKey(lifetime, serviceType), "Test"), factory1.Resolve(ServiceKey(lifetime, serviceType), "Test"));
                    else
                    {
                        Assert.Equal(factory1.Resolve(ServiceKey(lifetime, serviceType, ServiceLifetime.Singleton), "Test"), factory1.Resolve(ServiceKey(lifetime, serviceType, ServiceLifetime.Singleton), "Test"));
                        Assert.NotEqual(factory1.Resolve(ServiceKey(lifetime, serviceType, ServiceLifetime.Transient), "Test"), factory1.Resolve(ServiceKey(lifetime, serviceType, ServiceLifetime.Transient), "Test"));
                    }
                }
            }

            // Check between two factories
            var factory2 = provider.GetRequiredFactory<ITestService, string>();
            foreach (var lifetime in Enum.GetValues(typeof(ServiceLifetime)).Cast<ServiceLifetime>())
            {
                foreach (var serviceType in Enum.GetValues(typeof(ServiceType)).Cast<ServiceType>())
                {
                    if (serviceType == ServiceType.Custom)
                        Assert.NotEqual(factory1.Resolve(ServiceKey(lifetime, serviceType), "Test"), factory2.Resolve(ServiceKey(lifetime, serviceType), "Test"));
                    else
                    {
                        Assert.Equal(factory1.Resolve(ServiceKey(lifetime, serviceType, ServiceLifetime.Singleton), "Test"), factory2.Resolve(ServiceKey(lifetime, serviceType, ServiceLifetime.Singleton), "Test"));
                        Assert.NotEqual(factory1.Resolve(ServiceKey(lifetime, serviceType, ServiceLifetime.Transient), "Test"), factory2.Resolve(ServiceKey(lifetime, serviceType, ServiceLifetime.Transient), "Test"));
                    }
                }
            }
        }

        [Fact]
        public async Task ResolverLifetimeChecksAsyncVsSync()
        {
            var provider = Collection.BuildServiceProvider();
            var factory = provider.GetRequiredFactory<ITestService, string>();
            foreach (var lifetime in Enum.GetValues(typeof(ServiceLifetime)).Cast<ServiceLifetime>())
            {
                foreach (var serviceType in Enum.GetValues(typeof(ServiceType)).Cast<ServiceType>())
                {
                    if (serviceType == ServiceType.Custom)
                        Assert.NotEqual(await factory.ResolveAsync(ServiceKey(lifetime, serviceType), "Test"), factory.Resolve(ServiceKey(lifetime, serviceType), "Test"));
                    else
                    {
                        Assert.Equal(await factory.ResolveAsync(ServiceKey(lifetime, serviceType, ServiceLifetime.Singleton), "Test"), factory.Resolve(ServiceKey(lifetime, serviceType, ServiceLifetime.Singleton), "Test"));
                        Assert.NotEqual(await factory.ResolveAsync(ServiceKey(lifetime, serviceType, ServiceLifetime.Transient), "Test"), factory.Resolve(ServiceKey(lifetime, serviceType, ServiceLifetime.Transient), "Test"));
                    }
                }
            }
        }
    }
}
using System;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ServiceFactories.Tests
{
    public class ServiceCollectionTestsBaseTests : ServiceCollectionTestsBase
    {
        public ServiceCollectionTestsBaseTests() : base(s => AddTestComponents(s, ServiceLifetime.Singleton))
        {
        }

        [Theory]
        [InlineData(typeof(ArgumentNullException), ServiceLifetime.Singleton, ServiceType.SyncAsyncSingletonResolverAccessor, null)]
        [InlineData(typeof(ArgumentOutOfRangeException), ServiceLifetime.Singleton, ServiceType.SyncAsyncSingletonResolverAccessor, ServiceLifetime.Scoped)]
        public void ServiceKeyTests(Type exceptionType, ServiceLifetime lifetime, ServiceType serviceType,
            ServiceLifetime? resolverLifetime)
        {
            Assert.Throws(exceptionType, () => ServiceKey(lifetime, serviceType, resolverLifetime));
        }

        [Fact]
        public void CollectionServiceDescriptorCountTest()
        {
            Assert.Equal(22, Collection.Count);
        }
    }
}
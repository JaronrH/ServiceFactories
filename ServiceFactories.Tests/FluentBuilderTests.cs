using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using ServiceAccessor.Interfaces;
using ServiceFactories.Interfaces;
using ServiceFactories.Tests.Components;
using Xunit;

namespace ServiceFactories.Tests
{
    public class FluentBuilderTests : ServiceCollectionTestsBase
    {
        public FluentBuilderTests() : base(s => s.AddSingletonServiceFactory<ITestService, string>()) { }

        private IServiceAccessor<ITestService, string> BuildAndGetAccessor(Action<IFluentAccessorBuilder<ITestService, string>> builder, string serviceKey)
        {
            return Collection
                .AddServiceAccessor(builder)
                .BuildServiceProvider()
                .GetRequiredFactory<ITestService, string>()
                .GetAccessor(serviceKey);
        }

        [Fact]
        public void AccessorServiceLifetimeNotDefinedTest()
        {
            Assert.Equal("Accessor Service Lifetime is not defined.", Assert.Throws<Exception>(() => Collection.AddServiceAccessor<ITestService, string>(
                b => {})).Message);
        }

        [Fact]
        public void AccessorResolverTypeNotDefinedTest()
        {
            Assert.Equal("Accessor's Resolver type is not defined.", Assert.Throws<Exception>(() => Collection.AddServiceAccessor<ITestService, string>(
                b => { b
                    .ServiceLifetime(ServiceLifetime.Singleton)
                ;})).Message);
        }

        [Fact]
        public void AccessorKeyNotDefinedTest()
        {
            Assert.Equal("No Accessor Key(s) defined.", Assert.Throws<Exception>(() => Collection.AddServiceAccessor<ITestService, string>(
                b => { b
                        .ServiceLifetime(ServiceLifetime.Singleton)
                        .SingletonResolver()
                ;})).Message);
        }

        [Fact]
        public void AccessorResolverNotDefinedTest()
        {
            Assert.Equal("Neither a synchronous or asynchronous Service Resolver is defined.", Assert.Throws<Exception>(() => Collection.AddServiceAccessor<ITestService, string>(
                b => {b
                        .ServiceLifetime(ServiceLifetime.Singleton)
                        .SingletonResolver()
                        .WithKey("Test")
                ;})).Message);
        }

        [Fact]
        public void NullAsyncResolverTest()
        {
            Assert.Throws<ArgumentNullException>(() => Collection.AddServiceAccessor<ITestService, string>(
                b => {
                    b
                        .AsyncResolver(null)
                ;}));
        }

        [Fact]
        public void NullSyncResolverTest()
        {
            Assert.Throws<ArgumentNullException>(() => Collection.AddServiceAccessor<ITestService, string>(
                b => {
                    b
                        .SyncResolver(null)
                ;}));
        }

        [Fact]
        public void NullCanResolveKeyTest()
        {
            Assert.Throws<ArgumentNullException>(() => Collection.AddServiceAccessor<ITestService, string>(
                b => {
                    b
                        .CanResolveKey(null)
                ;}));
        }

        [Fact]
        public void SingletonResolverTest()
        {
            var accessor = BuildAndGetAccessor(b => b
                    .ServiceLifetime(ServiceLifetime.Singleton)
                    .SingletonResolver()
                    .WithKey("Test")
                    .SyncResolver(() => new TestImplementation("Test"))
                , "Test");
            Assert.Equal(accessor.Resolve(), accessor.Resolve());
        }

        [Fact]
        public void TransientResolverTest()
        {
            var accessor = BuildAndGetAccessor(b => b
                    .ServiceLifetime(ServiceLifetime.Singleton)
                    .TransientResolver()
                    .WithKey("Test")
                    .SyncResolver(() => new TestImplementation("Test"))
                , "Test");
            Assert.NotEqual(accessor.Resolve(), accessor.Resolve());
        }

        [Fact]
        public void SingletonAccessorTest()
        {
            var collection = new ServiceCollection()
                .AddServiceAccessor<ITestService, string>(b => b
                    .AsSingletonAccessor()
                    .TransientResolver()
                    .WithKey("Test")
                    .SyncResolver(() => new TestImplementation("Test"))
                );
            Assert.Equal(ServiceLifetime.Singleton, collection[0].Lifetime);
        }

        [Fact]
        public void ScopedAccessorTest()
        {
            var collection = new ServiceCollection()
                .AddServiceAccessor<ITestService, string>(b => b
                    .AsScopedAccessor()
                    .TransientResolver()
                    .WithKey("Test")
                    .SyncResolver(() => new TestImplementation("Test"))
                );
            Assert.Equal(ServiceLifetime.Scoped, collection[0].Lifetime);
        }

        [Fact]
        public void TransientAccessorTest()
        {
            var collection = new ServiceCollection()
                .AddServiceAccessor<ITestService, string>(b => b
                    .AsTransientAccessor()
                    .TransientResolver()
                    .WithKey("Test")
                    .SyncResolver(() => new TestImplementation("Test"))
                );
            Assert.Equal(ServiceLifetime.Transient, collection[0].Lifetime);
        }

        [Fact]
        public void CanResolveKeyTest()
        {
            var accessor = BuildAndGetAccessor(b => b
                    .ServiceLifetime(ServiceLifetime.Singleton)
                    .TransientResolver()
                    .WithKey("Test")
                    .SyncResolver(() => new TestImplementation("Test"))
                    .CanResolveKey((k,l) => l.All(i => i != k))
                , "NotTest");
            Assert.False(accessor.CanResolve("Test"));
            Assert.True(accessor.CanResolve("StillNotTest"));
        }
    }
}

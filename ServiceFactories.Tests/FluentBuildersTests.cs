using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using ServiceAccessor.Interfaces;
using ServiceFactories.Interfaces;
using ServiceFactories.Tests.Components;
using Xunit;

namespace ServiceFactories.Tests
{
    public class FluentBuildersTests
    {

        private IServiceFactory<ITestService, string> BuildAndGetAccessor(Action<IFluentAccessorBuilders<ITestService, string>> builders)
        {
            return new ServiceCollection()
                .AddServiceAccessors(builders)
                .BuildServiceProvider()
                .GetRequiredFactory<ITestService, string>();
        }

        [Fact]
        public void AddScopedFactoryTest()
        {
            var collection = new ServiceCollection()
                .AddServiceAccessors<ITestService, string>(b => b
                    .AddScopedFactory()
                );
            Assert.Single(collection);
            Assert.Equal(ServiceLifetime.Scoped, collection[0].Lifetime);
        }

        [Fact]
        public void AddSingletonFactoryTest()
        {
            var collection = new ServiceCollection()
                .AddServiceAccessors<ITestService, string>(b => b
                    .AddSingletonFactory()
                );
            Assert.Single(collection);
            Assert.Equal(ServiceLifetime.Singleton, collection[0].Lifetime);
        }

        [Fact]
        public void AddTransientFactoryTest()
        {
            var collection = new ServiceCollection()
                .AddServiceAccessors<ITestService, string>(b => b
                    .AddTransientFactory()
                );
            Assert.Single(collection);
            Assert.Equal(ServiceLifetime.Transient, collection[0].Lifetime);
        }

        [Fact]
        public void ScopedAccessorTest()
        {
            var provider = new ServiceCollection()
                .AddServiceAccessors<ITestService, string>(b => b
                    .AddTransientFactory()
                    .AsScopedAccessors()
                    .SingletonResolvers()
                    .AddAccessor(a => a
                        .WithKey("Test")
                        .SyncResolver(() => new TestImplementation("Test"))
                    )
                )
                .BuildServiceProvider();
            var factory1 = provider.GetRequiredFactory<ITestService, string>();
            var factory2 = provider.GetRequiredFactory<ITestService, string>();
            Assert.Equal(factory1.GetAccessor("Test"), factory2.GetAccessor("Test"));

            // Override in AddAccessor
            provider = new ServiceCollection()
                .AddServiceAccessors<ITestService, string>(b => b
                    .AddTransientFactory()
                    .AsScopedAccessors()
                    .SingletonResolvers()
                    .AddAccessor(a => a
                        .AsTransientAccessor()
                        .WithKey("Test")
                        .SyncResolver(() => new TestImplementation("Test"))
                    )
                )
                .BuildServiceProvider();
            factory1 = provider.GetRequiredFactory<ITestService, string>();
            factory2 = provider.GetRequiredFactory<ITestService, string>();
            Assert.NotEqual(factory1.GetAccessor("Test"), factory2.GetAccessor("Test"));
        }

        [Fact]
        public void SingletonAccessorTest()
        {
            var provider = new ServiceCollection()
                .AddServiceAccessors<ITestService, string>(b => b
                    .AddTransientFactory()
                    .AsSingletonAccessors()
                    .SingletonResolvers()
                    .AddAccessor(a => a
                        .WithKey("Test")
                        .SyncResolver(() => new TestImplementation("Test"))
                    )
                )
                .BuildServiceProvider();
            var factory1 = provider.GetRequiredFactory<ITestService, string>();
            var factory2 = provider.GetRequiredFactory<ITestService, string>();
            Assert.Equal(factory1.GetAccessor("Test"), factory2.GetAccessor("Test"));

            // Override in AddAccessor
            provider = new ServiceCollection()
                .AddServiceAccessors<ITestService, string>(b => b
                    .AddTransientFactory()
                    .AsScopedAccessors()
                    .SingletonResolvers()
                    .AddAccessor(a => a
                        .AsTransientAccessor()
                        .WithKey("Test")
                        .SyncResolver(() => new TestImplementation("Test"))
                    )
                )
                .BuildServiceProvider();
            factory1 = provider.GetRequiredFactory<ITestService, string>();
            factory2 = provider.GetRequiredFactory<ITestService, string>();
            Assert.NotEqual(factory1.GetAccessor("Test"), factory2.GetAccessor("Test"));
        }

        [Fact]
        public void TransientAccessorTest()
        {
            var provider = new ServiceCollection()
                .AddServiceAccessors<ITestService, string>(b => b
                    .AddTransientFactory()
                    .AsTransientAccessors()
                    .SingletonResolvers()
                    .AddAccessor(a => a
                        .WithKey("Test")
                        .SyncResolver(() => new TestImplementation("Test"))
                    )
                )
                .BuildServiceProvider();
            var factory1 = provider.GetRequiredFactory<ITestService, string>();
            var factory2 = provider.GetRequiredFactory<ITestService, string>();
            Assert.NotEqual(factory1.GetAccessor("Test"), factory2.GetAccessor("Test"));

            // Override in AddAccessor
            provider = new ServiceCollection()
                .AddServiceAccessors<ITestService, string>(b => b
                    .AddTransientFactory()
                    .AsScopedAccessors()
                    .SingletonResolvers()
                    .AddAccessor(a => a
                        .AsSingletonAccessor()
                        .WithKey("Test")
                        .SyncResolver(() => new TestImplementation("Test"))
                    )
                )
                .BuildServiceProvider();
            factory1 = provider.GetRequiredFactory<ITestService, string>();
            factory2 = provider.GetRequiredFactory<ITestService, string>();
            Assert.Equal(factory1.GetAccessor("Test"), factory2.GetAccessor("Test"));
        }



        [Fact]
        public void SingletonResolversTest()
        {
            var factory = BuildAndGetAccessor(b => b
                .AddSingletonFactory()
                .AsSingletonAccessors()
                .SingletonResolvers()
                .AddAccessor(a => a
                    .WithKey("Test")
                    .SyncResolver(() => new TestImplementation("Test"))
                )
            );
            Assert.Equal(factory.GetAccessor("Test").Resolve(), factory.GetAccessor("Test").Resolve());

            // Override in AddAccessor
            factory = BuildAndGetAccessor(b => b
                .AddSingletonFactory()
                .AsSingletonAccessors()
                .SingletonResolvers()
                .AddAccessor(a => a
                    .TransientResolver()
                    .WithKey("Test")
                    .SyncResolver(() => new TestImplementation("Test"))
                )
            );
            Assert.NotEqual(factory.GetAccessor("Test").Resolve(), factory.GetAccessor("Test").Resolve());
        }

        [Fact]
        public void TransientResolversTest()
        {
            var factory = BuildAndGetAccessor(b => b
                .AddSingletonFactory()
                .AsSingletonAccessors()
                .TransientResolvers()
                .AddAccessor(a => a
                    .WithKey("Test")
                    .SyncResolver(() => new TestImplementation("Test"))
                )
            );
            Assert.NotEqual(factory.GetAccessor("Test").Resolve(), factory.GetAccessor("Test").Resolve());

            // Override in AddAccessor
            factory = BuildAndGetAccessor(b => b
                .AddSingletonFactory()
                .AsSingletonAccessors()
                .TransientResolvers()
                .AddAccessor(a => a
                    .SingletonResolver()
                    .WithKey("Test")
                    .SyncResolver(() => new TestImplementation("Test"))
                )
            );
            Assert.Equal(factory.GetAccessor("Test").Resolve(), factory.GetAccessor("Test").Resolve());
        }

        [Fact]
        public void CanResolveKeyTest()
        {
            var factory = BuildAndGetAccessor(b => b
                .AddSingletonFactory()
                .AsSingletonAccessors()
                .SingletonResolvers()
                .CanResolveKey((k,l) => k.StartsWith("Valid") && l.Contains(k))
                .AddAccessor(a => a
                    .WithKey("Valid1")
                    .SyncResolver(() => new TestImplementation("Valid1"))
                )
                .AddAccessor(a => a
                    .WithKey("Valid2")
                    .SyncResolver(() => new TestImplementation("Valid2"))
                )
                .AddAccessor(a => a
                    .WithKey("Invalid")
                    .SyncResolver(() => new TestImplementation("Invalid"))
                )
                .AddAccessor(a => a
                    .WithKey("EverythingElse")
                    .CanResolveKey((k, l) => k.StartsWith("Valid") && !l.Contains(k))
                    .SyncResolver(() => new TestImplementation("EverythingElse"))
                )
            );
            Assert.True(factory.CanResolve("Valid1"));
            Assert.Equal("Valid1", factory.Resolve("Valid1").Name);
            Assert.True(factory.CanResolve("Valid2"));
            Assert.Equal("Valid2", factory.Resolve("Valid2").Name);
            Assert.True(factory.CanResolve("Validxxxxxxxxxxx"));
            Assert.Equal("EverythingElse", factory.Resolve("Validxxxxxxxxxxx").Name);
            Assert.False(factory.CanResolve("Invalid"));
            Assert.Throws<Exception>(() => factory.Resolve("Invalid"));
        }

        [Fact]
        public void NullCanResolveKeyTest()
        {
            Assert.Throws<ArgumentNullException>(() => new ServiceCollection().AddServiceAccessors<ITestService, string>(
                b => {
                    b
                        .CanResolveKey(null)
                ;}));
        }
    }
}
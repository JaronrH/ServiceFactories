using System;
using System.Threading.Tasks;
using ServiceFactories.Interfaces;

namespace ServiceFactories.Tests.Components
{
    public class TestAccessor<TKey> : IServiceAccessor<ITestService, TKey> where TKey : IComparable
    {
        public TestAccessor(TKey key)
        {
            Key = key;
        }

        public TKey Key { get; set; }

        #region Implementation of IServiceAccessor<ITest,in TKey>

        /// <summary>
        /// Can this Service Accessor resolve a service key?
        /// </summary>
        /// <param name="serviceKey">Key to check.</param>
        /// <returns>If an Accessor can resolve this key or not.</returns>
        public bool CanResolve(TKey serviceKey)
        {
            return Key.CompareTo(serviceKey) == 0;
        }

        /// <summary>
        /// Resolve Service Synchronously.
        /// </summary>
        /// <param name="args">Arguments for Service Resolver.</param>
        /// <returns>Service or null</returns>
        public ITestService Resolve(params object[] args)
        {
            return new TestImplementation($"{args[0]}-{Key}-Sync");
        }

        /// <summary>
        /// Resolve Service Asynchronously.
        /// </summary>
        /// <param name="args">Arguments for Service Resolver.</param>
        /// <returns>Service or null</returns>
        public Task<ITestService> ResolveAsync(params object[] args)
        {
            return Task.FromResult((ITestService)new TestImplementation($"{args[0]}-TestAccessor-{Key}-Async"));
        }

        #endregion
    }
}
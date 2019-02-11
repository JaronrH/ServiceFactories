using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ServiceFactories
{
    /// <inheritdoc />
    /// <summary>
    /// Async <see cref="T:System.Lazy`1" /> implementation.
    /// </summary>
    /// <typeparam name="TResult">Result type that is created lazily.</typeparam>
    public class AsyncLazy<TResult> : Lazy<Task<TResult>>
    {
        /// <inheritdoc />
        /// <summary>
        /// Create AsyncLazy with a Value Factory (sync function to create TResult).
        /// </summary>
        /// <param name="valueFactory">Function used to create a new TResult.</param>
        public AsyncLazy(Func<TResult> valueFactory) :
            base(() => Task.Factory.StartNew(valueFactory))
        { }

        /// <inheritdoc />
        /// <summary>
        /// Create AsyncLazy with a Task Factory (async function to create TResult).
        /// </summary>
        /// <param name="taskFactory">Function to create a new Task that will create TResult</param>
        public AsyncLazy(Func<Task<TResult>> taskFactory) :
            base(() => Task.Factory.StartNew(taskFactory).Unwrap())
        { }

        /// <summary>
        /// Get Awaiter for Async Task.
        /// </summary>
        /// <returns><see cref="TaskAwaiter{TResult}"/></returns>
        public TaskAwaiter<TResult> GetAwaiter() { return Value.GetAwaiter(); }
    }
}
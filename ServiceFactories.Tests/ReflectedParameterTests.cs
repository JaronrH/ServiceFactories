using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ServiceFactories.Interfaces;
using ServiceFactories.Tests.Components;
using Xunit;

namespace ServiceFactories.Tests
{
    public class ReflectedParameterTests
    {
        [Theory]
        [InlineData(typeof(AccessorDependencyInjectionExtensions))]
        [InlineData(typeof(FluentAccessorBuilderExtensions))]
        [InlineData(typeof(FluentAccessorBuildersExtensions))]
        [InlineData(typeof(ServiceFactoryDependencyInjectionExtensions))]
        [InlineData(typeof(ServiceFactoryExtensions))]
        [InlineData(typeof(ServiceProviderExtensions))]
        public async Task ThisArgumentNullTests(Type type)
        {
            var genericArgumentsDictionary = new Dictionary<string, Type>()
            {
                {"TService", typeof(ITestService)},
                {"TKey", typeof(string) },
                {"TServiceAccessor", typeof(TestAccessor<string>)}
            };
            var methodInfos = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
            foreach (var methodInfo in methodInfos)
                if (methodInfo.ContainsGenericParameters)
                {
                    var nonGenericMethodInfo = methodInfo.MakeGenericMethod(methodInfo.GetGenericArguments()
                        .Select(i => genericArgumentsDictionary[i.Name]).ToArray());
                    await ValidateMethodThrowsArgumentNullException(nonGenericMethodInfo);
                }
                else
                    await ValidateMethodThrowsArgumentNullException(methodInfo);
        }

        private async Task ValidateMethodThrowsArgumentNullException(MethodInfo methodInfo)
        {
            if (methodInfo.Name.EndsWith("Async"))
                await Assert.ThrowsAsync<ArgumentNullException>(() =>
                    (Task)methodInfo
                        .Invoke(null, methodInfo
                            .GetParameters()
                            .Select(i =>
                                i.ParameterType.IsValueType ? Activator.CreateInstance(i.ParameterType) : null)
                            .ToArray()
                        ));
            else
                Assert.Equal(typeof(ArgumentNullException),

                    Assert.Throws<TargetInvocationException>(() =>
                            methodInfo
                                .Invoke(null, methodInfo
                                    .GetParameters()
                                    .Select(i =>
                                        i.ParameterType.IsValueType ? Activator.CreateInstance(i.ParameterType) : null)
                                    .ToArray()
                                ))
                        .InnerException.GetType()
                );
        }
    }
}

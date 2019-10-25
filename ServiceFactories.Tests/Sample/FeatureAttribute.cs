using System;

namespace ServiceFactories.Tests.Sample
{
    /// <summary>
    /// Tag a class that inherits <see cref="IFeatureService"/> for registration of a <see cref="Features"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class FeatureAttribute : Attribute, IFeatureDescriptor
    {
        public FeatureAttribute(Features feature, string message)
        {
            Feature = feature;
            Message = message;
        }

        /// <summary>
        /// Feature implemented in service.
        /// </summary>
        public Features Feature { get; }

        /// <summary>
        /// Message Feature Provides.
        /// </summary>
        public string Message { get; }
    }
}
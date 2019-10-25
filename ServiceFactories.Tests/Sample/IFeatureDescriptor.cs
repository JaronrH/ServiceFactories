namespace ServiceFactories.Tests.Sample
{
    /// <summary>
    /// Feature Descriptor
    /// </summary>
    public interface IFeatureDescriptor
    {
        /// <summary>
        /// Feature implemented in service.
        /// </summary>
        Features Feature { get; }

        /// <summary>
        /// Message Feature Provides.
        /// </summary>
        string Message { get; }
    }
}
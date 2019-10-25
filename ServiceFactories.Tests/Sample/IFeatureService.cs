namespace ServiceFactories.Tests.Sample
{
    public interface IFeatureService
    {
        /// <summary>
        /// Number
        /// </summary>
        int Number { get; set; }

        /// <summary>
        /// Message.
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// Get a message from this feature.
        /// </summary>
        /// <returns></returns>
        string GetFeatureMessage();
    }
}
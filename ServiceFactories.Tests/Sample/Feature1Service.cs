namespace ServiceFactories.Tests.Sample
{
    [Feature(Features.Feature1, "Feature1 Attribute Message")]
    public class Feature1Service : IFeatureService
    {
        #region Implementation of IFeatureService

        /// <summary>
        /// Number
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Get a message from this feature.
        /// </summary>
        /// <returns></returns>
        public string GetFeatureMessage() => $"Feature 1: {Message}; Input Number: {Number}";

        #endregion
    }
}
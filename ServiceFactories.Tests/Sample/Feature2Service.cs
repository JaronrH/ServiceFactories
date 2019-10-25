namespace ServiceFactories.Tests.Sample
{
    [Feature(Features.Feature2, "Feature2 Attribute Message")]
    public class Feature2Service : IFeatureService
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
        public string GetFeatureMessage() => $"Feature 2: {Message}; Input Number: {Number}";

        #endregion
    }
}
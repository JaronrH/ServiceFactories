using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ServiceFactories.Interfaces;
using Xunit;

namespace ServiceFactories.Tests.Sample
{
    public class SampleTests
    {
        private readonly IServiceFactory<IFeatureService, Features> _factory;

        /// <summary>
        /// Example Setup.
        /// </summary>
        public SampleTests()
        {
            // Create Service Collection
            var services = new ServiceCollection();

            // Get this assembly for interrogation
            var assemblies = new [] {typeof(SampleTests).Assembly};

            // Get Feature Service Type
            var featureServiceInterfaceType = typeof(IFeatureService);

            // Register Service Implementations and Accessors
            foreach (var toRegister in assemblies
                .SelectMany(a => a.ExportedTypes) // Get all Exported Types (this is expensive so be sure to only scan the assemblies you need!)
                .Where(t => 
                    t.IsClass // Just look for classes
                    && !t.IsAbstract // Make sure the classes found are not abstract!
                    && featureServiceInterfaceType.IsAssignableFrom(t) // Makes sure the class implements our service interface
                    && t.GetCustomAttribute<FeatureAttribute>() != null // Make sure the class contains our custom attribute
                    )
                .Select(t => (t, t.GetCustomAttribute<FeatureAttribute>() as IFeatureDescriptor)) // Select both the type found and the descriptor (from the attribute)
            )
            {
                services.AddScoped(toRegister.Item1); // Add the IFeatureService implementation class as itself to DI
                services.AddServiceAccessor<IFeatureService, Features>(a => a // Create a fluently build accessor that uses Features as the key
                    .AsTransientAccessor() // Accessor will be created every time a factory is created
                    .SingletonResolver() // Resolved Service are created once inside the accessor.  Therefore, the same service should always be returned regardless of the input param.
                    .SyncResolver((s, args) => // Create Service Synchronously
                    {
                        var service = (IFeatureService) s.GetRequiredService(toRegister.Item1); // Build Service from DI
                        service.Message = toRegister.Item2.Message; // Set Message from FeatureAttribute
                        service.Number = (int)args[0]; // Set Number from Creation Argument
                        return service; // Return service
                    })
                    .CanResolveKey(i => i == toRegister.Item2.Feature) // Create function that determines if this accessor should be used or not.
                );
            }

            // Add Service Factory
            services.AddSingletonServiceFactory<IFeatureService, Features>();

            // Build Service provider
            var provider = services.BuildServiceProvider();

            // Get Factory from provider
            _factory = provider.GetRequiredFactory<IFeatureService, Features>();
        }

        /// <summary>
        /// Resolve Feature 1 and make sure it is valid.
        /// </summary>
        [Fact]
        public void ResolveFeature1Tests()
        {
            // Can we resolve this feature?
            Assert.True(_factory.CanResolve(Features.Feature1));

            // Get Accessor
            var accessor = _factory.GetAccessor(Features.Feature1);

            // Get Service with argument of 14
            var service = accessor.Resolve(14); 

            // Make sure it was created!
            Assert.NotNull(service);

            // Validate Service
            Assert.Equal("Feature1 Attribute Message", service.Message);
            Assert.Equal(14, service.Number);
            Assert.Equal("Feature 1: Feature1 Attribute Message; Input Number: 14", service.GetFeatureMessage());

            // Feature Services should be singleton scoped (reused) so creating an instance with arg 18 should return the same service.
            Assert.Equal(service.GetHashCode(), accessor.Resolve(18).GetHashCode());
        }

        /// <summary>
        /// Resolve Feature 2 and make sure it is valid.
        /// </summary>
        [Fact]
        public void ResolveFeature2Tests()
        {
            // Can we resolve this feature?
            Assert.True(_factory.CanResolve(Features.Feature2));

            // Get Accessor
            var accessor = _factory.GetAccessor(Features.Feature2);

            // Get Service with argument of 14
            var service = accessor.Resolve(14);

            // Make sure it was created!
            Assert.NotNull(service);

            // Validate Service
            Assert.Equal("Feature2 Attribute Message", service.Message);
            Assert.Equal(14, service.Number);
            Assert.Equal("Feature 2: Feature2 Attribute Message; Input Number: 14", service.GetFeatureMessage());

            // Feature Services should be singleton scoped (reused) so creating an instance with arg 18 should return the same service.
            Assert.Equal(service.GetHashCode(), accessor.Resolve(18).GetHashCode());
        }

        /// <summary>
        /// Verify that Feature 3 cannot be resolved.
        /// </summary>
        [Fact]
        public void ResolveFeature3Tests()
        {
            // Can we resolve this feature? (Nope!)
            Assert.False(_factory.CanResolve(Features.Feature3));

            // Get Accessor
            Assert.Equal("No service accessor defined for Feature3.", Assert.Throws<Exception>(() => _factory.GetAccessor(Features.Feature3)).Message);
        }

        /// <summary>
        /// Example on how to verify that all features are found in the assemblies.
        ///
        /// Can be easily used with #if DEBUG...#endif directives to only scan on debug builds.
        /// </summary>
        [Fact]
        public void FindMissingFeaturesTest()
        {
            // Get this assembly for interrogation (like in creation in constructor)
            var assemblies = new[] { typeof(SampleTests).Assembly };

            // Get Feature Service Type (like in creation in constructor)
            var featureServiceInterfaceType = typeof(IFeatureService);

            // Get all implemented features
            var featuressAccountedFor = assemblies
                .SelectMany(a => a.ExportedTypes)
                .Where(t =>
                        t.IsClass // Just look for classes
                        && !t.IsAbstract // Make sure the classes found are not abstract!
                        && featureServiceInterfaceType.IsAssignableFrom(t) // Makes sure the class implements our service interface
                        && t.GetCustomAttribute<FeatureAttribute>() != null // Make sure the class contains our custom attribute
                )
                .Select(t => t.GetCustomAttribute<FeatureAttribute>().Feature) // Get Feature
                .Distinct() // Make list distinct
                .ToArray();

            // Build list of missing features
            var missingFeatureTypes = Enum
                .GetValues(typeof(Features)) // Get all Features
                .Cast<Features>() // Cast to Feature Type
                .Where(i => !featuressAccountedFor.Contains(i)) // Filter to only return non-implemented features
                .ToArray();

            // If any missing, throw error
            /*
            if (missingFeatureTypes.Any()) 
                throw new Exception($"Features missing for {string.Join(", ", missingFeatureTypes)}");
            */
            Assert.Single(missingFeatureTypes);
            Assert.Contains(Features.Feature3, missingFeatureTypes);

        }
    }
}

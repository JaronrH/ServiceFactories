# Service Factories DI Extensions
This is an extension to Microsoft's .Net Core DI framework which was written to allow for services to be created via factories.  Essentially, it creates a service witch uses a key to select the correct factory and therefore create the desired service.
## Definitions

 - **Factory**: Service registered in DI that uses a Key to lookup a service.  Factories can be resolved using *IServiceFactory<TServiceType, TKeyType>*.
	 - *IServiceFactory<TServiceType, TKeyType>.CanResolve(TKey)* -->  Can a factory be resolved with the provided key?
	 - *IServiceFactory<TServiceType, TKeyType>.GetAccessor(TKey)* -->  Get a service accessor?
 - **Accessor**: Service registered in DI that is used to create a specific service on behalf of the factory.  Accessors are resolved using *IServiceAccessor<TServiceType, TKeyType>*.
	  - *IServiceFactory<TServiceType, TKeyType>.CanResolve(TKey)* -->  Can this accessor resolved a provided key?
	  - *IServiceFactory<TServiceType, TKeyType>.Resolve(args)* -->  Synchronously resolve a factory using the provided arguments (arguments are optional). 
	  - *IServiceFactory<TServiceType, TKeyType>.ResolveAsync(args)* -->  Asynchronously resolve a factory using the provided arguments (arguments are optional).

## Extensions and Fluent Configuration
While the above interfaces can be manually created, registered, and used, there are extension methods available to handle the broiler-plate work that can be used as desired.

 - Fluently-created Factories use DI to pass in a collection of *IServiceAccessor<TService, TKey>* implementations.  (These implementations should be as light-weight as possible since DI creates them while creating the factory itself.)
 - Fluently-created Accessors have the following features:
	 - Custom Scope for registration in DI
	 - Custom Key validation to see when a TKey applies to a service.  By default, a simple key comparison is used to resolve an accessor:
	 `key is IComparable
                        ? keys.Any(i => ((IComparable) key).CompareTo(i) == 0)
                        : keys.Contains(key)`
	 - Sync and Async Service Resolvers to create a service when a key is valid.  If only one is defined, the other will execute the defined version.  (Ex: if only a Sync resolver is defined, the Async will call it.)
	 - Resolver can use a key comparison 
	 - Services can be created once and cached (Singleton) or created every time (Transient).  (Singleton implementation uses a *SemaphoreSlim* to wrap the creation.)

### IServiceCollection.Add{Lifecycle}ServiceAccessor(), IServiceCollection.AddServiceAccessor(), etc.
This extension aids in creating Service accessors.  There are overloaded versions to register custom registrations:

    services.AddTransientServiceAccessor<TService, TKey, TServiceAccessor>
effectively registers

    services.Add(new ServiceDescriptor(typeof(IServiceAccessor<TService, TKey>), typeof(TServiceAccessor), ServiceLifetime.Transient));
In most cases however, the accessor will have very triveal implementations.  Therefore, a fluent configuration can allow accessors to be created.  For example:

    services.AddServiceAccessor<IFeatureService, FeaturesEnum>(a => a // Create a fluently build accessor that uses Features as the key
	    .AsTransientAccessor() // Accessor will be created every time a factory is created
	    .SingletonResolver() // Resolved Service are created once inside the accessor.  Therefore, the same service should always be returned regardless of the input param.
	    .SyncResolver(s => (IFeatureService)s.GetRequiredService(toRegister))
	    .CanResolveKey(i => i == Feature) // Create function that determines if this accessor should be used or not.
Accessors can be added in bulk, fluently, using *IServiceCollection.AddServiceAccessors()*

    services.AddServiceAccessors<ITestService, string>(b => b
	    .AddTransientFactory() // Register Factory as Transient so it is created every time
	    .AsScopedAccessors() // Add Accessors as Scoped by default
	    .SingletonResolvers() // Accessors create services once and re-use them by default
	    .AddAccessor(a => a
	        .AsTransientAccessor() // This accessor overrides the global setting and makes a new service each time
	        .WithKey("Test") // Key that matches this
	        .SyncResolver(() => new TestImplementation("Test")) // Service Creation
	    )
	)

### IServiceCollection.AddServiceAccessor()
Just like accessors, Factories can be created and registered fluently.  For example:

    services.AddSingletonServiceFactory<IFeatureService, Features>();
Uses the default factory and registers in DI as

    services.Add(new ServiceDescriptor(typeof(IServiceFactory<IFeatureService, Features>), typeof(ServiceFactory<IFeatureService, Features>), ServiceLifetime.Singleton));

### Getting Services from  *IServiceProvider*
There are extensions that can be used to help get factories and services from DI.

    provider.GetFactory<TService, TKey>() // Get a Factory or Null
    provider.GetRequiredFactory<TService, TKey>() // Get a required Factory.  Throws exception if unable to do so (similar to GetRequiredService())
    provider.GetService<TService, TKey>(TKey key, params []) // Automatically get a factory and resolve the service using the provided key and params syncronosly.  Returns null if unable to resolve.
    await provider.GetServiceAsync<TService, TKey>(TKey key, params []) // Automatically get a factory and resolve the service using the provided key and params asyncronosly.  Returns null if unable to resolve.
    provider.GetRequiredService<TService, TKey>(TKey key, params []) // Automatically get a factory and resolve the service using the provided key and params syncronosly. Throws exception if unable to do so (similar to GetRequiredService())
    await provider.GetRequiredServiceAsync<TService, TKey>(TKey key, params []) // Automatically get a factory and resolve the service using the provided key and params asyncronosly. Throws exception if unable to do so (similar to GetRequiredService())

## Scopes
Scopes are both the most powerful and confusing aspect to these factories.  Care will need to be taken to make sure all three parts are scoped correctly:

 1. The Factory itself can be created from DI as a Singleton, Transient, and Scope-level.
	 -- Singleton: Accessors cannot be Scope.  They must be Transient or Singleton.
	 -- Transient: Accessors cannot be Scope.  They must be Transient or Singleton.
	 -- Scope: Accessors can be Scope, Transient, or Singleton.
 2. Each Accessor that is created and injected via DI into the factories can have any of the above scope levels.
	 -- Singleton: Can create Singleton and Transient services.  Customer accessors can create scoped services if desired via a custom implementation of *IServiceAccessor<TServiceType, TKeyType>*.
	 -- Transient: Can create Singleton and Transient services.  Customer accessors can create scoped services if desired via a custom implementation of *IServiceAccessor<TServiceType, TKeyType>*.
	 -- Scope: Can create Scope, Transient, or Singleton services.
 3. Each fluent accessor created can either create a service once (Singleton) or every time (Transient).
	-- Singleton: Fluent Accessor will create a service once and cache it. 
	-- Transient: Fluent Accessor will create a new service every time.

### Things to look out for:

 - If a factory is defined as a singleton but an accessor is scoped, DI will throw an error since a singleton service cannot have scoped services injected into it.
 - A fluent-created accessor that is a Singleton cannot create a scoped service for the same reason as the above.  A Transient accessor *may* be able to if the Factory it belongs to was created in Transient or in Scope.
 - A fluent-created accessor that builds a service based on input arguments should be setup as a transient resolver.  This way, a new service is created each time with the new arguments.  Otherwise, only the first service created with the first arguments will be used.

## Check Out
 - Samples/Examples in xUnit tests
	 - SampleTests show how to use an Attribute and assembly scanning to register services as well as scan for missing services (see *SampleTests.FindMissingFeaturesTest*)
 - Markdown Writer: [https://stackedit.io/app](https://stackedit.io/app)
 - Scrutor [https://github.com/khellang/Scrutor](https://github.com/khellang/Scrutor)
	 - Excellent Guide: [https://andrewlock.net/using-scrutor-to-automatically-register-your-services-with-the-asp-net-core-di-container/](https://andrewlock.net/using-scrutor-to-automatically-register-your-services-with-the-asp-net-core-di-container/)

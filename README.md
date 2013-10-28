IocLite
=======

IOC Lite is an IOC container that can be used in projects that need a lightweight internal DI container. It can be
especially useful if you are building a library and need to use an IOC container, but also want to provide your users
with an easy way to either use your internal IOC to register their own dependencies or swap out your internal IOC 
container and use their own container like Structure Map, or Castle Windsor.

IOC Lite is not a full fledged IOC Container like StructureMap, or Castle Windsor and only supports a subset of their
functionality. However, it does support constructor injection and object lifetime management.

## Getting Started & Register, Resolve, Release Pattern

###Register

To get started with IOC Lite, in your *Global.asax* create a new instance of `Container`.

``` c#
protected void Application_Start()
{
    _container = new Container();
    _container.Register(new List<IRegistry>
    {
        new IocRegistry()
    });
}
```

To register your dependencies create a new class that dervies from `Registry` and override the `Load()` method.

``` c#
public class IocRegistry : Registry
{
    public override void Load()
    {
        For<IVideoGameRepository>().Use<VideoGameRepository>();
        For<IConsoleRepository>().Use<ConsoleRepository>();
    }
}
```

IOC Lite provides a fluent-stle API for registering your dependencies.

When registering dependencies you can also specify the object lifetime. 
By default dependencies will use the `Default Scope` and a new instance of the type will be created everytime
you request one from the container.

``` c#
public class IocRegistry : Registry
{
    public override void Load()
    {
        For<IVideoGameRepository>().Use<VideoGameRepository>().InSingletonScope();
        For<IConsoleRepository>().Use<ConsoleRepository>();
    }
}
```

###Resolve

To resolve an instance of a type you can use the following methods on the container.

``` c#
var instance = _container.Resolve(typeof(IVideoGameRepository));
```

``` c#
var instance = _container.Resolve<IVideoGameRepository>();
```

###Release

## Object Lifetime

IOC Lite provides a few options for object lifetime management. By default types registered with the container will
be created each time you try and resolve the type from the container. You can optionally change the scope of the object
to one of the following object scopes:

| Scope         | Description                |
| ------------- | -------------------------- |
| Default       | The container will create a new instance of the type everytime one is requested. |
| Singleton     | The container will create a single instance of the type the first time one is request, and will return that one instance for all future requests for the lifetime of the application. |
| Thread        | Like singleton scope, only the container will create a new instance per thread.
| Http Request  | Lite singleton and thread scope, only the container will create a new instance per http request. |


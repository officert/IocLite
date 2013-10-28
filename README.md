IocLite
=======

IOC Lite is an IOC container that can be used in projects that need a lightweight internal DI container. It can be
especially useful if you are building a library and need to use an IOC container, but also want to provide your users
with an easy way to either use your internal IOC to register their own dependencies or swap out your internal IOC 
container and use their own container like Structure Map, or Castle Windsor.

IOC Lite is not a full fledged IOC Container like StructureMap, or Castle Windsor and only supports a subset of their
functionality. However, it does support constructor injection and object lifetime management.

## Getting Started & Registering your dependencies

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

IOC Lite uses the Register, Resolve, Release pattern.



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


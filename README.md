IocLite
=======

IOC Lite is a lightweight IOC container that can be used in projects that need an internal DI container, as well as the
ability to allow a user to use their own IOC container.

IOC Lite is not a full fledged IOC Container like StructureMap, or Castle Windsor. It is only supports constructor
injection, does not support auto registration and only has a few options for object lifetime management, and is only
meant to be used in web applications.

## Getting Started & Registering your dependencies

To get started with IOC Lite, in your *Global.asax* create a new instance of `Container`.

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


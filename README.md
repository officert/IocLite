IocLite
=======

IOC Lite is a lightweight IOC container that can be used in projects that need an internal DI container, as well as the
ability to allow a user to use their own IOC container.

IOC Lite is not a full fledged IOC Container like StructureMap, or Castle Windsor. It is only supports constructor
injection, does not support auto registration and only has a few options for object lifetime management.


## Object Lifetime

IOC Lite provides a few options for object lifetime management. By default types registered with the container will
be created each time you try and resolve the type from the container. You can optionally change the scope of the object
to one of the following object scopes:



﻿using System;
using System.Web.Mvc;
using System.Web.Routing;
using IocLite.Interfaces;

namespace IocLite.SampleApp.Ioc
{
    public class IocControllerFactory : DefaultControllerFactory
    {
        private readonly IContainer _container;

        public IocControllerFactory(IContainer container)
        {
            _container = container;
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return (IController)_container.Resolve(controllerType);
        }
    }
}
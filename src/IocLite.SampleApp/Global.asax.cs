using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using IocLite.Interfaces;
using IocLite.SampleApp.Data;

namespace IocLite.SampleApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private IContainer _container;

        protected void Application_Start()
        {
            _container = new Container();

            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            _container.Register(new List<IRegistry>
            {
                new IocRegistry()
            });

            ControllerBuilder.Current.SetControllerFactory(new IocControllerFactory(_container));
        }
    }

    public class IocRegistry : Registry
    {
        public override void Load()
        {
            For<IVideoGameRepository>().Use<VideoGameRepository>();
        }
    }

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
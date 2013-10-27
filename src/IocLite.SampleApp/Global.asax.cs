using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using IocLite.Interfaces;
using IocLite.SampleApp.Data;

namespace IocLite.SampleApp
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
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

            _container.For<IVideoGameRepository>().Use<VideoGameRepository>();

            ControllerBuilder.Current.SetControllerFactory(new IocControllerFactory(_container));
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
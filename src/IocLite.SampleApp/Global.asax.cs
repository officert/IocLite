using System.Collections.Generic;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using IocLite.Interfaces;
using IocLite.SampleApp.Ioc;

namespace IocLite.SampleApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private IContainer _container;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            _container = new Container();
            _container.Register(new List<IRegistry>
            {
                new IocRegistry()
            });

            ControllerBuilder.Current.SetControllerFactory(new IocControllerFactory(_container));
        }
    }
}
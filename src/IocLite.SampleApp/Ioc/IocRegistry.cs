using IocLite.SampleApp.Data;

namespace IocLite.SampleApp.Ioc
{
    public class IocRegistry : Registry
    {
        public override void Load()
        {
            For<IVideoGameRepository>().Use<VideoGameRepository>();
            For<IConsoleRepository>().Use<ConsoleRepository>();
        }
    }
}
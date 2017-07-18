using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MOLUX.Startup))]
namespace MOLUX
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

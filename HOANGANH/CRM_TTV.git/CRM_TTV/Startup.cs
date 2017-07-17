using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CRM_TTV.Startup))]
namespace CRM_TTV
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
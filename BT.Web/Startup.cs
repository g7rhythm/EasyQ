using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BT.Web.Startup))]
namespace BT.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

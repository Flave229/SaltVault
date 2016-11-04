using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HouseFinance.Startup))]
namespace HouseFinance
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

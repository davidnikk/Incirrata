using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Incirrata.Startup))]
namespace Incirrata
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

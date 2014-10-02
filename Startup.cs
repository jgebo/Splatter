using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Splatter.Startup))]
namespace Splatter
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ADO_Demo.Startup))]
namespace ADO_Demo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

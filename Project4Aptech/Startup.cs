using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Project4Aptech.Startup))]
namespace Project4Aptech
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}

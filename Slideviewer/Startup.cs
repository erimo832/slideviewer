using Microsoft.Owin;
using Owin;

namespace ControlHub
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
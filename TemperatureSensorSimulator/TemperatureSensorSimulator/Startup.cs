using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;

[assembly: OwinStartup(typeof(TemperatureSensorSimulator.Startup))]

namespace TemperatureSensorSimulator
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //Adding the custome hubpipeline into the Globalhost
            GlobalHost.HubPipeline.AddModule(new CustomHandlingHubPipeLineModule());
           
            //allowing the cross platform
            app.UseCors(CorsOptions.AllowAll);
            
            //mapping the signalR into app
            app.MapSignalR();
            
        }
    }
}

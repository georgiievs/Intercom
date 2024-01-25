using Intercom.Application;
using Intercom.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Intercom
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingletonDatabase();

                    services.AddSingleton<AudioSpeaker>();
                    services.AddSingleton<IntercomController>();
                    services.AddSingleton<App>();

                    services.AddSingleton<OutdoorWindow>();
                    
                }).Build();
            
            var app = host.Services.GetRequiredService<App>();

            app.Run();    
        }   
    }
}
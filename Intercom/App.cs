using Intercom.Application;
using Intercom.Windows;
using System.Windows;

namespace Intercom
{
    public class App : System.Windows.Application
    {
        public App(OutdoorWindow outdoorWindow)
        {
            MainWindow = outdoorWindow;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow.Show();

            base.OnStartup(e);
        }
    }
}

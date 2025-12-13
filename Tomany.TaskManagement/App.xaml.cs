using System.Configuration;
using System.Data;
using System.Windows;

namespace Tomany.TaskManagement
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Load environment variables from .env file
            DotNetEnv.Env.Load();
        }
    }

}

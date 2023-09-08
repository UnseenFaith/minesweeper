using System.Windows;
using System.Windows.Media;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public void OnStartup(object sender, StartupEventArgs e)
        {
            var Settings = Minesweeper.Properties.Settings.Default;

            if (Settings.UPGRADE_REQUIRED)
            {
                Settings.Upgrade();
                Settings.UPGRADE_REQUIRED = false;
                Settings.Save();
            }
        }

        public void OnExit(object sender, ExitEventArgs e)
        {
            Minesweeper.Properties.Settings.Default.Save();
        }

    }
}

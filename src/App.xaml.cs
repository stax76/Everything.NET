
using System.Windows;
using System.Windows.Threading;

namespace EverythingNET
{
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString());
        }

        static AppSettings _Settings;

        public static AppSettings Settings {
            get {
                if (_Settings == null)
                    _Settings = SettingsManager.Load();

                return _Settings;
            }
        }
    }
}

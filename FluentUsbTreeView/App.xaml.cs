using FluentUsbTreeView.PInvoke;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FluentUsbTreeView {   
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        public static string[] Arguments;

        // Register device update notifications

        private void Application_Startup(object sender, StartupEventArgs e) {

            Arguments = e.Args;

            // Init console ; enables ANSI, unicode, and enables logging in WPF
            Kernel32.AttachConsole(-1);
            Console.OutputEncoding = Encoding.Unicode;
            Kernel32.EnableAnsiCmd();

            Logger.Info($"Received arguments: \"{string.Join(" ", Arguments)}\"");

            DeviceManaged.RegisterDeviceNotificationHandler();
            AppDomain.CurrentDomain.ProcessExit += (s, ev) => DeviceManaged.UnregisterDeviceNotifications();
        }

        private void Application_Exit(object sender, ExitEventArgs e) {
            DeviceManaged.UnregisterDeviceNotifications();
        }

        protected override void OnExit(ExitEventArgs e) {
            DeviceManaged.UnregisterDeviceNotifications();
            base.OnExit(e);
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
            Logger.Fatal($"{e.Exception.Message} ({e.Exception.HResult})\nStacktrace: {e.Exception.StackTrace}");
        }
    }
}

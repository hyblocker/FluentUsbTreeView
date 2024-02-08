using System;
using System.Reflection;
using System.Linq;
using System.Windows;
using FluentUsbTreeView.UsbTreeView;
using System.IO;

namespace FluentUsbTreeView {
    public class Program {
        private static Assembly ExecutingAssembly = Assembly.GetExecutingAssembly();
        private static string[] EmbeddedLibraries = ExecutingAssembly.GetManifestResourceNames().Where(x => x.EndsWith(".dll")).ToArray();

        [STAThreadAttribute]
        public static void Main() {
            string logFilePath = Path.Combine(Settings.Instance.ApplicationDirectory, $"fluent_utv_{DateTime.Now.ToString("yyyyMMdd-HHmmss.ffffff")}.log");
            Logger.Init(logFilePath);

            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => {
                var assemblyName = new AssemblyName(args.Name).Name + ".dll";
                var resourceName = EmbeddedLibraries.FirstOrDefault(x => x.EndsWith(assemblyName));
                if ( resourceName == null )
                    return null;

                using ( var stream = ExecutingAssembly.GetManifestResourceStream(resourceName) ) {
                    var bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);
                    var assembly = Assembly.Load(bytes);

                    if ( assemblyName == "WpfUi.Ui.dll" ) {
                        foreach ( var xaml in new[] { "Controls", "Fonts" } ) {
                            ResourceDictionary rd = new ResourceDictionary();
                            rd.Source = new Uri($"pack://application:,,,/WpfUi.Ui;component/Controls/{xaml}.xaml");
                            Application.Current.Resources.MergedDictionaries.Add(rd);
                        }
                    }

                    return assembly;
                }
            };

            App.Main();
        }
    }
}
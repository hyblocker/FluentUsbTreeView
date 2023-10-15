using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FluentUsbTreeView {
    public static class Util {

        public static void Quit() {
            Task.Run(() => {

                Application.Current.Dispatcher.InvokeShutdown();
                // Wait a few 250ms first, otherwise the window will hang
                Thread.Sleep(250);

                Application.Current.Dispatcher.InvokeAsync(new Action(() => {
                    Application.Current.Shutdown();

                    // @HACK: We should figure out *why* some other threads are keeping the process alive in some scenarios, and fix that behaviour.
                    Environment.Exit(0); // Sometimes we would have a background thread resulting in a zombie process
                }), DispatcherPriority.ContextIdle);
            });
        }

        /// <summary>
        /// Returns whether the current process is elevated or not
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCurrentProcessElevated() {
            WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent();
            WindowsPrincipal currentGroup = new WindowsPrincipal(currentIdentity);
            return currentGroup.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// Elevates the current process by re-launching it, passing all parameters to the new process, with a run as verb.
        /// </summary>
        /// <returns>Whether the application was successfully elevated or not</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ElevateSelf() {
            // try catch catches the user clicking no on the UAC prompt
            try {
                ProcessStartInfo procInfo = new ProcessStartInfo(){

                    // Pass same args
                    FileName            = Process.GetCurrentProcess().MainModule.FileName,
                    Arguments           = string.Join("\" \"", Environment.GetCommandLineArgs()),
                    WorkingDirectory    = Directory.GetCurrentDirectory(),

                    Verb                = "runas" // Force UAC prompt
                };

                Process.Start(procInfo);
                return true;
            } catch {
                return false;
            }
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FluentUsbTreeView {
    /// <summary>
    /// A logging class which mimics GLOG's trace style. While not highly configurable, this is
    /// designed to be a simple drag and drop logger which relies solely on built-in C# APIs
    /// </summary>
    public static class Logger {
        // This is used instead of Thread.CurrentThread.ManagedThreadId since it returns the OS thread rather than the managed thread
        // Consider using ManagedThreadId instead of this if you have to run this on non-Windows platforms
        // https://stackoverflow.com/a/1679270
        [DllImport("Kernel32", EntryPoint = "GetCurrentThreadId", ExactSpelling = true)]
        private static extern int GetCurrentWin32ThreadId();

        public static string LogFilePath;

        private static List<(ConsoleColor, string)> s_logLines = new List<(ConsoleColor, string)>();

        #region Log Functions

        public static void Info(string text, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "") {
            LogInternal(FormatToLogMessage(text, "I", lineNumber, filePath, memberName));
        }

        public static void Info(object obj, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "") {
            LogInternal(FormatToLogMessage(obj.ToString(), "I", lineNumber, filePath, memberName));
        }

        public static void Warn(string text, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "") {
            LogInternal(FormatToLogMessage(text, "W", lineNumber, filePath, memberName), ConsoleColor.Yellow);
        }

        public static void Warn(object obj, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "") {
            LogInternal(FormatToLogMessage(obj.ToString(), "W", lineNumber, filePath, memberName), ConsoleColor.Yellow);
        }

        public static void Error(string text, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "") {
            LogInternal(FormatToLogMessage(text, "E", lineNumber, filePath, memberName), ConsoleColor.Red);
        }

        public static void Error(object obj, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "") {
            LogInternal(FormatToLogMessage(obj.ToString(), "E", lineNumber, filePath, memberName), ConsoleColor.Red);
        }

        public static void Fatal(string text, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "") {
            LogInternal(FormatToLogMessage(text, "F", lineNumber, filePath, memberName), ConsoleColor.DarkRed);
        }

        public static void Fatal(object obj, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "") {
            LogInternal(FormatToLogMessage(obj.ToString(), "F", lineNumber, filePath, memberName), ConsoleColor.DarkRed);
        }

        internal static void PrivateDoNotUseLogExecption(string message, string displayString, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "") {
            LogInternalUniqueMessage(FormatToLogMessage(message, "F", lineNumber, filePath, memberName), FormatToLogMessage(displayString, "F", lineNumber, filePath, memberName), ConsoleColor.DarkRed);
        }

        #endregion

        #region Logger Internals

        /// <summary>
        /// Initializes the logger
        /// </summary>
        public static void Init(string filePath = "") {
            if ( filePath == "" )
                LogFilePath = $"{Assembly.GetCallingAssembly().GetName()}_{DateTime.Now.ToString("yyyyMMdd-HHmmss.ffffff")}.log";
            else
                LogFilePath = filePath;

            LogFilePath = Path.GetFullPath(filePath);
            string dir = Path.GetFullPath(Path.GetDirectoryName(LogFilePath));

            bool loggingPathDidntExist = false;
            if ( !Directory.Exists(dir) ) {
                Directory.CreateDirectory(dir);
                loggingPathDidntExist = true;
            }

            // Create the file, rest of the methods will append
            File.Create(LogFilePath).Close();

            if ( loggingPathDidntExist )
                LogInternal($"Created logging directory at \"{dir}\"");
            LogInternal($"Log file created at: {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}");
            LogInternal($"Running on machine: {Environment.MachineName}");
            LogInternal("Running duration (h:mm:ss): 0:00:00");
            LogInternal("Log line format: [IWEF]yyyyMMdd HH:mm:ss.ffffff threadid file::member:line] msg");
        }

        private static string FormatToLogMessage(string message, string level, int lineNumber, string filePath, string memberName) {
            return $"{level}{DateTime.Now.ToString("yyyyMMdd HH:mm:ss.ffffff")} {string.Format("{0,5:#####}", GetCurrentWin32ThreadId())} {Path.GetFileName(filePath)}::{memberName}:{lineNumber}] {message}";
        }

        private static void LogInternal(string message) {
            Console.ResetColor();
            Console.WriteLine(message);
            if ( LogFilePath == null ) {
                s_logLines.Add((ConsoleColor.White, message));
                return;
            }
            if ( s_logLines.Count > 0 ) {
                for ( int i = 0; i < s_logLines.Count; i++ ) {
                    File.AppendAllLines(LogFilePath, new[] { s_logLines[i].Item2 });
                }
                s_logLines.Clear();
            }
            File.AppendAllLines(LogFilePath, new[] { message });
        }

        private static void LogInternal(string message, ConsoleColor color) {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            if ( LogFilePath == null ) {
                s_logLines.Add((color, message));
                return;
            }
            if ( s_logLines.Count > 0 ) {
                for ( int i = 0; i < s_logLines.Count; i++ ) {
                    File.AppendAllLines(LogFilePath, new[] { s_logLines[i].Item2 });
                }
                s_logLines.Clear();
            }
            File.AppendAllLines(LogFilePath, new[] { message });
        }

        private static void LogInternalUniqueMessage(string message, string messageUI, ConsoleColor color) {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            if ( LogFilePath == null ) {
                s_logLines.Add((color, message));
                return;
            }
            if ( s_logLines.Count > 0 ) {
                for ( int i = 0; i < s_logLines.Count; i++ ) {
                    File.AppendAllLines(LogFilePath, new[] { s_logLines[i].Item2 });
                }
                s_logLines.Clear();
            }
            File.AppendAllLines(LogFilePath, new[] { message });
        }

        #endregion
    }
}

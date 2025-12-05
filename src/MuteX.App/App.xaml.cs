using System;
using System.IO;
using System.Windows;
using Application = System.Windows.Application;

namespace MuteX.App
{
    public partial class App : Application
    {
        public App()
        {
            // Log de excepciones
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                File.WriteAllText("crash.log", e.ExceptionObject.ToString());
            };

            DispatcherUnhandledException += (s, e) =>
            {
                File.WriteAllText("crash.log", e.Exception.ToString());
                e.Handled = true;
            };
        }
    }
}
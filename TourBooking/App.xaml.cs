using System;
using System.IO;
using System.Linq;
using System.Windows;
using TourBooking.Data;
using TourBooking.ViewModels;
using TourBooking.Views;
using TourBooking.Views.Admin;

namespace TourBooking
{
    public partial class App : Application
    {
        public App()
        {
            this.DispatcherUnhandledException += (s, args) =>
            {
                try
                {
                    string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "global_error.log");
                    File.WriteAllText(logPath, args.Exception.ToString());
                }
                catch { }
            };

            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                try
                {
                    string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "global_error.log");
                    File.WriteAllText(logPath, args.ExceptionObject.ToString());
                }
                catch { }
            };
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                base.OnStartup(e);
                AppDbContext.EnsureSeedData();
            }
            catch (Exception ex)
            {
                try
                {
                    string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "crash.log");
                    File.WriteAllText(logPath, ex.ToString());
                }
                catch { }
            }
        }
    }
}
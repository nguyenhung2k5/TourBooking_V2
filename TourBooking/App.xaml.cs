using System;
using System.Linq;
using System.Windows;
using TourBooking.Data; // Khai báo đường dẫn đến AppDbContext
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
                System.IO.File.WriteAllText(@"c:\IT\C#\TourBooking_V2\global_error.log", args.Exception.ToString());
            };
            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                System.IO.File.WriteAllText(@"c:\IT\C#\TourBooking_V2\global_error.log", args.ExceptionObject.ToString());
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
                System.IO.File.WriteAllText(@"c:\IT\C#\TourBooking_V2\crash.log", ex.ToString());
            }
        }
    }
}
using System;
using System.Windows.Input;
using TourBooking.Models;
using TourBooking.Services;

namespace TourBooking.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private object _currentViewModel;
        private string _currentPageTitle;

        public MainViewModel()
        {
            CurrentStaff = SessionService.CurrentStaff;

            DashboardViewModel = new DashboardViewModel();
            TourListViewModel = new TourListViewModel();
            BookingViewModel = new BookingViewModel();
            MyBookingsViewModel = new MyBookingsViewModel();

            NavigateDashboardCommand = new RelayCommand(_ => NavigateToDashboard());
            NavigateToursCommand = new RelayCommand(_ => NavigateToTours());
            NavigateBookingCommand = new RelayCommand(_ => NavigateToBooking());
            NavigateMyBookingsCommand = new RelayCommand(_ => NavigateToMyBookings());
            LogoutCommand = new RelayCommand(_ => Logout());

            NavigateToDashboard();
        }

        public event Action LogoutRequested;

        public Staff CurrentStaff { get; private set; }
        public DashboardViewModel DashboardViewModel { get; private set; }
        public TourListViewModel TourListViewModel { get; private set; }
        public BookingViewModel BookingViewModel { get; private set; }
        public MyBookingsViewModel MyBookingsViewModel { get; private set; }

        public object CurrentViewModel
        {
            get { return _currentViewModel; }
            private set { SetProperty(ref _currentViewModel, value); }
        }

        public string CurrentPageTitle
        {
            get { return _currentPageTitle; }
            private set { SetProperty(ref _currentPageTitle, value); }
        }

        public string StaffDisplayName
        {
            get { return CurrentStaff != null ? CurrentStaff.FullName : "Guest"; }
        }

        public bool IsAdmin
        {
            get { return SessionService.IsAdmin; }
        }

        public ICommand NavigateDashboardCommand { get; private set; }
        public ICommand NavigateToursCommand { get; private set; }
        public ICommand NavigateBookingCommand { get; private set; }
        public ICommand NavigateMyBookingsCommand { get; private set; }
        public ICommand LogoutCommand { get; private set; }

        private void NavigateToDashboard()
        {
            DashboardViewModel.Load();
            CurrentPageTitle = "Dashboard";
            CurrentViewModel = DashboardViewModel;
        }

        private void NavigateToTours()
        {
            TourListViewModel.Load();
            CurrentPageTitle = "Danh sach tour";
            CurrentViewModel = TourListViewModel;
        }

        private void NavigateToBooking()
        {
            BookingViewModel.Load();
            CurrentPageTitle = "Tao booking";
            CurrentViewModel = BookingViewModel;
        }

        private void NavigateToMyBookings()
        {
            MyBookingsViewModel.Load();
            CurrentPageTitle = "Don hang cua toi";
            CurrentViewModel = MyBookingsViewModel;
        }

        private void Logout()
        {
            SessionService.Logout();
            var handler = LogoutRequested;
            if (handler != null)
            {
                handler();
            }
        }
    }
}

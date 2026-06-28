using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TourBooking.Data;
using TourBooking.Models;

namespace TourBooking.ViewModels
{
    public class AdminTourViewModel : BaseViewModel
    {
        private int _totalTours;
        private int _activeTours;
        private double _fillRate;
        private int _inactiveTours;
        private ObservableCollection<Tour> _toursList;
        private ObservableCollection<Tour> _upcomingTours;

        public int TotalTours { get => _totalTours; set { _totalTours = value; OnPropertyChanged(); } }
        public int ActiveTours { get => _activeTours; set { _activeTours = value; OnPropertyChanged(); } }
        public double FillRate { get => _fillRate; set { _fillRate = value; OnPropertyChanged(); } }
        public int InactiveTours { get => _inactiveTours; set { _inactiveTours = value; OnPropertyChanged(); } }
        public ObservableCollection<Tour> ToursList { get => _toursList; set { _toursList = value; OnPropertyChanged(); } }
        public ObservableCollection<Tour> UpcomingTours { get => _upcomingTours; set { _upcomingTours = value; OnPropertyChanged(); } }

        public ICommand AddTourCommand { get; }
        public ICommand EditTourCommand { get; }
        public ICommand DeleteTourCommand { get; }
        public ICommand ExportCsvCommand { get; }

        public AdminTourViewModel()
        {
            AddTourCommand = new RelayCommand<object>(obj => MessageBox.Show("Mở Form thêm mới Tour du lịch!", "Thông báo"));
            EditTourCommand = new RelayCommand<Tour>(tour => MessageBox.Show($"Mở Form sửa Tour: {tour?.TourCode}", "Thông báo"));
            DeleteTourCommand = new RelayCommand<Tour>(ExecuteDeleteTour);
            ExportCsvCommand = new RelayCommand<object>(obj => MessageBox.Show("Đang kết xuất danh sách Tour ra file .CSV...", "Thông báo"));

            LoadDataFromDatabase();
        }

        private void LoadDataFromDatabase()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var allTours = context.Tours.ToList();

                    TotalTours = allTours.Count;
                    ActiveTours = allTours.Count(t => t.IsActive);
                    InactiveTours = allTours.Count(t => !t.IsActive);

                    int totalSlots = allTours.Sum(t => t.TotalSlots);
                    int totalAvailable = allTours.Sum(t => t.AvailableSlots);
                    int totalBooked = totalSlots - totalAvailable;
                    FillRate = totalSlots > 0 ? Math.Round((double)totalBooked / totalSlots * 100, 1) : 0;

                    ToursList = new ObservableCollection<Tour>(allTours);

                    DateTime today = DateTime.Today;
                    var upcoming = context.Tours
                        .Where(t => t.DepartureDate >= today && t.IsActive)
                        .OrderBy(t => t.DepartureDate)
                        .Take(2)
                        .ToList();
                    UpcomingTours = new ObservableCollection<Tour>(upcoming);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối CSDL: {ex.Message}", "Lỗi Hệ Thống");
            }
        }

        private void ExecuteDeleteTour(Tour tour)
        {
            if (tour == null) return;
            if (MessageBox.Show($"Xác nhận xóa Tour: {tour.TourName}?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new AppDbContext())
                    {
                        var dbTour = context.Tours.Find(tour.TourId);
                        if (dbTour != null)
                        {
                            context.Tours.Remove(dbTour);
                            context.SaveChanges();
                            LoadDataFromDatabase();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa: {ex.Message}");
                }
            }
        }
    }
}
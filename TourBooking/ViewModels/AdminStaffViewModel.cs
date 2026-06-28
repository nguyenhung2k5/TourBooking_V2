using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TourBooking.Data;
using TourBooking.Models;

namespace TourBooking.ViewModels
{
    public class AdminStaffViewModel : BaseViewModel
    {
        private ObservableCollection<Staff> _allStaffs;
        private ObservableCollection<Staff> _filteredStaffs;
        private string _searchQuery;
        private int _totalStaffCount;
        private int _activeStaffCount;

        public int TotalStaffCount { get => _totalStaffCount; set { _totalStaffCount = value; OnPropertyChanged(); } }
        public int ActiveStaffCount { get => _activeStaffCount; set { _activeStaffCount = value; OnPropertyChanged(); } }

        public ObservableCollection<Staff> FilteredStaffs
        {
            get => _filteredStaffs;
            set { _filteredStaffs = value; OnPropertyChanged(); }
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged();
                ExecuteSearch();
            }
        }

        public ICommand AddStaffCommand { get; }
        public ICommand EditStaffCommand { get; }
        public ICommand ToggleStatusCommand { get; }

        public AdminStaffViewModel()
        {
            AddStaffCommand = new RelayCommand<object>(obj => MessageBox.Show("Mở Form thêm nhân viên mới!", "Thông báo"));
            EditStaffCommand = new RelayCommand<Staff>(staff => MessageBox.Show($"Sửa thông tin nhân viên: {staff?.FullName}", "Thông báo"));
            ToggleStatusCommand = new RelayCommand<Staff>(ExecuteToggleStatus);

            LoadDataFromDatabase();
        }

        private void LoadDataFromDatabase()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var staffsList = context.Staffs.ToList();

                    TotalStaffCount = staffsList.Count;
                    ActiveStaffCount = staffsList.Count(s => s.IsActive);

                    _allStaffs = new ObservableCollection<Staff>(staffsList);
                    FilteredStaffs = new ObservableCollection<Staff>(staffsList);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi Hệ Thống");
                _allStaffs = new ObservableCollection<Staff>();
                FilteredStaffs = new ObservableCollection<Staff>();
            }
        }

        private void ExecuteSearch()
        {
            if (_allStaffs == null) return;

            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                FilteredStaffs = new ObservableCollection<Staff>(_allStaffs);
                return;
            }

            var results = _allStaffs.Where(s =>
                (s.FullName != null && s.FullName.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0) ||
                (s.StaffCode != null && s.StaffCode.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0));

            FilteredStaffs = new ObservableCollection<Staff>(results.ToList());
        }

        private void ExecuteToggleStatus(Staff staff)
        {
            if (staff == null) return;

            string actionText = staff.IsActive ? "Khóa" : "Kích hoạt";
            if (MessageBox.Show($"Bạn có chắc chắn muốn {actionText} tài khoản của {staff.FullName}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new AppDbContext())
                    {
                        var dbStaff = context.Staffs.Find(staff.StaffId);
                        if (dbStaff != null)
                        {
                            dbStaff.IsActive = !dbStaff.IsActive;
                            context.SaveChanges();

                            LoadDataFromDatabase();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi cập nhật trạng thái: {ex.Message}", "Lỗi");
                }
            }
        }
    }
}
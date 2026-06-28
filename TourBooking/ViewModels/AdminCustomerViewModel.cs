using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TourBooking.Data;
using TourBooking.Models;

namespace TourBooking.ViewModels
{
    public class AdminCustomerViewModel : BaseViewModel
    {
        private ObservableCollection<Customer> _allCustomers;
        private ObservableCollection<Customer> _filteredCustomers;
        private string _searchQuery;
        private int _totalCustomersCount;
        private decimal _averageRevenue;
        private int _vipCustomersCount;

        public int TotalCustomersCount { get => _totalCustomersCount; set { _totalCustomersCount = value; OnPropertyChanged(); } }
        public decimal AverageRevenue { get => _averageRevenue; set { _averageRevenue = value; OnPropertyChanged(); } }
        public int VipCustomersCount { get => _vipCustomersCount; set { _vipCustomersCount = value; OnPropertyChanged(); } }

        public ObservableCollection<Customer> FilteredCustomers
        {
            get => _filteredCustomers;
            set { _filteredCustomers = value; OnPropertyChanged(); }
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

        public ICommand AddCustomerCommand { get; }
        public ICommand ExportExcelCommand { get; }

        public AdminCustomerViewModel()
        {
            AddCustomerCommand = new RelayCommand<object>(obj => MessageBox.Show("Mở Form thêm khách hàng mới!", "Thông báo"));
            ExportExcelCommand = new RelayCommand<object>(obj => MessageBox.Show("Đang kết xuất dữ liệu khách hàng ra file Excel...", "Thông báo"));

            LoadDataFromDatabase();
        }

        private void LoadDataFromDatabase()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var customerList = context.Customers.ToList();

                    TotalCustomersCount = customerList.Count;
                    AverageRevenue = customerList.Count > 0 ? 934000 : 0;
                    VipCustomersCount = customerList.Count(c => c.FullName.Contains("VIP") || c.CustomerId % 3 == 0);

                    _allCustomers = new ObservableCollection<Customer>(customerList);
                    FilteredCustomers = new ObservableCollection<Customer>(customerList);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối bảng Khách hàng: {ex.Message}", "Lỗi Hệ Thống");
                _allCustomers = new ObservableCollection<Customer>();
                FilteredCustomers = new ObservableCollection<Customer>();
            }
        }

        private void ExecuteSearch()
        {
            if (_allCustomers == null) return;

            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                FilteredCustomers = new ObservableCollection<Customer>(_allCustomers);
                return;
            }

            var results = _allCustomers.Where(c =>
                (c.FullName != null && c.FullName.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0) ||
                (c.Email != null && c.Email.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0) ||
                (c.NationalId != null && c.NationalId.Contains(SearchQuery)) ||
                (c.Phone != null && c.Phone.Contains(SearchQuery)));

            FilteredCustomers = new ObservableCollection<Customer>(results.ToList());
        }
    }
}
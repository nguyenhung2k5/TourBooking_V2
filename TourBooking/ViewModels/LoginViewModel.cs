using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TourBooking.Data;
using TourBooking.Helpers;
using TourBooking.Services;

namespace TourBooking.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string _username;
        private string _password;
        private string _errorMessage;
        private bool _isBusy;

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(ExecuteLogin, _ => !IsBusy);
        }

        public event Action LoginSucceeded;

        public string Username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }

        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            private set { SetProperty(ref _errorMessage, value); }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            private set { SetProperty(ref _isBusy, value); }
        }

        public ICommand LoginCommand { get; private set; }

        private void ExecuteLogin(object parameter)
        {
            ErrorMessage = string.Empty;

            var passwordBox = parameter as PasswordBox;
            if (passwordBox != null)
            {
                Password = passwordBox.Password;
            }

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Vui long nhap day du tai khoan va mat khau.";
                MessageBox.Show("Vui lòng nhập đầy đủ tài khoản và mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IsBusy = true;

            try
            {
                string passwordHash = PasswordHasher.Hash(Password);

                using (var db = new AppDbContext())
                {
                    var username = Username.Trim();
                    var staff = db.Staffs.FirstOrDefault(s => s.Username == username
                                                            && (s.PasswordHash == passwordHash || s.PasswordHash == Password)
                                                            && s.IsActive);

                    if (staff == null)
                    {
                        ErrorMessage = "Tai khoan hoac mat khau khong chinh xac.";
                        MessageBox.Show("Tài khoản hoặc mật khẩu không chính xác!", "Đăng nhập thất bại", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    SessionService.CurrentStaff = staff;

                    var handler = LoginSucceeded;
                    if (handler != null)
                    {
                        handler();
                    }

                    var mainWindow = new MainWindow();
                    mainWindow.Show();

                    var currentWindow = parameter as Window;
                    if (currentWindow == null && passwordBox != null)
                    {
                        currentWindow = Window.GetWindow(passwordBox);
                    }

                    if (currentWindow != null)
                    {
                        currentWindow.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                MessageBox.Show("Lỗi kết nối cơ sở dữ liệu: " + ex.Message, "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}

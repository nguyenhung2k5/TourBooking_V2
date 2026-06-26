using System;
using System.Linq;
using System.Windows;
using TourBooking.Data;       // Thư mục chứa AppDbContext
using TourBooking.Helpers;    // Thư mục chứa PasswordHasher
using TourBooking.Models;     // Thư mục chứa Enum UserRole và Staff

namespace TourBooking.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string passwordRaw = txtPassword.Password.Trim();

            // 1. Kiểm tra không để trống ô nhập liệu
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(passwordRaw))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tài khoản và mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Mã hóa mật khẩu người dùng vừa gõ sang SHA256 để đối soát
            string passwordHash = PasswordHasher.Hash(passwordRaw);

            try
            {
                // 3. Kết nối Database kiểm tra tài khoản
                using (var db = new AppDbContext())
                {
                    var staff = db.Staffs.FirstOrDefault(s => s.Username == username && s.PasswordHash == passwordHash && s.IsActive);

                    if (staff != null)
                    {
                        // 4. Mở màn hình chính MainWindow
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();

                        // 5. Tắt màn hình đăng nhập hiện tại
                        this.Close();
                    }
                    else
                    {
                        // ĐĂNG NHẬP THẤT BẠI
                        MessageBox.Show("Tên đăng nhập hoặc mật khẩu không chính xác!", "Lỗi đăng nhập", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối cơ sở dữ liệu: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
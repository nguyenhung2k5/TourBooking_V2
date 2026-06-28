using System;
using System.Windows;
using TourBooking.Models;
using TourBooking.Services;

namespace TourBooking.Views.Dialogs
{
    public partial class StaffFormDialog : Window
    {
        public bool IsSuccess { get; private set; }
        private readonly Staff _staffToEdit;

        public StaffFormDialog(Staff staffToEdit = null)
        {
            InitializeComponent();
            _staffToEdit = staffToEdit;

            if (_staffToEdit != null)
            {
                txtHeaderTitle.Text = "Cập nhật thông tin Nhân viên";
                txtFullName.Text = _staffToEdit.FullName;
                txtStaffCode.Text = _staffToEdit.StaffCode;
                txtUsername.Text = _staffToEdit.Username;
                txtUsername.IsEnabled = false; // Không cho đổi username
                cboRole.SelectedIndex = _staffToEdit.Role == UserRole.Admin ? 1 : 0;
            }
            else
            {
                txtHeaderTitle.Text = "Thêm mới Nhân viên";
                txtStaffCode.Text = "NV" + new Random().Next(100, 999);
                txtPassword.Password = "123456";
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            UserRole role = cboRole.SelectedIndex == 1 ? UserRole.Admin : UserRole.Staff;

            try
            {
                var staffService = new StaffService();
                if (_staffToEdit != null)
                {
                    // Update existing staff
                    using (var context = new TourBooking.Data.AppDbContext())
                    {
                        var dbStaff = context.Staffs.Find(_staffToEdit.StaffId);
                        if (dbStaff != null)
                        {
                            dbStaff.FullName = fullName;
                            dbStaff.StaffCode = txtStaffCode.Text.Trim();
                            dbStaff.Role = role;
                            if (!string.IsNullOrWhiteSpace(password))
                            {
                                dbStaff.PasswordHash = TourBooking.Helpers.PasswordHasher.Hash(password);
                            }
                            context.SaveChanges();
                        }
                    }
                    MessageBox.Show("Cập nhật thông tin nhân viên thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    IsSuccess = true;
                    DialogResult = true;
                    Close();
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(password))
                    {
                        MessageBox.Show("Vui lòng nhập mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    var result = staffService.Register(fullName, username, password, role);
                    if (result.Success)
                    {
                        MessageBox.Show("Thêm mới nhân viên thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        IsSuccess = true;
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show(result.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hệ thống: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

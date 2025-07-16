using ProjectPRN212.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProjectPRN212
{
    /// <summary>
    /// Interaction logic for UserProfileControl.xaml
    /// </summary>
    public partial class UserProfileControl : UserControl
    {
        private readonly SafeDrivingCertificateDbContext _context;
        private readonly int _userId;
        private User _currentUser;

        public UserProfileControl(int userId)
        {
            InitializeComponent();
            _context = new SafeDrivingCertificateDbContext();
            _userId = userId;
            LoadUserData();
        }

        private void LoadUserData()
        {
            _currentUser = _context.Users.FirstOrDefault(u => u.UserId == _userId);
            if (_currentUser != null)
            {
                txtFullName.Text = _currentUser.FullName;
                txtEmail.Text = _currentUser.Email;
                txtPhone.Text = _currentUser.Phone;
                txtSchool.Text = _currentUser.School;
                txtClass.Text = _currentUser.Class;
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null) return;

            // Cập nhật thông tin cá nhân
            _currentUser.FullName = txtFullName.Text.Trim();
            _currentUser.Phone = txtPhone.Text.Trim();
            _currentUser.School = txtSchool.Text.Trim();
            _currentUser.Class = txtClass.Text.Trim();

            // Xử lý đổi mật khẩu nếu có
            var currentPassword = txtCurrentPassword.Password;
            var newPassword = txtNewPassword.Password;
            var confirmPassword = txtConfirmPassword.Password;

            if (!string.IsNullOrWhiteSpace(currentPassword) ||
                !string.IsNullOrWhiteSpace(newPassword) ||
                !string.IsNullOrWhiteSpace(confirmPassword))
            {
                if (_currentUser.Password != currentPassword)
                {
                    MessageBox.Show("Mật khẩu hiện tại không đúng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (newPassword != confirmPassword)
                {
                    MessageBox.Show("Mật khẩu mới không khớp.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (newPassword.Length < 6)
                {
                    MessageBox.Show("Mật khẩu mới phải có ít nhất 6 ký tự.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _currentUser.Password = newPassword;
            }

            _context.SaveChanges();
            MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

            // Clear password fields
            txtCurrentPassword.Password = "";
            txtNewPassword.Password = "";
            txtConfirmPassword.Password = "";
        }
    }
}

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
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
        }
        private void Label_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Bạn có muốn thoát add hay không!!!", "Exit", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                this.Close();
            }
        }
        SafeDrivingCertificateDbContext context = new SafeDrivingCertificateDbContext();
        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtPassword.Password) || string.IsNullOrWhiteSpace(txtConfimpass.Password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông  tin!!!", "Infor Error", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                return;
            }
            if (!txtPassword.Password.Equals(txtConfimpass.Password))
            {
                MessageBox.Show("Pass word và confim pass không giống nhau!!!", "Infor Error", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                return;
            }
            if (CheckeUser(txtEmail.Text))
            {
                MessageBox.Show("Tài khoản này đã tồn tại!!!", "Infor Error", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                return;
            }
            ValidateString validate = new ValidateString();
            if (validate.ValidateUsername(txtEmail.Text) && validate.ValidatePassWord(txtPassword.Password))
            {
                User user = new User();
                user.Email = txtEmail.Text;
                user.Password = txtPassword.Password;
                user.Role = "Student";
                user.FullName = txtFullname.Text;
                context.Users.Add(user);
                context.SaveChanges();
                MessageBox.Show("Bạn đã đăng kí thành công!!!", "Sign Success", MessageBoxButton.YesNo, MessageBoxImage.Information);
                Login register = new Login();
                register.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Tài khoản của bạn phải có ít nhất 8 ký tự và chữ bắt đầu không là số và mất khẩu phải trên 6 kí tự", "Infor Error", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                return;
            }
        }
        public bool CheckeUser(string userName)
        {
            var listUser = context.Users.ToList();
            foreach (var user in listUser)
            {
                if (user.Email == userName)
                {
                    return true;
                }
            }
            return false;
        }
        private void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            Login register = new Login();
            register.Show();
            this.Close();
        }

    }
}

using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectPRN212
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }
        SafeDrivingCertificateDbContext context = new SafeDrivingCertificateDbContext();
        private void Label_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Bạn có muốn thoát add hay không!!!", "Exit", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                this.Close();
            }
        }

        private void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtEmail.Text) || string.IsNullOrEmpty(txtPassword.Password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông  tin!!!", "Infor Error", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                return;
            }
            var user = context.Users.FirstOrDefault(user => user.Email == txtEmail.Text);
            if (user != null && user.Password == txtPassword.Password)
            {
                if (user.Role == "Student")
                {
                    UserHome home = new UserHome(user);
                    home.Show();
                    this.Close();
                }
               else if (user.Role == "Admin")
                {
                    AdminHome Adminhome = new AdminHome();
                    Adminhome.Show();
                    this.Close();
                }
                else
                {
                    TeacherHome teacherHome = new TeacherHome(user.UserId);
                    teacherHome.Show();
                    this.Close();

                }
            }
            else
            {
                MessageBox.Show("Mật khẩu hoặc tài khoản không chính xác vui lòng thử lại!!!", "Login Error", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            }
        }

        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            Register register = new Register();
            register.Show();
            this.Close();
        }
    }
}

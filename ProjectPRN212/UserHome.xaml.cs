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
    /// Interaction logic for UserHome.xaml
    /// </summary>
    public partial class UserHome : Window
    {
        private readonly User _currentUser;
        public UserHome(User user)
        {
            InitializeComponent();
            _currentUser = user;
        }
        private void BtnRegisterCourse_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new MemberWindow(_currentUser.UserId);
        }
        
         private void BtnNotifications_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new NotificationPage(_currentUser);
        }

        private void ButtonResult_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content= new ExamResultPage(_currentUser.UserId);
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UserProfileControl(_currentUser.UserId);

        }

        private void ButtonCert_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new CertificateListControl(_currentUser.UserId);
        }
    }
}

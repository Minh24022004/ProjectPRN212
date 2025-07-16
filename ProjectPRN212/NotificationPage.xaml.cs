using ProjectPRN212.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for NotificationPage.xaml
    /// </summary>
    public partial class NotificationPage : UserControl
    {
        private readonly SafeDrivingCertificateDbContext _context = new();
        private readonly User _currentUser;
        public NotificationPage(User user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadNotifications();
        }

        private void LoadNotifications()
        {
            List<Notification> notifications;

            if (_context.Notifications.Any(n => n.UserID != null))
            {
                notifications = _context.Notifications
                    .Where(n => n.UserID == _currentUser.UserId)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToList();
            }
            else
            {
                notifications = _context.Notifications
                    .OrderByDescending(n => n.CreatedAt)
                    .ToList();
            }

            lvNotifications.ItemsSource = notifications;

            txtEmpty.Visibility = notifications.Count == 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }
        private void MarkAllAsRead_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new SafeDrivingCertificateDbContext())
            {
                var unreadNotifications = context.Notifications.Where(n => !n.IsRead).ToList();

                foreach (var notification in unreadNotifications)
                {
                    notification.IsRead = true;
                }

                context.SaveChanges();
            }

            LoadNotifications(); // Hàm reload danh sách thông báo
            MessageBox.Show("Tất cả thông báo đã được đánh dấu là đã đọc.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void lvNotifications_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = lvNotifications.SelectedItem as Notification;
            if (selected != null && !selected.IsRead)
            {
                selected.IsRead = true;

                using (var context = new SafeDrivingCertificateDbContext())
                {
                    context.Notifications.Update(selected);
                    context.SaveChanges();
                }

                LoadNotifications(); // reload để cập nhật giao diện
            }
        }

    }

}

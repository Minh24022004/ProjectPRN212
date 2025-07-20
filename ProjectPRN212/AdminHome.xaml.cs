using Microsoft.EntityFrameworkCore;
using ProjectPRN212.Models;
using ProjectPRN212.ViewModels;
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
    /// Interaction logic for AdminHome.xaml
    /// </summary>
    public partial class AdminHome : Window
    {
        private readonly SafeDrivingCertificateDbContext _context;
        public AdminHome()
        {
            InitializeComponent();
            _context = new SafeDrivingCertificateDbContext();
        }
        private void BtnReport_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new SafeDrivingCertificateDbContext())
            {
                var data = (from c in context.Courses
                            let total = context.Registrations.Count(r => r.CourseId == c.CourseId)
                            let passed = (from r in context.Results
                                          join e1 in context.Exams on r.ExamId equals e1.ExamId
                                          where r.PassStatus == true && e1.CourseId == c.CourseId
                                          select r.UserId).Distinct().Count()
                            select new
                            {
                                CourseId = c.CourseId,
                                CourseName = c.CourseName,
                                TotalStudents = total,
                                PassedStudents = passed,
                                PassRate = total > 0 ? Math.Round((double)passed * 100 / total, 2) : 0
                            }).ToList();


                ReportListView.ItemsSource = data;
                ReportListView.Visibility = Visibility.Visible;
            }
        }

        private void BtnEditUser_Click(object sender, RoutedEventArgs e)
        {
            // Mở cửa sổ quản lý người dùng
            var userManager = new EditUserWindow();
            userManager.ShowDialog();
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            // Mở cửa sổ cài đặt
            var config = new PhieuDuyetChungChiView();
            config.ShowDialog();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); 
        }

        private void ButtonExams_Click(object sender, RoutedEventArgs e)
        {
            var exams= new ExamManagementView();
            exams.ShowDialog();
        }

        private void DetailButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is int courseId)
            {
                using (var context = new SafeDrivingCertificateDbContext())
                {
                   
                    var results = (from r in context.Results
                                   join e1 in context.Exams on r.ExamId equals e1.ExamId
                                   where e1.CourseId == courseId
                                   join u in context.Users on r.UserId equals u.UserId
                                   select new
                                   {
                                       u.FullName,
                                       u.Class,
                                       u.School,
                                       r.Score,
                                       Status = r.PassStatus ? "Đậu ✅" : "Trượt ❌"
                                   }).ToList();

                    // Hiển thị kết quả trong MessageBox (hoặc mở cửa sổ chi tiết)
                    string message = "Danh sách học sinh:\n";
                    foreach (var r in results)
                    {
                        message += $"{r.FullName} | {r.Class} | {r.School} | {r.Score} điểm | {r.Status}\n";
                    }

                    MessageBox.Show(message, "Chi tiết kết quả", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

    }
}

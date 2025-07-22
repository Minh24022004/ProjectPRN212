using Microsoft.EntityFrameworkCore;
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
    /// Interaction logic for MemberWindow.xaml
    /// </summary>
    public partial class MemberWindow : UserControl
    {
        private SafeDrivingCertificateDbContext _context;
        private int _userId;
        public MemberWindow(int userId)
        {
            InitializeComponent();
            _context = new SafeDrivingCertificateDbContext();
            _userId = userId;

            LoadCourses();
            LoadRegisteredCourses();
        }

        private void LoadCourses()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var courses = _context.Courses
                .Where(c => c.StartDate > today)
                .ToList();

            cbCourses.ItemsSource = courses;
            cbCourses.DisplayMemberPath = "CourseName";
            cbCourses.SelectedValuePath = "CourseId";
        }


        private void cbCourses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbCourses.SelectedValue != null)
            {
                int courseId = (int)cbCourses.SelectedValue;
                var today = DateOnly.FromDateTime(DateTime.Now);

                var exams = _context.Exams
                    .Where(ex => ex.CourseId == courseId && ex.Date > today)
                    .ToList();

                lvExams.ItemsSource = exams;
            }
        }
        private void LoadRegisteredCourses()
        {
            var registered = _context.Registrations
                .Where(r => r.UserId == _userId)
                .Include(r => r.Course)
                .ToList();

            lvRegisteredCourses.ItemsSource = registered;
        }


        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (cbCourses.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn khóa học.");
                return;
            }

            int courseId = (int)cbCourses.SelectedValue;

            // Kiểm tra nếu đã đăng ký
            bool isAlreadyRegistered = _context.Registrations
                .Any(r => r.UserId == _userId && r.CourseId == courseId);

            if (isAlreadyRegistered)
            {
                MessageBox.Show("Bạn đã đăng ký khóa học này rồi.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Tạo đăng ký mới
            var registration = new Registration
            {
                UserId = _userId,
                CourseId = courseId,
                Status = "Pending"
            };

            _context.Registrations.Add(registration);
            _context.SaveChanges();

            MessageBox.Show("Đăng ký khóa học thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

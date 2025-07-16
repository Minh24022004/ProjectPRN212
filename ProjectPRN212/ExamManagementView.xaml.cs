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
    /// Interaction logic for ExamManagementView.xaml
    /// </summary>
    public partial class ExamManagementView : Window
    {
        private SafeDrivingCertificateDbContext _context;

        public ExamManagementView()
        {
            InitializeComponent();
            _context = new SafeDrivingCertificateDbContext();
            LoadExamList();
        }

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    LoadExamList();
        //}

        private void LoadExamList()
        {
            var exams = _context.Exams
                .Include(e => e.Course)
                .ToList();
            ExamListView.ItemsSource = exams;
        }

        private void AddExam_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddEditExamWindow();
            if (dialog.ShowDialog() == true)
            {
                var newExam = dialog.Exam;

                
                _context.Exams.Add(newExam);
                _context.SaveChanges();

                var course = _context.Courses.FirstOrDefault(c => c.CourseId == newExam.CourseId);
                if (course != null)
                {
                    var registeredUsers = _context.Registrations
                        .Where(r => r.CourseId == course.CourseId)
                        .Select(r => r.UserId)
                        .ToList();

                    foreach (var userId in registeredUsers)
                    {
                        var notification = new Notification
                        {
                            Message = $"Khóa học {course.CourseName} đã được lên lịch thi vào ngày {newExam.Date:dd/MM/yyyy} tại phòng {newExam.Room}.",
                            CreatedAt = DateTime.Now,
                            UserID = userId
                        };
                        _context.Notifications.Add(notification);
                    }

                    _context.SaveChanges();
                }

                LoadExamList();
                MessageBox.Show("Thêm kỳ thi và gửi thông báo thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void EditExam_Click(object sender, RoutedEventArgs e)
        {
            if (ExamListView.SelectedItem is Exam selectedExam)
            {
                var dialog = new AddEditExamWindow(selectedExam);
                if (dialog.ShowDialog() == true)
                {
                    _context.Entry(selectedExam).State = EntityState.Modified;
                    _context.SaveChanges();
                    LoadExamList();
                }
            }
        }

        private void DeleteExam_Click(object sender, RoutedEventArgs e)
        {
            if (ExamListView.SelectedItem is Exam selectedExam)
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa kỳ thi này?", "Xác nhận",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    _context.Exams.Remove(selectedExam);
                    _context.SaveChanges();
                    LoadExamList();
                }
            }
        }
    }
}

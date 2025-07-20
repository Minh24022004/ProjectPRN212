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
using ProjectPRN212.ViewModels;

namespace ProjectPRN212
{
    /// <summary>
    /// Interaction logic for TeacherHome.xaml
    /// </summary>
    public partial class TeacherHome : Window
    {
        private Course selectedCourse;
        private readonly SafeDrivingCertificateDbContext _context;
        private readonly int _teacherId;

        public TeacherHome(int teacherId)
        {
            InitializeComponent();
            _context = new SafeDrivingCertificateDbContext();
            _teacherId = teacherId;

            LoadCourses();
            LoadPendingRegistrations();
        }

        private void LoadCourses()
        {
            var courses = _context.Courses
                            .Where(c => c.TeacherId == _teacherId)
                            .Include(c=> c.Registrations)
                            .ToList();

            lvMyCourses.ItemsSource = courses;
            cbCourses.ItemsSource = courses;
        }

        private void AddCourse_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCourseName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên khóa học.");
                return;
            }

            var course = new Course
            {
                CourseName = txtCourseName.Text.Trim(),
                StartDate = DateOnly.FromDateTime(dpStartDate.SelectedDate.Value),
                EndDate = DateOnly.FromDateTime(dpEndDate.SelectedDate.Value),
                TeacherId = _teacherId
            };

            _context.Courses.Add(course);
            _context.SaveChanges();

            LoadCourses();
            MessageBox.Show("Đã thêm khóa học mới!");
        }

        private void lvMyCourses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedCourse = lvMyCourses.SelectedItem as Course;
            if (selectedCourse != null)
            {
                txtCourseName.Text = selectedCourse.CourseName;
                dpStartDate.SelectedDate = selectedCourse.StartDate.ToDateTime(new TimeOnly(0, 0));
                dpEndDate.SelectedDate = selectedCourse.EndDate.ToDateTime(new TimeOnly(0, 0));
            }
        }

        private void EditCourse_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCourse == null)
            {
                MessageBox.Show("Vui lòng chọn một khóa học để chỉnh sửa.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCourseName.Text) || dpStartDate.SelectedDate == null || dpEndDate.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.");
                return;
            }

            selectedCourse.CourseName = txtCourseName.Text.Trim();
            selectedCourse.StartDate = DateOnly.FromDateTime(dpStartDate.SelectedDate.Value);
            selectedCourse.EndDate = DateOnly.FromDateTime(dpEndDate.SelectedDate.Value);

            _context.Courses.Update(selectedCourse);
            _context.SaveChanges();

            MessageBox.Show("Cập nhật khóa học thành công!");
            ClearFields();
            LoadCourses();
        }
        private void ClearFields()
        {
            txtCourseName.Text = "";
            dpStartDate.SelectedDate = null;
            dpEndDate.SelectedDate = null;
            selectedCourse = null;
        }
        private void cbCourses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbCourses.SelectedValue is int courseId)
            {
                var registrations = _context.Registrations
                    .Include(r => r.User)
                    .Where(r => r.CourseId == courseId && r.Status == "Approved")
                    .ToList();

                var results = _context.Results
                    .Include(r => r.Exam)
                    .Where(r => r.Exam.CourseId == courseId)
                    .ToList();

                var studentScores = registrations.Select(r => new StudentScore
                {
                    User =r.User,
                    Score = results.FirstOrDefault(x => x.UserId == r.UserId)?.Score ?? 0
                }).ToList();

                lvStudentsToGrade.ItemsSource = studentScores;
            }
        }

        private void SaveScores_Click(object sender, RoutedEventArgs e)
        {
            foreach (dynamic item in lvStudentsToGrade.Items)
            {
                int userId = item.User.UserId;
                decimal score = Convert.ToDecimal(item.Score);
                int courseId = (int)cbCourses.SelectedValue;

                var exam = _context.Exams.FirstOrDefault(x => x.CourseId == courseId);
                bool passStatus = score >= 80;
                if (exam == null) continue;

                var result = _context.Results.FirstOrDefault(r => r.UserId == userId && r.ExamId == exam.ExamId);
                if (result == null)
                {
                    result = new Result
                    {
                        UserId = userId,
                        ExamId = exam.ExamId,
                        Score = score,
                        PassStatus = passStatus
                    };
                    _context.Results.Add(result);
                    var notification = new Notification
                    {
                        Message = $"Bạn đã được chấm điểm {score} cho kỳ thi khóa học {exam.Course.CourseName}.",
                        CreatedAt = DateTime.Now,
                        UserID = userId
                    };
                    _context.Notifications.Add(notification);
                }
                else
                {
                    result.Score = score;
                }
            }

            _context.SaveChanges();
            MessageBox.Show("Đã lưu điểm!");
        }

        private void LoadPendingRegistrations()
        {
            var pending = _context.Registrations
                .Include(r => r.User)
                .Include(r => r.Course)
                .Where(r => r.Course.TeacherId == _teacherId && r.Status == "Pending")
                .ToList();

            lvPendingRegistrations.ItemsSource = pending;
        }

        private void Approve_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var registration = btn?.Tag as Registration;

            if (registration != null)
            {
                registration.Status = "Approved";
                _context.SaveChanges();
                LoadPendingRegistrations();
                MessageBox.Show("Đã duyệt học sinh.");
                var notification = new Notification
                {
                    Message = $"Bạn đã được duyệt cho khóa học {registration.Course.CourseName}.",
                    CreatedAt = DateTime.Now,
                    UserID = registration.UserId
                };
                _context.Notifications.Add(notification);
                _context.SaveChanges();
            }
        }

    }
}

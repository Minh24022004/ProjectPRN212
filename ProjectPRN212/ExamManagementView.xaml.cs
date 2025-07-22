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
    public partial class ExamManagementView : Window, IDisposable
    {
        private SafeDrivingCertificateDbContext _context;

        public ExamManagementView()
        {
            InitializeComponent();
            _context = new SafeDrivingCertificateDbContext();
            LoadExamList();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        private void LoadExamList()
        {
            try
            {
                var exams = _context.Exams
                    .Include(e => e.Course)
                    .ToList();
                ExamListView.ItemsSource = exams;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách kỳ thi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddExam_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddEditExamWindow();
            if (dialog.ShowDialog() == true)
            {
                var newExam = dialog.Exam;

                if (newExam == null)
                {
                    MessageBox.Show("Dữ liệu kỳ thi không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Validate Course
                var course = _context.Courses.FirstOrDefault(c => c.CourseId == newExam.CourseId);
                if (course == null)
                {
                    MessageBox.Show("Khóa học không tồn tại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Kiểm tra ngày thi phải lớn hơn ngày kết thúc khóa học
                if (newExam.Date <= course.EndDate)
                {
                    MessageBox.Show("Ngày thi phải lớn hơn ngày kết thúc của khóa học.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validate ngày thi
                var today = DateOnly.FromDateTime(DateTime.Today);
                if (newExam.Date < today)
                {
                    MessageBox.Show("Ngày thi không được nhỏ hơn ngày hiện tại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validate phòng thi
                if (string.IsNullOrWhiteSpace(newExam.Room))
                {
                    MessageBox.Show("Phòng thi không được để trống.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    _context.Exams.Add(newExam);

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
                    LoadExamList();
                    MessageBox.Show("Thêm kỳ thi và gửi thông báo thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi thêm kỳ thi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void EditExam_Click(object sender, RoutedEventArgs e)
        {
            if (ExamListView.SelectedItem is Exam selectedExam)
            {
                var dialog = new AddEditExamWindow(selectedExam);
                if (dialog.ShowDialog() == true)
                {
                    var updatedExam = dialog.Exam;

                    // Validate khóa học
                    var course = _context.Courses.FirstOrDefault(c => c.CourseId == updatedExam.CourseId);
                    if (course == null)
                    {
                        MessageBox.Show("Khóa học không tồn tại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var today = DateOnly.FromDateTime(DateTime.Today);
                    if (updatedExam.Date < today)
                    {
                        MessageBox.Show("Ngày thi không được nhỏ hơn ngày hiện tại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // ❗ Kiểm tra ngày thi phải sau ngày kết thúc khóa học
                    if (updatedExam.Date <= course.EndDate)
                    {
                        MessageBox.Show("Ngày thi phải sau ngày kết thúc của khóa học.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(updatedExam.Room))
                    {
                        MessageBox.Show("Phòng thi không được để trống.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    try
                    {
                        selectedExam.CourseId = updatedExam.CourseId;
                        selectedExam.Date = updatedExam.Date;
                        selectedExam.Room = updatedExam.Room;

                        _context.Entry(selectedExam).State = EntityState.Modified;
                        _context.SaveChanges();

                        LoadExamList();
                        MessageBox.Show("Cập nhật kỳ thi thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi cập nhật kỳ thi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một kỳ thi để chỉnh sửa.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void DeleteExam_Click(object sender, RoutedEventArgs e)
        {
            if (ExamListView.SelectedItem is Exam selectedExam)
            {
                if (_context.Results.Any(r => r.ExamId == selectedExam.ExamId))
                {
                    MessageBox.Show("Không thể xóa kỳ thi vì đã có kết quả liên quan.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (MessageBox.Show("Bạn có chắc chắn muốn xóa kỳ thi này?", "Xác nhận",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _context.Exams.Remove(selectedExam);
                        _context.SaveChanges();
                        LoadExamList();
                        MessageBox.Show("Xóa kỳ thi thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi xóa kỳ thi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {   
                MessageBox.Show("Vui lòng chọn một kỳ thi để xóa.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
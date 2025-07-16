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
    /// Interaction logic for PhieuDuyetChungChiView.xaml
    /// </summary>
    public partial class PhieuDuyetChungChiView : Window
    {
        private SafeDrivingCertificateDbContext _context;

        public PhieuDuyetChungChiView()
        {
            InitializeComponent();
            _context = new SafeDrivingCertificateDbContext();
            LoadFilters();
            LoadPassedStudents();
        }

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    LoadPassedStudents();
        //}
        private void LoadFilters()
        {
            // Lấy danh sách lớp duy nhất
            var classList = _context.Users
                .Where(u => u.Role == "Student" && u.Class != null)
                .Select(u => u.Class)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            ClassFilterComboBox.ItemsSource = classList;
            ClassFilterComboBox.SelectedIndex = -1;

            // Lấy danh sách khóa học
            var courseList = _context.Courses
                .Select(c => new { c.CourseId, c.CourseName })
                .ToList();

            CourseFilterComboBox.ItemsSource = courseList;
            CourseFilterComboBox.DisplayMemberPath = "CourseName";
            CourseFilterComboBox.SelectedValuePath = "CourseId";
            CourseFilterComboBox.SelectedIndex = -1;
        }
        private void LoadPassedStudents()
        {
            var query = from r in _context.Results
                        where r.PassStatus
                           && !_context.Certificates.Any(c => c.UserId == r.UserId)
                        join u in _context.Users on r.UserId equals u.UserId
                        join e in _context.Exams on r.ExamId equals e.ExamId
                        join c in _context.Courses on e.CourseId equals c.CourseId
                        select new PassedStudentViewModel
                        {
                            UserId = u.UserId,
                            FullName = u.FullName,
                            Email = u.Email,
                            Class = u.Class ?? "",
                            Score = r.Score,
                            CourseName = c.CourseName
                        };

            // Áp dụng lọc nếu có chọn
            string selectedClass = ClassFilterComboBox.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectedClass))
            {
                query = query.Where(p => p.Class == selectedClass);
            }

            if (CourseFilterComboBox.SelectedValue is int selectedCourseId)
            {
                query = query.Where(p => p.CourseName == _context.Courses
                    .FirstOrDefault(c => c.CourseId == selectedCourseId)!.CourseName);
            }

            PassedStudentListView.ItemsSource = query.ToList();
        }


        private void ApproveCertificates_Click(object sender, RoutedEventArgs e)
        {
            var selected = PassedStudentListView.SelectedItems.Cast<PassedStudentViewModel>().ToList();

            if (selected.Count == 0)
            {
                MessageBox.Show("Hãy chọn ít nhất một học sinh để phê duyệt.");
                return;
            }

            foreach (var student in selected)
            {
                string certCode = $"CERT-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";

                var cert = new Certificate
                {
                    UserId = student.UserId,
                    IssuedDate = DateOnly.FromDateTime(DateTime.Today),
                    ExpirationDate = DateOnly.FromDateTime(DateTime.Today.AddYears(3)),
                    CertificateCode = certCode
                };


                _context.Certificates.Add(cert);

                // (Tuỳ chọn) Thêm thông báo
                
                _context.Notifications.Add(new Notification
                {
                    UserID = student.UserId,
                    Message = $"Bạn đã được cấp chứng chỉ số {certCode}",
                    CreatedAt = DateTime.Now
                });
                
            }

            _context.SaveChanges();
            MessageBox.Show("Phê duyệt thành công!");
            LoadPassedStudents();
        }

        private void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            LoadPassedStudents();
        }
        private void ClearFilter_Click(object sender, RoutedEventArgs e)
        {
            ClassFilterComboBox.SelectedIndex = -1;
            CourseFilterComboBox.SelectedIndex = -1;
            LoadPassedStudents(); 
        }
    }
}

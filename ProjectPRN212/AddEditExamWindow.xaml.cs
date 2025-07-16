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
    /// Interaction logic for AddEditExamWindow.xaml
    /// </summary>
    public partial class AddEditExamWindow : Window
    {
        private SafeDrivingCertificateDbContext _context;
        public Exam Exam { get; private set; }

        public AddEditExamWindow(Exam? existingExam = null)
        {
            InitializeComponent();
            _context = new SafeDrivingCertificateDbContext();

            var courses = _context.Courses.ToList();
            CourseComboBox.ItemsSource = courses;

            if (existingExam != null)
            {
                Exam = existingExam;
                CourseComboBox.SelectedValue = Exam.CourseId;
                DatePicker.SelectedDate = Exam.Date.ToDateTime(new TimeOnly(0, 0));
                RoomTextBox.Text = Exam.Room;
            }
            else
            {
                Exam = new Exam();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (CourseComboBox.SelectedValue == null || DatePicker.SelectedDate == null || string.IsNullOrWhiteSpace(RoomTextBox.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.");
                return;
            }

            Exam.CourseId = (int)CourseComboBox.SelectedValue;
            Exam.Date = DateOnly.FromDateTime(DatePicker.SelectedDate.Value);
            Exam.Room = RoomTextBox.Text.Trim();

            this.DialogResult = true;
            this.Close();
        }
    }
}

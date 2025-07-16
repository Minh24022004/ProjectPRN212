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
    /// Interaction logic for ExamResultPage.xaml
    /// </summary>
    public partial class ExamResultPage : UserControl
    {
        private SafeDrivingCertificateDbContext _context;
        private int _userId;

        public ExamResultPage(int userId)
        {
            InitializeComponent();
            _context = new SafeDrivingCertificateDbContext();
            _userId = userId;
            LoadResults();
        }

        private void LoadResults()
        {
            var results = _context.Results
                .Where(r => r.UserId == _userId)
                .Select(r => new
                {
                    CourseName = r.Exam.Course.CourseName,
                    ExamDate = r.Exam.Date.ToString("dd/MM/yyyy"),
                    Room = r.Exam.Room,
                    Score = r.Score,
                    Status = r.PassStatus == true ? "Đậu" : "Trượt"
                })
                .ToList();

            lvResults.ItemsSource = results;
            txtEmpty.Visibility = results.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}


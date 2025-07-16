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
    /// Interaction logic for CertificateListControl.xaml
    /// </summary>
    public partial class CertificateListControl : UserControl
    {
        private readonly SafeDrivingCertificateDbContext _context;
        private readonly int _userId;

        public CertificateListControl(int userId)
        {
            InitializeComponent();
            _context = new SafeDrivingCertificateDbContext();
            _userId = userId;
            LoadCertificates();
        }

        private void LoadCertificates()
        {
            var certificates = _context.Certificates
      .Where(c => c.UserId == _userId)
      .Select(c => new
      {
          c.CertificateCode,
          CourseName = _context.Registrations
              .Where(r => r.UserId == c.UserId)
              .OrderByDescending(r => r.RegistrationId)
              .Select(r => r.Course.CourseName)
              .FirstOrDefault(),
          IssueDate = c.IssuedDate.ToShortDateString()
      })
      .ToList();


            lvCertificates.ItemsSource = certificates;

            txtEmpty.Visibility = certificates.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}

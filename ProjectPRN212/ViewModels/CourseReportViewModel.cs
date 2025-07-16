using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPRN212.ViewModels
{
    public class CourseReportViewModel
    {
        public string CourseName { get; set; }
        public int TotalStudents { get; set; }
        public int PassedStudents { get; set; }
        public double PassRate { get; set; }
    }
}

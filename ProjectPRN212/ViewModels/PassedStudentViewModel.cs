using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPRN212.ViewModels
{
    public class PassedStudentViewModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Class { get; set; } = "";
        public decimal Score { get; set; }
        public string CourseName { get; set; } = "";
        public int CourseId { get; set; } = 0;
    }

}

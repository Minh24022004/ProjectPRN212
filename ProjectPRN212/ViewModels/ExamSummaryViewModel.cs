using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPRN212.ViewModels
{
    public class ExamSummaryViewModel
    {
        public int ExamId { get; set; }
        public string CourseName { get; set; } = "";
        public DateOnly Date { get; set; }
        public string Room { get; set; } = "";
        public int CourseId { get; set; }  
    }
}

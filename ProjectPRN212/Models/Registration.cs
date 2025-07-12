using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPRN212.Models
{
    public class Registration
    {
        public int RegistrationID { get; set; }
        public int UserID { get; set; }
        public int CourseID { get; set; }
        public string Status { get; set; } // Pending, Approved, Rejected
        public string Comments { get; set; }
        public User User { get; set; }
        public Course Course { get; set; }
    }
}

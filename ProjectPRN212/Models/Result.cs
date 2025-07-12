using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPRN212
{
    public class Result
    {
        public int ResultID { get; set; }
        public int ExamID { get; set; }
        public int UserID { get; set; }
        public decimal Score { get; set; }
        public bool PassStatus { get; set; }
        public Exam Exam { get; set; }
        public User User { get; set; }
    }
}

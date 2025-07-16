using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProjectPRN212
{
    class ValidateString
    {
        public bool ValidateUsername(string input)
        {
            bool check = true;
            char input1 = input[0];
            if (input.Length < 8 || char.IsDigit(input1))
            {
                check = false;
            }
            return check;
        }
        public bool ValidatePassWord(string input)
        {
            bool check = true;
            if (input.Length < 6)
            {
                check = false;
            }
            return check;
        }
        public bool ValidateQuestion(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }
            return true;
        }
    }
}

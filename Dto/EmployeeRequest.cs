using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogPlgTest.Dto
{
    public class EmployeeRequest
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Lastname { get; set; }

        public string Department { get; set; }

        public string CompName { get; set; }
        public string WindowName { get; set; }

        public string Status { get; set; }
    }
}

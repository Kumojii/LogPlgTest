using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogPlgTest.Dto
{
    public class EmployeeResult
    {
        public bool Allowed { get; set; }
        public bool NeedRegistr { get; set; }
        public bool NeedUpdate { get; set; }
        public string Message { get; set; }
        public uint EmployeeId { get; set; }
    }
}

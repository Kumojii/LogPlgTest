using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogPlgTest.Models
{
    public class VerifyResult
    {
        public bool EmpAllowed { get; set; }
        public bool EmpNeedRegistr { get; set; }
        public bool EmpNeedUpdate { get; set; }
        public bool PlgAllowed { get; set; }
        public bool HasAccess { get; set; }

        public bool Result { get; set; }
        public string Message { get; set; }

    }
}
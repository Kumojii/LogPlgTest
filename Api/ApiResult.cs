using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogPlgTest.Api
{
    public class ApiResult<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string Error { get; set; }
        public Exception Exception { get; set; }
    }

    public class ApiResult
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public Exception Exception { get; set; }
    }

}

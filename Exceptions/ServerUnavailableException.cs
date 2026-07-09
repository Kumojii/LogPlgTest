using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogPlgTest.Exceptions
{
    public class ServerUnavailableException : Exception
    {
        public ServerUnavailableException()
        {
        }

        public ServerUnavailableException(string message)
            : base(message)
        {
        }

        public ServerUnavailableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

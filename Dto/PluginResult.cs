using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogPlgTest.Dto
{
    public class PluginResult
    {
        public bool Allowed { get; set; }
        public bool HasAccess { get; set; }
        public bool IsLatest { get; set; }
        public string Message { get; set; }
        public uint PluginId { get; set; }
    }
}

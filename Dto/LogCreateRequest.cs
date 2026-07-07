using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogPlgTest.Dto
{
    public class LogCreateRequest
    {
        public string PluginName { get; set; }
        public string ButtonName { get; set; }
        public string Version { get; set; }

        public string UserName { get; set; }
        public string MachineName { get; set; }

        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTimeOffset Date { get; set; } = DateTime.Now;
        public uint Elements { get; set; }
        public string Message { get; set; } = string.Empty;
        public string FullMessage { get; set; } = string.Empty;
        public string ErrorPath { get; set; } = string.Empty;
        public string ErrorDetails { get; set; } = string.Empty;
        public string Result { get; set; }

        public TimeSpan InterfaceTime { get; set; } = TimeSpan.Zero;
        public TimeSpan WorkTime { get; set; } = TimeSpan.Zero;
        public TimeSpan InformationTime { get; set; } = TimeSpan.Zero;
    }
}
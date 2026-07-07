using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogPlgTest.Enums
{
    public enum LogResult
    {
        /// <summary>
        /// Неуспешно
        /// </summary>
        Failed = 1,
        /// <summary>
        /// Успешно
        /// </summary>
        Succeeded,
        /// <summary>
        /// Отмена
        /// </summary>
        Cancelled
    }
}

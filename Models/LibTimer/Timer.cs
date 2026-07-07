using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogPlgTest.Models.LibTimer
{
    public enum Timer
    {
        /// <summary>
        /// Время взаимодействия с интерфейсом
        /// </summary>
        Interface,

        /// <summary>
        /// Время работы логики 
        /// </summary>
        Work,

        /// <summary>
        /// Время уведомлений для модальных messagebox и т.п.
        /// </summary>
        Notification
    }
}

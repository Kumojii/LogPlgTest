using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogPlgTest.Models.LibTimer
{
    public static class PlgTimers
    {
        // REVIEW: Точно public ?
        public static Stopwatch _uiTimer = new Stopwatch();
        public static Stopwatch _workTimer = new Stopwatch();
        public static Stopwatch _notifyTimer = new Stopwatch();

        public static TimeSpan InterfaceTime { get; set; } = TimeSpan.Zero;
        public static TimeSpan WorkTime { get; set; } = TimeSpan.Zero;
        public static TimeSpan NotifyTime { get; set; } = TimeSpan.Zero;


        public static void StartTimer(Timer timer)
        {
            _uiTimer.Stop();
            _workTimer.Stop();
            _notifyTimer.Stop();

            if (timer == Timer.Interface)
                _uiTimer.Start();
            else if (timer == Timer.Work)
                _workTimer.Start();
            else if (timer == Timer.Notification)
                _notifyTimer.Start();
        }

        public static void StopTimers()
        {
            _uiTimer.Stop();
            _workTimer.Stop();
            _notifyTimer.Stop();
            InterfaceTime = _uiTimer.Elapsed;
            WorkTime = _workTimer.Elapsed;
            NotifyTime = _notifyTimer.Elapsed;
        }

        public static void RefreshTimers()
        {
            _uiTimer.Reset();
            _workTimer.Reset();
            _notifyTimer.Reset();
        }
    }
}

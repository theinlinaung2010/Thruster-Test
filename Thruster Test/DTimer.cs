using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Thruster_Test
{
    class DTimer
    {
        private DispatcherTimer timer;
        public event Action<int> TimerTick;

        private int _timesCalled = 0;

        public DTimer(int PeriodInMilliSeconds)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(PeriodInMilliSeconds);
            timer.Tick += timer_Task;
        }

        public void Start()
        {
            _timesCalled = 0;
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        private void timer_Task(object sender, EventArgs e)
        {
            _timesCalled++;
            TimerTick(_timesCalled);
            //Console.WriteLine(_timesCalled);          
        }
    }
}

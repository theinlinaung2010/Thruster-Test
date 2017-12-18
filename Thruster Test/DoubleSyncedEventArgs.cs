using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thruster_Test
{
    class DoubleSyncedEventArgs: EventArgs
    {
        private readonly double value;
        private readonly uint time;

        public DoubleSyncedEventArgs(double _value, uint _time)
        {
            value = _value;
            time = _time;
        }

        public double GetValue
        {
            get { return value; }
        }

        public uint GetTime
        {
            get { return time; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jint;

namespace Fox.Core
{
    class SleepConstraint : IConstraint
    {
        int sleepTime;

        public SleepConstraint(int sleepTime)
        {
            this.sleepTime = sleepTime;
        }

        public void Check()
        {
            Thread.Sleep(sleepTime);
        }

        public void Reset()
        {
        }
    }
}

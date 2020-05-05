using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RFCProtocolTesting
{
    class ConnectionCounter
    {
        private int totalCounter = 0;
        private int currentCounter = 0;
        public int IncrementTotalCounter() { return Interlocked.Increment(ref totalCounter); }
        public int IncrementCurrentCounter() { return Interlocked.Increment(ref currentCounter); }
        public int ResetCurrentCounter() { return Interlocked.Exchange(ref currentCounter, 0); }
    }
}

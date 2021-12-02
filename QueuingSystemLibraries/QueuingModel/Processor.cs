using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using QueuingSystemLibraries.Common;
using QueuingSystemLibraries.Other;

namespace QueuingSystemLibraries.QueuingModel
{
    
    
    public sealed class Processor: CommonProcessor
    {
        private Time _currentTime;
        private Queue _queue;
        private Client _currentClient;

        public Processor(ulong id, string title, TypeProcessor type, Queue queue)
        {
            if (title is null || queue is null)
            {
                throw new NullReferenceException();
            }
            InitCommonTitle(id, title);
            Type = type;

        }

        public void Start()
        {
            while (true)
            {
                
            }
        }
        


        public void TickTimerHandler(object sender, TimeEventArgs e)
        {
            Interlocked.Exchange(ref _currentTime, e.CurrentTime);
        }
    }
}
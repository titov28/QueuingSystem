using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using QueuingSystemLibraries.Common;

namespace QueuingSystemLibraries.QueuingModel
{
    public sealed class Queue: CommonQueue
    {
        private readonly object _lockObject; 
        

        public Queue(ulong id, string title, int maxSize):base(maxSize)
        {
            InitCommonTitle(id, title);
            _lockObject = new object();
        }

        public bool TryEnqueue(Client cl)
        {
            bool locked = false;

            if ((NumberClient + 1) > MaxSize)
            {
                return false;
            }
            
            try
            {
                Monitor.Enter(_lockObject, ref locked);
                AddingClient();
                Clients.Add(cl);
                cl.EnteredInQueue();
                NumberClient++;
                NonEmpty();
                return true;
            }
            finally
            {
                if (locked)
                {
                    Monitor.Exit(_lockObject);
                }
            }
        }

        public bool TryDequeue(TypeProcessor tp, ref Client cl)
        {
            bool locked = false;

            if (NumberClient == 0)
            {
                Empty();
                return false;
            }
            
            try
            {
                Monitor.Enter(_lockObject, ref locked);
                RetrievingClient();
                int clientPosition = TryFind(tp); 
                if( clientPosition == -1) return false;
                cl = Clients[clientPosition];
                cl.WentOutQueue();
                Clients.RemoveAt(clientPosition);
                NumberClient--;
                if (NumberClient == 0)
                {
                    Empty();
                }
                else
                {
                    NonEmpty();
                }
                return true;
            }
            finally
            {
                if(locked) Monitor.Exit(_lockObject);
            }
            
        }
        
        private int TryFind(TypeProcessor tp)
        {
            int clientPosition = -1;
            for (int i = 0; i < Clients.Count; i++)
            {
                if (Clients[i].CurrentTypeProcessor == tp)
                {
                    clientPosition = i;
                    break;
                }
            }

            return clientPosition;
        }
        
        public bool IsFull()
        {
            return NumberClient == MaxSize;
        }

        public bool IsEmpty()
        {
            return NumberClient == 0;
        }
    }
    
    
}
using System;
using System.Collections;
using System.Collections.Generic;
using QueuingSystemLibraries.Common;
using QueuingSystemLibraries.Other;

namespace QueuingSystemLibraries.QueuingModel
{
    public sealed class Client: CommonClient
    {
        public Time StartGoingInSystem;
        public Request[] Requests { get; private set; }

        public Client(ulong id, string title, TypeProcessor processor, Request[] requests, Time startTime)
        {
            if (requests is null || startTime is null || title is null)
            {
                throw new NullReferenceException();
            }

            StartGoingInSystem = startTime;
            InitCommonTitle(id, title);
            CurrentTypeProcessor = processor;
            Requests = requests;
        }

        public bool IsProcessed()
        {
            bool flag = true;
            for (int i = 0; i < Requests.Length; i++)
            {
                if (Requests[i].Status == RequestStatus.NonProcessed)
                {
                    flag = false;
                }
            }

            return flag;
        }
        
    }
   
}
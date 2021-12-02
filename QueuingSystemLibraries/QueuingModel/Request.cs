using System;
using QueuingSystemLibraries.Common;
using QueuingSystemLibraries.Other;

namespace QueuingSystemLibraries.QueuingModel
{
    
    public sealed class Request: CommonRequest
    {
        public Time ProcessingTime { get; private set; }
        public Time DeviationTime { get; private set; }
        
        public Request(ulong id, string title, TypeRequest type, Time processingTime, Time deviationTime)
        {
            if (title == null || processingTime == null || deviationTime == null)
            {
                throw new NullReferenceException();
            }
            
            InitCommonTitle(id, title);

            Type = type;
            ProcessingTime = processingTime;
            DeviationTime = deviationTime;

        }
    }
}
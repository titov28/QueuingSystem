using System;
using QueuingSystemLibraries.QueuingModel;
using QueuingSystemLibraries.Other;

namespace QueuingSystemLibraries.Common
{
    public enum ClientStatus
    {
        Created,
        Rejection,
        EnteredInSystem,
        EnteredInQueue,
        WentOutFromQueue,
        Processing,
        PartiallyProcessed,
        Processed
    }
    public abstract class CommonClient: CommonTitle
    {
        public ClientStatus Status { get; protected set; } = ClientStatus.Created;
        public TypeProcessor CurrentTypeProcessor { get; protected set; }
        
    #region StatisticTime
        public Time EnteredInQueueTime { get; protected set; }
        public Time WentOutFromQueueTime { get; protected set; }
        public Time SummaryInQueueTime { get; protected set; }
        
    #endregion

    #region ChangeStatusMethods

        public void Rejection()
        {
            Status = ClientStatus.Rejection;
        }

        public void EnteredInSystem()
        {
            Status = ClientStatus.EnteredInSystem;
        }

        public void EnteredInQueue()
        {
            EnteredInQueueTime = QSTimer.GetCurrentTime();
            Status = ClientStatus.EnteredInQueue;
        }

        public void WentOutQueue()
        {
            WentOutFromQueueTime = QSTimer.GetCurrentTime();
            SummaryInQueueTime += WentOutFromQueueTime - EnteredInQueueTime;
            Status = ClientStatus.WentOutFromQueue;
        }

        public void ProcessingStart()
        {
            Status = ClientStatus.Processing;
        }

        public void PartiallyProcessed()
        {
            Status = ClientStatus.PartiallyProcessed;
        }

        public void ProcessingFinish()
        {
            Status = ClientStatus.Processed;
        }
    #endregion

    
    
    }
}
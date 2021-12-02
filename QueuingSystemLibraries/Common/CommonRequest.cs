using System;
using System.Collections.Concurrent;
using QueuingSystemLibraries.Other;

namespace QueuingSystemLibraries.Common
{
    public enum TypeRequest
    {
        XCHG = 0,
        CARD = 1,
        CRED = 2,
        ACNT = 3
    }

    public enum RequestStatus
    {
        NonProcessed,
        Processed,
        Processing,
        Interrupted
    }
    
    public abstract class CommonRequest : CommonTitle
    {
        private bool _interrupted = false;
        public RequestStatus Status { get; protected set; } = RequestStatus.NonProcessed;

        public TypeRequest Type { get; protected set; }
        
    #region StatisticTime
        public Time ProcessingStartTime { get; protected set; }
        public Time ProcessingFinishTime { get; protected set; }
        public Time ProcessingInterruptionTime { get; protected set; }
        public Time ProcessingResumptionTime { get; protected set; }
        
    #endregion

        public bool IsInterrupted()
        {
            return _interrupted;
        }
        
    #region ChangeStatusMethods

        public void ProcessingStart()
        {
            ProcessingStartTime = QSTimer.GetCurrentTime();
            Status = RequestStatus.Processing;
        }

        public void ProcessingFinish()
        {
            ProcessingFinishTime = QSTimer.GetCurrentTime();
            Status = RequestStatus.Processed;
        }

        public void ProcessingInterruption()
        {
            ProcessingInterruptionTime = QSTimer.GetCurrentTime();
            _interrupted = true;
            Status = RequestStatus.Interrupted;
        }

        public void ProcessingResumption()
        {
            ProcessingResumptionTime = QSTimer.GetCurrentTime();
            Status = RequestStatus.Processing;
        }

    #endregion

    }
}
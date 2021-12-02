using System;
using System.Collections.Generic;
using System.Threading;
using QueuingSystemLibraries.QueuingModel;

namespace QueuingSystemLibraries.Common
{
    public enum QueueStatus
    {
        Empty,
        NonEmpty,
        Retrieving,
        Adding
    }
    
    public abstract class CommonQueue : CommonTitle
    {
        public int NumberClient { get; protected set; }
        public int MaxSize { get; protected set; }

        protected List<Client> Clients;
        public CommonQueue(int maxSize)
        {
            if (maxSize < 0)
            {
                maxSize = 10;
            }
            
            NumberClient = 0;
            MaxSize = maxSize;
            Clients = new List<Client>(maxSize);
        }
        public QueueStatus Status { get; protected set; } = QueueStatus.Empty;
        
    #region ChangeStatusMethods

        public void RetrievingClient()
        {
            Status = QueueStatus.Retrieving;
        }

        public void AddingClient()
        {
            Status = QueueStatus.Adding;
        }

        public void Empty()
        {
            Status = QueueStatus.Empty;
        }

        public void NonEmpty()
        {
            Status = QueueStatus.NonEmpty;
        }
        
        
    #endregion
    }
}
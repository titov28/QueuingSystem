namespace QueuingSystemLibraries.Common
{
    public enum TypeProcessor
    {
        Exchange,
        Operating
    }

    public enum ProcessorStatus
    {
        Ready, 
        Processing,
        Interrupted,
        FindingClient
    }
    
    public abstract class CommonProcessor: CommonTitle
    {
        public TypeProcessor Type { get; protected set; }
        public ProcessorStatus Status { get; protected set; } = ProcessorStatus.Ready;



    #region ChangeStatusMethods

        public void Ready()
        {
            Status = ProcessorStatus.Ready;
        }

        public void Processing()
        {
            Status = ProcessorStatus.Processing;
        }

        public void Interrupted()
        {
            Status = ProcessorStatus.Interrupted;
        }

        public void FindingClient()
        {
            Status = ProcessorStatus.FindingClient;
        }

    

    #endregion
    }
}
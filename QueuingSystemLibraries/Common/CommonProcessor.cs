namespace QueuingSystemLibraries.Common
{
    public enum TypeProcessor
    {
        Exchange,
        Operating
    }
    
    public abstract class CommonProcessor: CommonTitle
    {
        public TypeProcessor Type { get; protected set; }
    }
}
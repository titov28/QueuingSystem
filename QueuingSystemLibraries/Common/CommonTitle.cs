namespace QueuingSystemLibraries.Common
{
   public abstract class CommonTitle
    {
        public void InitCommonTitle(ulong id, string title)
        {
            ID = id;
            Title = title;
        }
        public ulong ID { get; protected set; }
        public string Title { get; protected set; }

    }
}
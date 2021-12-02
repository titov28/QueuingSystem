using System.Threading;

namespace QueuingSystemLibraries.Other
{
    public static class Numerator
    {
        private static ulong _currentNumberSomeObject = 0;
        private static ulong _currentNumberOfClient = 2000;
        private static ulong _currentNumberOfRequest = 4000;
        

        public static ulong GetNewNumber()
        {
            return Interlocked.Increment(ref _currentNumberSomeObject);
        }
        
        public static ulong GetNewNumberClient()
        {
            return Interlocked.Increment(ref _currentNumberOfClient);
        }
        
        public static ulong GetNewNumberRequest()
        {
            return Interlocked.Increment(ref _currentNumberOfRequest);
        }
        
    }
}
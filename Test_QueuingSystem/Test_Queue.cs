using QueuingSystemLibraries.Common;
using QueuingSystemLibraries.Other;
using Xunit;
using QueuingSystemLibraries.QueuingModel;

namespace Test_QueuingSystem
{
    public class Test_Queue
    {
        
        [Fact]
        public void Try_Add_Client()
        {
            //arrange
            Queue sut = new Queue(1, "Test_queue", 2);
            bool flag = false;
            //act
            flag = sut.TryEnqueue(new Client(2, "Test_Client", TypeProcessor.Operating, new Request[]
            {
                new Request(3, "Test_Request", TypeRequest.ACNT, new Time(9, 0), new Time(0, 10))
            },new Time(1)));

            //assert
            Assert.True(flag);
            Assert.Equal(1, sut.NumberClient);
        }

        [Fact]
        public void Try_Add_If_Full()
        {
            //arrange
            Queue sut = new Queue(1, "Test_queue", 1);
            bool flag = false;
            //act
            flag = sut.TryEnqueue(new Client(2, "Test_Client", TypeProcessor.Operating,new Request[]
            {
                new Request(84, "Test_Request", TypeRequest.ACNT, new Time(9, 0), new Time(0, 10))
            }, new Time(1)));

            flag = sut.TryEnqueue(new Client(3, "Test_Client", TypeProcessor.Operating, new Request[]
            {
                new Request(85, "Test_Request", TypeRequest.ACNT, new Time(9, 0), new Time(0, 10))
            },new Time(1)));
            
            //assert
            Assert.False(flag);
            Assert.Equal(1, sut.NumberClient);
        }

        [Fact]
        public void Try_Dequeue()
        {
            //arrange
            Queue sut = new Queue(1, "Test_queue", 2);
            bool flag = false;
            flag = sut.TryEnqueue(new Client(2, "Test_Client",TypeProcessor.Operating, new Request[]
            {
                new Request(84, "Test_Request", TypeRequest.ACNT, new Time(9, 0), new Time(0, 10))
            }, new Time(1)));

            flag = sut.TryEnqueue(new Client(3, "Test_Client", TypeProcessor.Operating,new Request[]
            {
                new Request(85, "Test_Request", TypeRequest.ACNT, new Time(9, 0), new Time(0, 10))
            }, new Time(1)));

            Client client = null;
            
            //act
            flag = sut.TryDequeue(TypeProcessor.Operating, ref client);
            
            //assert
            Assert.True(flag);
            Assert.Equal(1, sut.NumberClient);
            Assert.NotNull(client);

        }

        [Fact]
        public void Try_Dequeue_client_with_different_processor_type()
        {
            //arrange
            Queue sut = new Queue(1, "Test_queue", 2);
            bool flag = false;
            flag = sut.TryEnqueue(new Client(2, "Test_Client",TypeProcessor.Exchange, new Request[]
            {
                new Request(84, "Test_Request", TypeRequest.ACNT, new Time(9, 0), new Time(0, 10))
            },new Time(1)));

            flag = sut.TryEnqueue(new Client(3, "Test_Client", TypeProcessor.Exchange,new Request[]
            {
                new Request(85, "Test_Request", TypeRequest.ACNT, new Time(9, 0), new Time(0, 10))
            },new Time(1)));

            Client client = null;
            
            //act
            flag = sut.TryDequeue(TypeProcessor.Operating, ref client);
            
            //assert
            Assert.False(flag);
            Assert.Equal(2, sut.NumberClient);
            Assert.Null(client);
        }

        [Fact]
        public void Try_Dequeue_without_clients()
        {
            //arrange
            Queue sut = new Queue(1, "Test_queue", 2);
            bool flag = false;
            Client client = null;
            
            //act
            flag = sut.TryDequeue(TypeProcessor.Operating, ref client);
            
            //assert
            Assert.False(flag);
        }
        
    }
}
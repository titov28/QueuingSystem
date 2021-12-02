using System;
using System.Collections.Concurrent;
using QueuingSystemLibraries.Common;
using QueuingSystemLibraries.Other;
using QueuingSystemLibraries.QueuingModel;


namespace ConsoleInterface
{
    class Program
    {
        static void Main(string[] args)
        {
            QSTimer timer = new QSTimer(new Time(9, 0), 500);
            
            
            Generator gr = new Generator(255, new Time(9, 0), new Time(17, 30));
            timer.TickTimerEvent += gr.TickTimerHandler;
            gr.Generate();
            
            int maxSizeQueue = 10;

            ulong localId = Numerator.GetNewNumber();
            Queue queue = new Queue(localId, $"Queue id:{localId}", maxSizeQueue);

            Processor[] processors = new Processor[7];

            for (int i = 0; i < 2; i++)
            {
                localId = Numerator.GetNewNumber();
                processors[i] = new Processor(localId, $"Processor id:{localId} type:{TypeProcessor.Exchange.ToString()}", TypeProcessor.Exchange, queue);
                timer.TickTimerEvent += processors[i].TickTimerHandler;
            }
            
            for (int i = 2; i < 7; i++)
            {
                localId = Numerator.GetNewNumber();
                processors[i] = new Processor(localId, $"Processor id:{localId} type:{TypeProcessor.Operating.ToString()}", TypeProcessor.Operating, queue);
                timer.TickTimerEvent += processors[i].TickTimerHandler;
            }
            
            localId = Numerator.GetNewNumber();
            QueuingSystem qs = new QueuingSystem(localId, $"QueuingSystem id:{localId}", queue, processors);
            gr.GeneratedClientEvent += qs.GeneratedClientHandler;
            Console.ReadLine();
        }
    }
}

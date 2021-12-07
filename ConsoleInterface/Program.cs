using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using QueuingSystemLibraries.Common;
using QueuingSystemLibraries.Other;
using QueuingSystemLibraries.QueuingModel;


namespace ConsoleInterface
{
    class Program
    {
        static void Main(string[] args)
        {
            Barrier barrier = new Barrier(10);
            Thread thread;
                        
            QSTimer timer = new QSTimer(new Time(8, 55), new Time(18,0), 500);
            timer.InitBarrier(barrier);
            
            Generator gr = new Generator(300, new Time(9, 0), new Time(17, 30));
            timer.TickTimerEvent += gr.TickTimerHandler;
            gr.Generate();
            gr.InitBarrier(barrier);
            thread = new Thread(gr.Start);
            thread.Name = "Generator";
            thread.Start();
            
            int maxSizeQueue = 10;

            ulong localId = Numerator.GetNewNumber();
            Queue queue = new Queue(localId, $"Queue id:{localId}", maxSizeQueue);

            Processor[] processors = new Processor[7];

            for (int i = 0; i < 2; i++)
            {
                localId = Numerator.GetNewNumber();
                processors[i] = new Processor(localId, $"Processor id:{localId} type:{TypeProcessor.Exchange.ToString()}", TypeProcessor.Exchange, queue);
                timer.TickTimerEvent += processors[i].TickTimerHandler;
                processors[i].InitBarrier(barrier);
                thread = new Thread(processors[i].Start);
                thread.Name = $"Processor id: {processors[i].ID}";
                thread.Start();
            }
            
            for (int i = 2; i < 7; i++)
            {
                localId = Numerator.GetNewNumber();
                processors[i] = new Processor(localId, $"Processor id:{localId} type:{TypeProcessor.Operating.ToString()}", TypeProcessor.Operating, queue);
                timer.TickTimerEvent += processors[i].TickTimerHandler;
                processors[i].InitBarrier(barrier);
                thread = new Thread(processors[i].Start);
                thread.Name = $"Processor id: {processors[i].ID}";
                thread.Start();
            }
            
            localId = Numerator.GetNewNumber();
            QueuingSystem qs = new QueuingSystem(localId, $"QueuingSystem id:{localId}", queue, processors);
            gr.GeneratedClientEvent += qs.GeneratedClientHandler;
            qs.InitBarrier(barrier);
            thread = new Thread(qs.Start);
            thread.Name = $"QueuingSystrem id: {qs.ID}";
            thread.Start();
            
            thread = new Thread(timer.Start);
            thread.Name = "Timer_thread";
            thread.Start();

            int processed = 0;
            int partiallyProcessed = 0;
            int rejection = 0;
            int error = 0;
            while (true)
            {
                Console.Clear();
                foreach (var client in QueuingSystem.GlobalQueue)
                {
                    switch (client.Status)
                    {
                        case ClientStatus.Processed:
                            processed++;
                            break;
                        case ClientStatus.PartiallyProcessed:
                            partiallyProcessed++;
                            break;
                        case ClientStatus.Rejection:
                            rejection++;
                            break;
                        default:
                            error++;
                            break;
                    }
                   
                }
                
                Console.Write($"Current time: {QSTimer.GetCurrentTime()}\n");
                Console.Write($"Clients:\n");
                Console.Write($"\tProcessed: {processed}\n");
                Console.Write($"\tPartially processed: {partiallyProcessed}\n");
                Console.Write($"\tRejection: {rejection}\n");
                Console.Write($"\tError: {error}\n");
                
                Console.Write($"Queuing System status: {qs.Status}\n");
                Console.Write($"Queue capacity: {qs.GetQueueCapacity()}\n");
                Console.Write($"Generator queue capacity: {gr.GetClients().Count}\n");
                //TODO Оформить вывод в консоль
                for (int i = 0; i < processors.Length; i++)
                {
                    Console.Write($"Processor id: {processors[i].ID} status: {processors[i].Status}\n");
                }
                
                
                Thread.Sleep(500);
                Console.SetCursorPosition(0,0);
                processed = 0;
                partiallyProcessed = 0;
                rejection = 0;
                error = 0;
            }
            
            
            //barrier.Dispose();
            Console.ReadLine();
        }
    }
}

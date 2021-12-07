using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using QueuingSystemLibraries.Common;
using QueuingSystemLibraries.Other;

namespace QueuingSystemLibraries.QueuingModel
{
    delegate bool SomeDo(Client cl);

    public enum QueuingSystemStatus
    {
        Ready,
        NotReady
    }
    
    public sealed class QueuingSystem: CommonTitle
    {
        private Barrier _barrier;
        public static ConcurrentQueue<Client> GlobalQueue = new ConcurrentQueue<Client>();
        private Queue _queue;
        private Processor[] _processors;

        public QueuingSystemStatus Status = QueuingSystemStatus.Ready;
        private SomeDo _doSomething;
        private Client _movedClient;
        public QueuingSystem(ulong id, string title, Queue queue, Processor[] processors)
        {
            if (title is null || queue is null || processors is null)
            {
                throw new NullReferenceException();
            }

            InitCommonTitle(id, title);

            _queue = queue;
            _processors = processors;
        }

        public void Start()
        {
            while (true)
            {
                if (_doSomething != null)
                {
                    if (!_doSomething(_movedClient))
                    {
                        _movedClient.Rejection();
                    }
                    _movedClient = null;
                    Unsubscribe();
                    Status = QueuingSystemStatus.Ready;
                }
                
                if (_barrier is not null)
                {
                    //Console.WriteLine($"{Thread.CurrentThread.Name} wrote");
                    _barrier.SignalAndWait();
                }
            }
        }
        
        public void InitBarrier(Barrier br)
        {
            if (br is not null)
            {
                _barrier = br;
            }
        }
        
        public static TypeProcessor WhatProcessorNeed(TypeRequest tr)
        {
            TypeProcessor processor ; 
            switch (tr)
            {
                case TypeRequest.XCHG:
                    processor = TypeProcessor.Exchange;
                    break;
                case TypeRequest.ACNT:
                case TypeRequest.CARD:
                case TypeRequest.CRED:
                    processor = TypeProcessor.Operating;
                    break;
                default: 
                    throw new Exception("Некорректный тип Request");
                    break;
            }

            return processor;
        }
       
        private void Subscribe()
        {
            _doSomething = _queue.TryEnqueue;
        }

        private void Unsubscribe()
        {
            _doSomething = null;
        }

        public void GeneratedClientHandler(object sender, GeneratedClientEventArgs e)
        {
            if (Status == QueuingSystemStatus.Ready)
            {
                Status = QueuingSystemStatus.NotReady;
                Interlocked.Exchange(ref _movedClient , e.Client);
                _movedClient.EnteredInSystem();
                Subscribe();
                GlobalQueue.Enqueue(_movedClient);
            }
            else
            {
                e.Client.Rejection();
                GlobalQueue.Enqueue(e.Client);
            }
        }

        public int GetQueueCapacity()
        {
            return _queue.NumberClient;
        }
        
    }
}
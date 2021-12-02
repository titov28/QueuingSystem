using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using QueuingSystemLibraries.Common;
using QueuingSystemLibraries.Other;

namespace QueuingSystemLibraries.QueuingModel
{
    delegate bool SomeDo(Client cl);

    enum QueuingSystemStatus
    {
        Ready,
        NotReady
    }
    
    public sealed class QueuingSystem: CommonTitle
    {

        public static List<Client> GlobalQueue = new List<Client>();
        private Queue _queue;
        private Processor[] _processors;

        private QueuingSystemStatus Status = QueuingSystemStatus.Ready;
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
                    Status = QueuingSystemStatus.NotReady;
                    GlobalQueue.Add(_movedClient);
                    if (!_doSomething(_movedClient))
                    {
                        _movedClient.Rejection();
                    }
                    _movedClient = null;
                    Unsubscribe();
                    Status = QueuingSystemStatus.Ready;
                }
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

        public static void AddInGlobalQueue(Client cl)
        {
            GlobalQueue.Add(cl);
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
                Interlocked.Exchange(ref _movedClient , e.Client);
                _movedClient.EnteredInSystem();
                Subscribe();
            }
            else
            {
                e.Client.Rejection();
                GlobalQueue.Add(e.Client);
            }
        }
        
    }
}
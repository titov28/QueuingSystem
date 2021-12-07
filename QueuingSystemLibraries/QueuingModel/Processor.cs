using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using QueuingSystemLibraries.Common;
using QueuingSystemLibraries.Other;

namespace QueuingSystemLibraries.QueuingModel
{
    
    
    public sealed class Processor: CommonProcessor
    {
        private Barrier _barrier;
        private Time _currentTime;
        private Time _finishTime;
        private Queue _queue;
        private Client _currentClient;
        private Random _random;
        private bool _clientAddedFlag = false;


        public Processor(ulong id, string title, TypeProcessor type, Queue queue)
        {
            if (title is null || queue is null)
            {
                throw new NullReferenceException();
            }
            InitCommonTitle(id, title);
            Type = type;
            _random = new Random();
            _queue = queue;
            Subscribe();
        }

        public void Start()
        {
            while (true)
            {
                if (_clientAddedFlag)
                {
                    if (!FindClient())
                    {
                        continue;
                    }
                }
                
                if (_currentClient is not null)
                {
                    
                    StartProcessing();
                    
                    Ready();
                    
                    if (!_queue.IsEmpty())
                    {
                        FindClient();
                    }
                    
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
        
        private void StartProcessing()
        {
            Processing();
            _currentClient.ProcessingStart();
            foreach (var request in _currentClient.Requests)
            {
                if (request.Status != RequestStatus.Processed)
                {
                    if (CanProcessing(request.Type))
                    {
                        request.ProcessingStart();
                        _finishTime = _currentTime + request.ProcessingTime.Clone();
                        _finishTime.AddMinutes(_random.Next(request.DeviationTime.GetMinutes()));
                        while (_currentTime <= _finishTime)
                        {
                            if (request.IsInterrupted())
                            {
                                //Interrupted();
                                //TODO Придумать, как добавить прерывание
                            }
                            if (_barrier is not null)
                            {
                                //Console.WriteLine($"{Thread.CurrentThread.Name} wrote");
                                _barrier.SignalAndWait();
                            }
                        }
                        request.ProcessingFinish();
                    }
                    else
                    {
                        break;
                    }
                }
            }
            
            if (!_currentClient.IsProcessed())
            {
                _currentClient.ChangeCurrentProcessor();
                
                if (!_queue.TryEnqueue(_currentClient))
                {
                    _currentClient.PartiallyProcessed();
                }
            }
            else
            {
                _currentClient.ProcessingFinish();
            }

            _currentClient = null;
        }

        private bool CanProcessing(TypeRequest tr)
        {
            return tr == TypeRequest.XCHG ? Type == TypeProcessor.Exchange : Type == TypeProcessor.Operating;
        }

        private bool FindClient()
        {
            bool flag = false;
            FindingClient();
            //Unsubscribe();
            Volatile.Write(ref _clientAddedFlag , false);
            flag = _queue.TryDequeue(Type, ref _currentClient);
            if (!flag)
            {
                Ready();
                //Subscribe();
            }

            return flag;
        }
        
        private void Subscribe()
        {
            _queue.ClientAdded += ClientAddedHandler;
        }

        private void Unsubscribe()
        {
            _queue.ClientAdded -= ClientAddedHandler;
        }
        public void ClientAddedHandler(object sender, EventArgs e)
        {
            if (Status == ProcessorStatus.Ready)
            {
                Volatile.Write(ref _clientAddedFlag, true);
            }
        }

        public void TickTimerHandler(object sender, TimeEventArgs e)
        {
            Interlocked.Exchange(ref _currentTime, e.CurrentTime);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using QueuingSystemLibraries.Common;
using QueuingSystemLibraries.QueuingModel;

namespace QueuingSystemLibraries.Other
{
    public class Generator
    {
        private Barrier _barrier;
        private int _numberClients;
        private int _interval;
        private Time _timeStartGeneratingClient;
        private Time _timeFinishGeneratingClient;

        private Random _randomGeneratorForClient = new Random();
        private Random _randomGeneratorForRequest = new Random();
        private Random _randomIntervalGenerator = new Random();
        
        
        private List<Client> Clients;
        private Time _currentTime;

        public event EventHandler<GeneratedClientEventArgs> GeneratedClientEvent; 

        public Generator(int numberClients, Time startGeneretingTime, Time finishGeneratingTime)
        {
            if (startGeneretingTime is null || finishGeneratingTime is null)
            {
                throw new NullReferenceException();
            }

            if (startGeneretingTime >= finishGeneratingTime)
            {
                throw new ArgumentException();
            }
            
            if (numberClients <= 0)
            {
                numberClients = 1;
            }
            
            _numberClients = numberClients;
            _timeStartGeneratingClient = startGeneretingTime;
            _timeFinishGeneratingClient = finishGeneratingTime;
            _interval = ((finishGeneratingTime - startGeneretingTime)).GetTimeInMinutes() / _numberClients;
            Clients = new List<Client>(_numberClients);
            _currentTime = new Time(8, 30);
        }

        public void Start()
        {
            while (true)
            {
                if (_currentTime != _timeFinishGeneratingClient)
                {
                    if (Clients.Count != 0)
                    {
                        for (int i = 0; i < Clients.Count; i++)
                        {
                            if (Clients[i].StartGoingInSystem > _currentTime)
                            {
                                break;
                            }
                            
                            if (Clients[i].StartGoingInSystem <= _currentTime)
                            {
                                OnGeneratedClient(new GeneratedClientEventArgs(Clients[i]));
                                Clients.RemoveAt(i);
                            }

                        }
                    }
                }

                if (_barrier is not null)
                {
                    //Console.WriteLine($"{Thread.CurrentThread.Name} wrote");
                    _barrier.SignalAndWait();
                }

            }

        }
        private void OnGeneratedClient(GeneratedClientEventArgs e)
        {
            EventHandler<GeneratedClientEventArgs> handler = Volatile.Read(ref GeneratedClientEvent);
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void InitBarrier(Barrier br)
        {
            if (br is not null)
            {
                _barrier = br;
            }
        }
        
        public void Generate()
        {
            Time currentTime = _timeStartGeneratingClient.Clone();
            int randomInterval = 0;
            for (int i = 0; i < _numberClients; i++)
            {
                Clients.Add(CreateClient(currentTime));
                //randomInterval = _randomIntervalGenerator.Next(_interval);
                //currentTime.AddMinutes(randomInterval);
                currentTime += new Time(_interval);
            }
            
        }

        private Client CreateClient(Time currentTime)
        {
            int clientSize = 1;
            int randomNumber = _randomGeneratorForClient.Next(100);
            if (randomNumber < 2)
            {
                clientSize = 2;
            }

            if (randomNumber is >= 2 and < 5)
            {
                clientSize = 3;
            }

            ulong clientID = Numerator.GetNewNumberClient();
            Request[] requests = CreateRequest(clientSize);
            TypeProcessor tp = requests[0].Type is TypeRequest.XCHG ? TypeProcessor.Exchange : TypeProcessor.Operating;

            return new Client(clientID, $"Client ID: {clientID}", tp, requests, currentTime);
        }
        
        private Request[] CreateRequest(int size)
        {
            Request[] requests = new Request[size];
            ulong requestID;
            TypeRequest tr;
            for(int i = 0 ; i < size; i++)
            {
                requestID = Numerator.GetNewNumberRequest();
                tr = (TypeRequest)_randomGeneratorForRequest.Next(4);
                requests[i] = new Request(requestID, $"Request ID: {requestID}", tr, GetProcessingTime(tr),
                    GetDeviationTime(tr));
            }

            return requests;
        }

        private Time GetProcessingTime(TypeRequest tr)
        {
            Time pt;
            switch (tr)
            {
                case TypeRequest.XCHG:
                    pt = new Time(10);
                    break;
                case TypeRequest.CARD:
                    pt = new Time(8);
                    break;
                case TypeRequest.CRED:
                    pt = new Time(6);
                    break;
                case TypeRequest.ACNT:
                    pt = new Time(4);
                    break;
                default: pt = new Time(15);
                    break;
            }

            return pt;
        }

        private Time GetDeviationTime(TypeRequest tr)
        {
            Time dt;
            switch (tr)
            {
                case TypeRequest.XCHG:
                    dt = new Time(5);
                    break;
                case TypeRequest.CARD:
                    dt = new Time(4);
                    break;
                case TypeRequest.CRED:
                    dt = new Time(3);
                    break;
                case TypeRequest.ACNT:
                    dt = new Time(2);
                    break;
                default: dt = new Time(5);
                    break;
            }

            return dt;
        }


        public List<Client> GetClients()
        {
            return Clients;
        }
        
        
        public void TickTimerHandler(object sender, TimeEventArgs e)
        {
            Interlocked.Exchange(ref _currentTime, e.CurrentTime);
        }

        
        
    }

    public class GeneratedClientEventArgs : EventArgs
    {
        public Client Client { get; private set; }

        public GeneratedClientEventArgs(Client cl)
        {
            if (cl is null)
            {
                throw new NullReferenceException();
            }

            Client = cl;
        }
    }
    
}
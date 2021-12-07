using System;
using System.Threading;
using System.Collections.Generic;

namespace QueuingSystemLibraries.Other
{
    public class QSTimer
    {
        private Barrier _barrier;
        private static readonly ReaderWriterLockSlim _readerWriterLock =
            new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        private static Time _currentTime;
        private static Time _finish;
        private static Timer _timer;
        private int _delay;

        public event EventHandler<TimeEventArgs> TickTimerEvent;

        public QSTimer(Time startTime, Time finishTime, int delay)
        {
            if (startTime is null || finishTime is null)
            {
                throw new NullReferenceException();
            }

            if (delay <= 0)
            {
                delay = 500;
            }

            _currentTime = startTime;
            _finish = finishTime;
            _delay = delay;
            //_timer = new Timer(IncrementTime, 1, Timeout.Infinite, Timeout.Infinite);
        }

        public void Start()
        {
           // _timer.Change(_delay, Timeout.Infinite);
           while (true)
           {
               if(_currentTime != _finish) IncrementTime(1);
              
               if (_barrier is not null)
               {
                   //
                   _barrier.SignalAndWait();
               }
               //Thread.Sleep(_delay);
           }
        }

        public void InitBarrier(Barrier br)
        {
            if (br is not null)
            {
                _barrier = br;
            }
        }

        private void IncrementTime(object interval)
        {
            //_readerWriterLock.EnterWriteLock();
            Time newTime = _currentTime + new Time((int)interval);
            Interlocked.Exchange(ref _currentTime , newTime);
            //_readerWriterLock.ExitWriteLock();
            OnTickTimer(new TimeEventArgs(_currentTime));
            //_timer.Change(_delay, Timeout.Infinite);
        }

        private void OnTickTimer(TimeEventArgs e)
        {
            EventHandler<TimeEventArgs> handler = Volatile.Read(ref TickTimerEvent);
            if (handler != null)
            {
                handler.Invoke(this, e);
            }

        }

        public static Time GetCurrentTime()
        {
            Time time;
            //_readerWriterLock.EnterReadLock();
            time = _currentTime.Clone();
            //_readerWriterLock.ExitReadLock();
            return time;
        }

    }

    public class TimeEventArgs : EventArgs
    {
        public readonly Time CurrentTime;

        public TimeEventArgs(Time currentTime)
        {
            CurrentTime = currentTime;
        }
    }


}
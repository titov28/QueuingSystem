using System;
using System.Threading;

namespace QueuingSystemLibraries.Other
{
    public class QSTimer
    {
        private static readonly ReaderWriterLockSlim _readerWriterLock =
            new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        private static Time _currentTime;
        private static Timer _timer;
        private int _delay;

        public event EventHandler<TimeEventArgs> TickTimerEvent;

        public QSTimer(Time startTime, int delay)
        {
            if (startTime is null)
            {
                throw new NullReferenceException();
            }

            if (delay <= 0)
            {
                delay = 500;
            }

            _currentTime = startTime;
            _delay = delay;
            _timer = new Timer(IncrementTime, 1, Timeout.Infinite, Timeout.Infinite);
        }

        public void Start()
        {
            _timer.Change(_delay, Timeout.Infinite);
        }


        private void IncrementTime(object interval)
        {
            _readerWriterLock.EnterWriteLock();
            _currentTime += new Time((int)interval);
            _readerWriterLock.ExitWriteLock();
            OnTickTimer(new TimeEventArgs(_currentTime));
            _timer.Change(_delay, Timeout.Infinite);
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
            _readerWriterLock.EnterReadLock();
            time = _currentTime.Clone();
            _readerWriterLock.ExitReadLock();
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
using System;

namespace QueuingSystemLibraries.Other
{
    public class Time
    {
        private int _minutes = 0;
        private static int _maxMinutes = 1440;
        
        public Time(){}
        
        public Time(int hours, int minutes)
        {
            Init(hours, minutes);
        }

        public Time(int minutes)
        {
            minutes = minutes % _maxMinutes;

            if (minutes < 0)
            {
                minutes = _maxMinutes + minutes;
            }
            
            _minutes = minutes;
        }
        
        private void Init(int hours, int minutes)
        {
            if (hours < 0 || hours > 23)
            {
                hours = 0;
            }

            if (minutes < 0 || minutes > 59)
            {
                minutes = 0;
            }
            
            _minutes = hours * 60 + minutes;
        }
        
        public void AddMinutes(int minutes)
        {
           
            int tempMinutes = minutes % _maxMinutes;

            if (tempMinutes < 0 && Math.Abs(tempMinutes) > _minutes)
            {
                _minutes = Math.Abs((_minutes + tempMinutes + _maxMinutes) % _maxMinutes);
            }
            else
            {
                _minutes = Math.Abs((_minutes + tempMinutes) % _maxMinutes);
            }

        }   

        public void AddHours(int hours)
        {
            if ((int.MaxValue / 60) < hours || (int.MinValue / 60) > hours)
            {
                return;
            }
            
            AddMinutes(hours * 60);
        }

        public int GetHours()
        {
            return _minutes / 60;
        }

        public int GetMinutes()
        {
            return _minutes % 60;
        }

        public Time Clone()
        {
            return new Time(_minutes);
        }

        public int GetTimeInMinutes()
        {
            return _minutes;
        }
            
        public void SetTime(int hours, int minutes)
        {
            Init(hours, minutes);
        }


        #region Operators

        public static Time operator +(Time t1, Time t2)
        {
            return new Time(t1._minutes + t2._minutes);
        }

        public static Time operator -(Time t1, Time t2)
        {
            int minutes = t1._minutes - t2._minutes;
            if (minutes < 0)
            {
                minutes = _maxMinutes + minutes;
            }

            return new Time(minutes);
        }
        
        public static bool operator ==(Time t1, Time t2)
        {
            if (t1 is null || t2 is null) return false;
            return t1._minutes == t2._minutes;
        }
        
        public static bool operator !=(Time t1, Time t2)
        {
            if (t1 is null || t2 is null) return true;
            return t1._minutes != t2._minutes;
        }

        public static bool operator >(Time t1, Time t2)
        {
            return t1._minutes > t2._minutes;
        }
        
        public static bool operator <(Time t1, Time t2)
        {
            return t1._minutes < t2._minutes;
        }
        
        public static bool operator >=(Time t1, Time t2)
        {
           return t1._minutes >= t2._minutes;
        }
        
        public static bool operator <=(Time t1, Time t2)
        {
            return t1._minutes <= t2._minutes;
        }
        
        #endregion
        
        
        
        public override string ToString()
        {
            int hours = _minutes / 60;
            int minutes = _minutes % 60;

            return $"{hours:00}:{minutes:00}";
        }
    }
}
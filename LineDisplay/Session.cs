using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Data;
using System.Reflection;
using System.Reflection.Emit;

namespace LineDisplay
{
    public class Session
    {
        public int Shift { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        DateTime startTime;

        public bool Isbreak = false;
        public string StartTime
        {
            get { return startTime.ToString("yyyy-MM-dd HH:mm:ss"); }
            set
            {
                if (value == String.Empty) 
                {

                }
                else
                {
                    try
                    {
                        startTime = DateTime.Parse(value);
                    }
                    catch (Exception e)
                    {
                        return;
                    }
                }
            }
        }

        DateTime endTime;
        public string EndTime
        {
            get { return endTime.ToString("yyyy-MM-dd HH:mm:ss"); }
            set
            {
                if (value == String.Empty)
                {

                }
                else
                {
                    try
                    {
                        endTime = DateTime.Parse(value);
                    }
                    catch (Exception e)
                    {
                        return;
                    }
                }
            }
        }

        public Session()
        {
        }
        public Session(int shift, int id, string description, DateTime startTime, DateTime endTime)
        {
            Shift = shift;
            ID = id;
            Name = description;
            this.startTime = startTime;
            this.endTime = endTime;
        }

        public Session(String from, String to)
        {
            StartTime = from;
            EndTime = to;
        }
        public Session(int shift, int id, string description, String startTime, String endTime)
        {
            Shift = shift;
            ID = id;
            Name = description;
            StartTime = startTime;
            EndTime = endTime;
        }

        public Session(int shift, int id, String startTime, String endTime)
        {
            Shift = shift;
            ID = id;
            Name = string.Empty;
            StartTime = startTime;
            EndTime = endTime;
        }

        public bool IsWithin(DateTime ts)
        {
            DateTime start = new DateTime(ts.Year, ts.Month, ts.Day, startTime.Hour, startTime.Minute, startTime.Second);
            DateTime end = new DateTime(ts.Year, ts.Month, ts.Day, endTime.Hour, endTime.Minute, endTime.Second);



            if (end < start)
            {
                if (ts.TimeOfDay > start.TimeOfDay)
                    end = end.AddDays(1);
                else if (ts.TimeOfDay < start.TimeOfDay)
                    start = start.AddDays(-1);
            }



            if ((ts >= start) && ts < end)
                return true;
            return false;

        }

        public override string ToString()
        {
            return startTime.ToString("HH:mm");
        }
    }


    public class SessionCollection : ObservableCollection<Session>
    {
        public Session getCurrentSession()
        {
            IEnumerator<Session> enumerator = this.GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (enumerator.Current.IsWithin(DateTime.Now))
                {
                    return enumerator.Current;
                }

            }
            return null;
        }


        public TimeSpan getTotalTimeWithin()
        {
            TimeSpan TotalTime = new TimeSpan();
            List<TimeSpan> timeSpanList = new List<TimeSpan>();
            DateTime notionalendTime;
            DateTime notionalstartTime;
            DateTime startTime = DateTime.Now;
            DateTime endTime;
            IEnumerator<Session> enumerator = this.GetEnumerator();
            while (enumerator.MoveNext())
            {
                notionalendTime =DateTime.Parse(enumerator.Current.EndTime);
                notionalstartTime = DateTime.Parse(enumerator.Current.StartTime);

                startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, notionalstartTime.Hour,
                    notionalstartTime.Minute, notionalstartTime.Second);
                endTime =
                    new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, notionalendTime.Hour, notionalendTime.Minute,
                        notionalendTime.Second);
                       
                if (endTime < DateTime.Now)
                {
                    timeSpanList.Add(endTime - startTime);
                    continue;
                }
                break;
            }

            if( enumerator.Current.Isbreak == false )
                timeSpanList.Add(DateTime.Now - startTime);

            foreach (TimeSpan ts in timeSpanList)
            {
                TotalTime += ts;
            }
            return TotalTime;
        }
    }

    public class sessionInfo
    {
        public int ShiftIndex { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        public sessionInfo()
        {
        }
    }
}

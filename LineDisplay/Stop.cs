using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace LineDisplay
{
    public class Stop
    {
        public Timer timeoutTimer;
        public event EventHandler<StopTimeoutArgs> StopTimeoutEvent;
        public Stop()
        {
            ID = -1;
            //timeoutTimer = new Timer(60*60*1000);
            timeoutTimer = new Timer(60 * 60);
            timeoutTimer.AutoReset = false;
            timeoutTimer.Elapsed += timeoutTimer_Elapsed;
        }

        public Stop(double stopCloseDuration)
        {
            ID = 0;
            //timeoutTimer = new Timer(60*60*1000);
            timeoutTimer = new Timer(stopCloseDuration*60*1000);
            timeoutTimer.AutoReset = false;
            timeoutTimer.Elapsed += timeoutTimer_Elapsed;
        }

        void timeoutTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timeoutTimer.Stop();
            if (StopTimeoutEvent != null)
                StopTimeoutEvent(this, new StopTimeoutArgs(ID));
        }

        public int ID { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int Code { get; set; }
        public string Comment { get; set; }
        public String Status { get; set; }
        public Boolean Updated{ get; set; }
    }
}

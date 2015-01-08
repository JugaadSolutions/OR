using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using System.Diagnostics;

namespace LineDisplay
{
    public enum COMMAND { MACHINE_OFF = 0, CANCEL_MACHINE_OFF };
    public partial class MachineStatus
    {

        Timer tick;
        Timer cycleTimer;
        Timer planTimer;
        

        

        DateTime stopTs;
        public event EventHandler<StopInfo> CloseStopEvent;
        public void Timer_Init()
        {
           tick = new Timer(1000);
           tick.Elapsed += new ElapsedEventHandler(tick_Elapsed);
           tick.AutoReset = false;


           cycleTimer = new Timer();
            cycleTimer.Elapsed +=cycleTimer_Elapsed;
            cycleTimer.AutoReset = false;

            planTimer = new Timer();
            planTimer.Elapsed += planTimer_Elapsed;
            planTimer.AutoReset=false;

           

       
        }

     
        public void TimerStart()
        {
            tick.Start();
            
            //if (Status != MACHINE_STATUS.IN_BREAK)
            //{
            //    if (Project != null && Project.CycleTime <= 0)
            //    {
            //        cycleTimer.Start();

            //        planTimer.Start();
            //    }
            //}
        }

        public void TimerStop()
        {
            if (tick != null)
            {
                tick.Stop();
            }
            if (cycleTimer != null)
            {
                cycleTimer.Stop();
            }
            if (planTimer != null)
            {
                planTimer.Stop();
            }

            
        }

        void planTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Project.CycleTime <= 0) return;
            planTimer.Stop();
            updatePlan();
            planTimer.Interval = Project.CycleTime * 1000;
            planTimer.Start();
        }


        void tick_Elapsed(object sender, ElapsedEventArgs e)
        {
            
            update();

            tick.Start();
        }



        void cycleTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (currentStop == null && Status == MACHINE_STATUS.ACTIVE)
            {
                stopTs = DateTime.Now;
                currentStop = dataAccess.insertStop(stopTs, MachineID, stopCloseDuration);

                currentStop.StopTimeoutEvent += currentStop_StopTimeoutEvent;

                stopWatch = new Stopwatch();
                stopWatch.Restart();
                Status = MACHINE_STATUS.SPEED_LOSS;

            }
        }

        void currentStop_StopTimeoutEvent(object sender, StopTimeoutArgs e)
        {
            
            dataAccess.CloseStop(e.ID);
            if (CloseStopEvent != null)
            {
                CloseStopEvent(this, new StopInfo(e.ID, DateTime.Now));
            }
        }

    }

    public class Command
    {
        public int SlNo {get;set;}
        public COMMAND ID {get;set;}
        public int Parmeters {get;set;}
        public int Status {get;set;}
    }

}

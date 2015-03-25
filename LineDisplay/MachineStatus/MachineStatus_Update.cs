using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace LineDisplay
{
    public partial class MachineStatus
    {
        Stopwatch stopWatch;
        Stop currentStop;
        void update()
        {

            if (updateShift() == true)
            {
                ShiftChanged = true;
                return;
            }
            ShiftChanged = false;

            if (updateSession())
                return;

            switch (Status)
            {
                case MACHINE_STATUS.UNDEFINED:
                case MACHINE_STATUS.OFF: return;
                case MACHINE_STATUS.IN_BREAK:

                    if (inBreak() == false)
                    {
                        Status = previousStatus;
                        planTimer.Interval = Project.CycleTime * 1000;
                        planTimer.Start();
                        cycleTimer.Interval = Project.CycleTime * 1000;
                        cycleTimer.Start();

                    }
                    break;

                case MACHINE_STATUS.SPEED_LOSS:
                    stopWatch.Stop();
                    if (stopWatch.ElapsedMilliseconds > (tms / 100) * 1000 * cycletime)
                    {
                        Status = MACHINE_STATUS.STOPPED;
                        currentStop.Status = "Open";
                        currentStop.Code = 0;

                        dataAccess.updateStop_Open(currentStop);
                    }
                    else stopWatch.Start();
                    break;


                case MACHINE_STATUS.ACTIVE:
                case MACHINE_STATUS.STOPPED:



                    if (inBreak() == true)
                    {
                        if (Status != MACHINE_STATUS.IN_BREAK)
                        {
                            previousStatus = Status;
                            Status = MACHINE_STATUS.IN_BREAK;
                            planTimer.Stop();
                            cycleTimer.Stop();

                        }
                        break;
                    }
                    break;

                default: break;
            }

            if (Status != MACHINE_STATUS.UNDEFINED && Status != MACHINE_STATUS.OFF)
            {

                updateManPower();
                updateMachineParameters();
                updateProject();
            }







        }

        bool inBreak()
        {
            return CurrentShift.inBreak();
        }

        void updateMachineParameters()
        {
            double rm, omin, omax, gmin, gmax;
            String n;

            dataAccess.getMachineDetails(MachineID, out n, out GroupID,
              out rm, out omin, out omax, out gmin, out gmax, out tms, out mpInputStartupDuration, out stopCloseDuration);
            RedMax = rm;
            OrangeMin = omin;
            GreenMax = gmax;
            OrangeMax = omax;
            GreenMin = gmin;
            Name = n;
        }

        void updateProject()
        {
            Project curProject = dataAccess.getProject(MachineID);

            if (curProject != null)
            {
                if (curProject.ID != Project.ID)
                {
                    if (previousStatus != MACHINE_STATUS.NONE)
                        Status = previousStatus;
                    cycletime = curProject.CycleTime;
                    Project = curProject;

                }
            }
            else
            {
                if (Status != MACHINE_STATUS.UNDEFINED) previousStatus = Status;
                Project.ID = -1;
                Status = MACHINE_STATUS.UNDEFINED;
                planTimer.Stop();
                cycleTimer.Stop();
            }
        }

        bool updateShift()
        {
            bool result = false;
            Shift s = Shifts.getCurrentShift();
            if (s.ID != currentShift.ID)
            {
                TimerStop();        //stop timers



                dataAccess.updateOpenStops(MachineID);





                dataAccess.CloseManpowerInput(MachineID);   //close manpower
                dataAccess.closeProjectSession(MachineID, Project.ID, CurrentShift.ID,
                    SessionActual - previousActual, SessionPlan - previousPlan);
                SessionPlan = 0;    //reset counts
                SessionActual = 0;
                ShiftPlan = 0;
                ShiftActual = 0;
                previousPlan = 0;
                previousPlan = 0;
                MPInput = 0;

                Project.ID = -1;
                Shifts = dataAccess.getShifts(MachineID);
                s = Shifts.getCurrentShift();

                CurrentShift = s;

                CurrentShift.Sessions = dataAccess.getSessions(CurrentShift.ID, MachineID);
                CurrentShift.Breaks = dataAccess.getBreaks(CurrentShift.ID, MachineID);


                previousStatus = MACHINE_STATUS.NONE;

                if (Status != MACHINE_STATUS.OFF)
                {
                    if (CurrentShift.IsActive)
                    {

                        Status = MACHINE_STATUS.ACTIVE;



                    }
                    else Status = MACHINE_STATUS.UNDEFINED;
                }

                updateOR();
                updateMachineParameters();
                updateProject();





                result = true;

                DateTime ts = DateTime.Now;

                DateTime temp = DateTime.Parse(CurrentShift.StartTime);
                curShiftStart = new DateTime(ts.Year, ts.Month, ts.Day, temp.Hour,
                   temp.Minute, temp.Second);

                temp = DateTime.Parse(CurrentShift.EndTime);

                curShiftEnd = new DateTime(ts.Year, ts.Month, ts.Day, temp.Hour,
                  temp.Minute, temp.Second);


                if (curShiftEnd < curShiftStart)
                {
                    if (ts.TimeOfDay > curShiftStart.TimeOfDay)
                        curShiftEnd = curShiftEnd.AddDays(1);
                    else if (ts.TimeOfDay < curShiftStart.TimeOfDay)
                        curShiftStart = curShiftStart.AddDays(-1);
                }

                TimerStart();
            }

            return result;
        }


        bool updateSession()
        {
            double rm, omin, omax, gmin, gmax;
            String n;
            bool result = false;
            //if (CurrentShift.IsActive == false) return false;
            Session s = currentShift.Sessions.getCurrentSession();
            if (s != currentSession)
            {
                dataAccess.updateProjectSession(MachineID, Project.ID, CurrentShift.ID,
                  SessionActual - previousActual, SessionPlan - previousPlan);



                SessionActual = 0;
                SessionPlan = 0;
                previousPlan = 0;
                previousActual = 0;

                CurrentSession = s;

                updateMachineParameters();
                updateProject();


                DateTime ts = DateTime.Now;

                DateTime temp = DateTime.Parse(CurrentSession.StartTime);
                curSessionStart = new DateTime(ts.Year, ts.Month, ts.Day, temp.Hour,
                   temp.Minute, temp.Second);

                temp = DateTime.Parse(CurrentSession.EndTime);

                curSessionEnd = new DateTime(ts.Year, ts.Month, ts.Day, temp.Hour,
                  temp.Minute, temp.Second);


                if (curSessionEnd < curSessionStart)
                {
                    if (ts.TimeOfDay > curShiftStart.TimeOfDay)
                        curSessionEnd = curSessionEnd.AddDays(1);
                    else if (ts.TimeOfDay < curSessionStart.TimeOfDay)
                        curSessionStart = curSessionStart.AddDays(-1);
                }


                updateOR();

                result = true;
            }

            return result;
        }

        DateTime? updatePlan()
        {
            SessionPlan = dataAccess.getMaxSessionPlan(MachineID, curSessionStart.ToString("yyyy-MM-dd HH:mm:ss"),
                    curSessionEnd.ToString("yyyy-MM-dd HH:mm:ss"));
            SessionPlan++;

            ShiftPlan++;

            cycletime = Project.CycleTime;

            dataAccess.updatePlan(SessionPlan, MachineID);

            updateOR();

            return DateTime.Now;

        }

        public DateTime? updateActual()
        {
            DateTime from;
            DateTime to;

            cycleTimer.Stop();
            if (Status == MACHINE_STATUS.OFF || (Status == MACHINE_STATUS.UNDEFINED)) return null;
            DateTime fromTs = DateTime.Parse(CurrentSession.StartTime);

            DateTime toTs = DateTime.Parse(currentSession.EndTime);

            if (fromTs.Day != toTs.Day)
            {
                if (DateTime.Now.Hour == 0)
                {
                    from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 1,
                fromTs.Hour, fromTs.Minute, fromTs.Second);
                    to = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                toTs.Hour, toTs.Minute, toTs.Second);
                }
                else
                {
                    from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                fromTs.Hour, fromTs.Minute, fromTs.Second);
                    to = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day + 1,
                toTs.Hour, toTs.Minute, toTs.Second);
                }
            }
            else
            {
                from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
               fromTs.Hour, fromTs.Minute, fromTs.Second);
                to = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
               toTs.Hour, toTs.Minute, toTs.Second);
            }

            int mi = dataAccess.getMachineInputs(from, to, MachineID);

            int pulseCount = dataAccess.getMachinePulseCount(MachineID);

            int actual = (int)System.Math.Floor(Convert.ToDouble(mi) / pulseCount);

            if (actual == SessionActual)
                return null;

            else
            {
                ShiftActual += actual - SessionActual;

                SessionActual = actual;

                dataAccess.updateActual(SessionActual, MachineID);
                if (Status == MACHINE_STATUS.STOPPED)
                {
                    Status = MACHINE_STATUS.ACTIVE;
                    currentStop = null;
                    dataAccess.updateStop_To(MachineID);
                }

                else if (Status == MACHINE_STATUS.SPEED_LOSS)
                {
                    Status = MACHINE_STATUS.ACTIVE;

                    currentStop = null;
                    stopWatch.Stop();
                    dataAccess.updateStop_ToSpeedLoss(MachineID);
                }

                else if (Status == MACHINE_STATUS.IN_BREAK)
                {
                    previousStatus = MACHINE_STATUS.ACTIVE;
                    dataAccess.updateStop_To(MachineID);
                }
                currentStop = null;

                cycleTimer.Interval = (Project.CycleTime) * 1000;

                if (Status != MACHINE_STATUS.IN_BREAK)
                {
                    cycleTimer.Start();
                }
                updateOR();
                return DateTime.Now;
            }



        }


        void updateOR()
        {
            if (SessionPlan == 0)
                SessionOR = 0;
            else
            {
                SessionOR = ((double)SessionActual / SessionPlan * 100);
            }

            if (ShiftPlan == 0)
                ShiftOR = 0;
            else
            {
                ShiftOR = ((double)ShiftActual / ShiftPlan * 100);
            }
        }

        public void updateManPower()
        {



            int SlNo = dataAccess.HasMPInputOpen(MachineID, CurrentShift.ID, curShiftStart.ToString("yyyy-MM-dd HH:mm:ss"),
                curShiftEnd.ToString("yyyy-MM-dd HH:mm:ss"));
            if (SlNo == 0)
            {
                if ((DateTime.Now - curShiftStart).TotalMinutes > mpInputStartupDuration)
                {
                    MPInput = dataAccess.InsertMPInput(MachineID, CurrentShift.ID);
                }
                else MPInput = 0;
            }
            else if (SlNo == -1)
            {

                MPInput = 0;
            }
            else if (SlNo > 0)
            {
                MPInput = SlNo;
            }

        }



    }



    public class StopInfo : EventArgs
    {
        public int slNo;
        public DateTime timestamp;

        public StopInfo(int slNo, DateTime ts)
        {
            this.slNo = slNo;
            timestamp = ts;
        }
    }

}

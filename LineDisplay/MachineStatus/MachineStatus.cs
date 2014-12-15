using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
namespace LineDisplay
{
    public enum MACHINE_STATUS
    {
        UNDEFINED = 0, ACTIVE = 1, SPEED_LOSS, IN_BREAK, OFF, STOPPED,
        NONE
    };
    public partial class MachineStatus : INotifyPropertyChanged
    {
        String name;
        public String Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }
        Project project;
        public Project Project
        {
            get { return project; }
            set
            {
                if (project != null)
                {
                    if (project.ID == value.ID) return;
                    dataAccess.updateProject(MachineID, project.ID, value.ID,currentShift.ID,
                        SessionActual - previousActual,SessionPlan - previousPlan);
                    previousPlan = SessionPlan;
                    previousActual = SessionActual;
                }
                project = value;
                if (value.CycleTime <= 0 ) return;
               
                    cycleTimer.Stop();
                    cycleTimer.Interval = value.CycleTime * 1000;
                    

                    planTimer.Stop();
                    planTimer.Interval = value.CycleTime * 1000;


                    if (Status == MACHINE_STATUS.ACTIVE || Status == MACHINE_STATUS.SPEED_LOSS 
                        || Status == MACHINE_STATUS.STOPPED)
                    {
                        planTimer.Start();
                        cycleTimer.Start();
                    }

                OnPropertyChanged("Project");
            }
        }



        Session currentSession = new Session { ID = -1 };
        public Session CurrentSession
        {
            get { return currentSession; }
            set
            {
                currentSession = value;
                OnPropertyChanged("CurrentSession");
            }
        }


        Shift currentShift = new Shift { ID = -1 };
        public Shift CurrentShift
        {
            get { return currentShift; }
            set
            {
                currentShift = value;
                OnPropertyChanged("CurrentShift");
            }
        }

   

        int sessionActual;
        public int SessionActual
        {
            get { return sessionActual; }
            set
            {
                sessionActual = value;
                OnPropertyChanged("SessionActual");
            }
        }

        int sessionPlan;
        public int SessionPlan
        {
            get { return sessionPlan; }
            set
            {
                sessionPlan = value;
                OnPropertyChanged("SessionPlan");
            }
        }

    

        double sessionOR = 0;
        public double SessionOR
        {
            get { return sessionOR; }
            set
            {
                sessionOR = value;
                OnPropertyChanged("SessionOR");
            }
        }


        int shiftActual;
        public int ShiftActual
        {
            get { return shiftActual; }
            set
            {
                shiftActual = value;
                OnPropertyChanged("ShiftActual");
            }
        }

        int shiftPlan;
        public int ShiftPlan
        {
            get { return shiftPlan; }
            set
            {
                shiftPlan = value;
                OnPropertyChanged("ShiftPlan");
            }
        }



        double shiftOR = 0;
        public double ShiftOR
        {
            get { return shiftOR; }
            set
            {
                shiftOR = value;
                OnPropertyChanged("ShiftOR");
            }
        }

        double redMax = 0;
        public double RedMax
        {
            get { return redMax; }
            set
            {
                redMax = value;
                OnPropertyChanged("RedMax");
            }
        }

        double orangeMin = 0;
        public double OrangeMin
        {
            get { return orangeMin; }
            set
            {
                orangeMin = value;
                OnPropertyChanged("OrangeMin");
            }
        }


        double orangeMax = 0;
        public double OrangeMax
        {
            get { return orangeMax; }
            set
            {
                orangeMax = value;
                OnPropertyChanged("OrangeMax");
            }
        }


        double greenMin = 0;
        public double GreenMin
        {
            get { return greenMin; }
            set
            {
                greenMin = value;
                OnPropertyChanged("GreenMin");
            }
        }


        double greenMax = 0;
        public double GreenMax
        {
            get { return greenMax; }
            set
            {
                greenMax = value;
                OnPropertyChanged("GreenMax");
            }
        }


        MACHINE_STATUS status;
        MACHINE_STATUS previousStatus;
        public MACHINE_STATUS Status
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged("Status");
            }
        }

        int mpInput;
        public int MPInput
        {
            get { return mpInput; }
            set
            {
                mpInput = value;
                OnPropertyChanged("MPInput");

            }
        }

        bool shiftChanged;
        public bool ShiftChanged
        {
            get { return shiftChanged; }
            set
            {
                shiftChanged = value;
                OnPropertyChanged("ShiftChanged");

            }
        }

        #region INotifyPropetyChangedHandler
        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion

        DataAccess dataAccess;

        ShiftCollection Shifts;

        int currentOff = -1;

        int GroupID;
        int MachineID;
        double cycletime;

        double NOT1 = 0;
        double NOT2 = 0;
        double tms = 0;
        DateTime curShiftStart;
        DateTime curShiftEnd;

        DateTime curSessionStart;
        DateTime curSessionEnd;

        double mpInputStartupDuration;
        double stopCloseDuration;
        int previousActual = 0;
        int previousPlan = 0;

        public MachineStatus(DataAccess dataAccess,int machine)
        {
            this.dataAccess = dataAccess;
            MachineID = machine;
            Timer_Init();
            Shifts = dataAccess.getShifts(MachineID);
           Status =  previousStatus = MACHINE_STATUS.NONE;
            
            dataAccess.deleteOpenStops(MachineID);

           // dataAccess.getMachineDetails(MachineID, out n,  out GroupID,
           //     out rm, out omin, out omax, out gmin, out gmax,out tms,
           //     out mpInputStartupDuration,out stopCloseDuration);

           // RedMax = rm;
           // OrangeMin = omin;
           // GreenMax = gmax;
           // OrangeMax = omax;
           // GreenMin = gmin;
           // Name = n;

           Project = new LineDisplay.Project();
           //// cycletime = Project.CycleTime;

           // updateShiftSessions();
  
        }


        void updateShiftSessions()
        {
            Shifts = dataAccess.getShifts(MachineID);
            CurrentShift = Shifts.getCurrentShift();


            CurrentShift.Sessions = dataAccess.getSessions(CurrentShift.ID, MachineID);

            CurrentShift.Breaks = dataAccess.getBreaks(CurrentShift.ID, MachineID);

            CurrentSession = currentShift.Sessions.getCurrentSession();


            if (currentShift.IsActive)
            {
                if (inBreak() == true)
                {
                    Status = MACHINE_STATUS.IN_BREAK;
                    previousStatus = MACHINE_STATUS.ACTIVE;
                }
                else
                {
                    previousStatus = Status = MACHINE_STATUS.ACTIVE;
                }
            }

            SessionActual = 0;


            SessionPlan = 0;



            ShiftActual = 0;

          


            ShiftPlan = 0;


               
        }

        public void SWITCH_OFF(int machineOffCode)
        {
            previousStatus = Status;
            cycleTimer.Stop();
            planTimer.Stop();

            //TimerStop();

            currentOff = dataAccess.insertOff(MachineID, machineOffCode);
            dataAccess.updateOpenStops(MachineID);
            currentStop = null;
            Status = MACHINE_STATUS.OFF;
            
        }

        public void SWITCH_ON()
        {
            dataAccess.updateOff(currentOff);
            currentOff = -1;

            if (CurrentShift.IsActive)
            {
                Status = MACHINE_STATUS.ACTIVE;
            }
            else Status = MACHINE_STATUS.UNDEFINED;
            planTimer.Start();
            cycleTimer.Start();
            //TimerStart();


        }
    }



}

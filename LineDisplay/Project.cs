using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
namespace LineDisplay
{
    public class Project : INotifyPropertyChanged
    {
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


        public int ID { get; set; }
        public String Name { get; set; }
        public double CycleTime { get; set; }

        public Project()
        {
            ID = -1;
            Name = "Return";
            CycleTime = 0;

        }

        public Project(int id, string name, double cycleTime)
        {
            ID = id;
            Name = name;
            CycleTime = cycleTime;
        }

        public override string ToString()
        {
            if (CycleTime > 0)
                return Name + " " + CycleTime.ToString() + "s";
            else return Name;
        }
    }
}

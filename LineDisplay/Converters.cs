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
    [ValueConversion(typeof(MACHINE_STATUS), typeof(Brush))]
    public class statusToBackgroundConv : IValueConverter
    {

        Brush background = Brushes.LimeGreen;
        
        public object Convert(object value, Type targetType, object obj, CultureInfo culInfo)
        {
            if (targetType != typeof(Brush)) return null;
            switch ((MACHINE_STATUS)value)
            {
                case MACHINE_STATUS.ACTIVE: background = Brushes.Lime;
                    break;

                case MACHINE_STATUS.SPEED_LOSS: background = Brushes.Lime;
                    break;

                case MACHINE_STATUS.IN_BREAK: background = Brushes.DeepSkyBlue;
                    break;
                case MACHINE_STATUS.OFF: background= Brushes.DeepSkyBlue;
                    break;

                case MACHINE_STATUS.STOPPED: background = Brushes.Red;
                    break;

                case MACHINE_STATUS.UNDEFINED: background = Brushes.Gray;
                    break;
                default:
                    break;
            }


            return background;
        }


        public object ConvertBack(object value, Type targetType, object obj, CultureInfo culInfo)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(Shift), typeof(String))]
    public class ShiftTolabelConv : IValueConverter
    {
        public object Convert(object value, Type targetType, object obj, CultureInfo culInfo)
        {
            //if (targetType != typeof(String)) return null;

            return  "Actual Shift " +((Shift)value).ToString()+" -" ;


        }


        public object ConvertBack(object value, Type targetType, object obj, CultureInfo culInfo)
        {
            throw new NotImplementedException();
        }
    }
    






    
    public class ORTolabelConv : IMultiValueConverter
    {   
       
        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)values[1] > 0 )
            {
                return "Enter Shift Manpower:";
            }
            else if ((int)values[2] == 0)
            {
                return "";
            }

            else
            {
                if ((double)values[0] == 0)
                    return "0%";
                return ((double)values[0]).ToString("###.#") + "%";
            }
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    [ValueConversion(typeof(double), typeof(String))]
    public class ShfitORTolabelConv : IValueConverter
    {



        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((double)value == 0)
                return "0%";
            return ((double)value).ToString("###.#") + "%";
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(int), typeof(String))]
    public class PlanTolabelConv : IValueConverter
    {
        public object Convert(object value, Type targetType, object obj, CultureInfo culInfo)
        {
            //if (targetType != typeof(String)) return null;

            return "P "+((int)value).ToString();


        }


        public object ConvertBack(object value, Type targetType, object obj, CultureInfo culInfo)
        {
            throw new NotImplementedException();
        }
    }


    [ValueConversion(typeof(int), typeof(String))]
    public class ActualTolabelConv : IValueConverter
    {
        public object Convert(object value, Type targetType, object obj, CultureInfo culInfo)
        {
            //if (targetType != typeof(String)) return null;

            return "A " + ((int)value).ToString();

        }


        public object ConvertBack(object value, Type targetType, object obj, CultureInfo culInfo)
        {
            throw new NotImplementedException();
        }
    }


    [ValueConversion(typeof(Project), typeof(String))]
    public class ProjectTolabelConv : IValueConverter
    {
        public object Convert(object value, Type targetType, object obj, CultureInfo culInfo)
        {
            //if (targetType != typeof(String)) return null;

            return ((Project)value).ToString();

        }


        public object ConvertBack(object value, Type targetType, object obj, CultureInfo culInfo)
        {
            throw new NotImplementedException();
        }
    }


    [ValueConversion(typeof(Session), typeof(String))]
    public class SessionTolabelConv : IValueConverter
    {
        public object Convert(object value, Type targetType, object obj, CultureInfo culInfo)
        {
            //if (targetType != typeof(String)) return null;

            return "Actual Time Interval "+((Session)value).ToString()+" -";

        }


        public object ConvertBack(object value, Type targetType, object obj, CultureInfo culInfo)
        {
            throw new NotImplementedException();
        }
    }


    




}

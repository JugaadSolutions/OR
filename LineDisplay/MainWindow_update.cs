using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using Microsoft.Win32;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;

namespace LineDisplay
{
    public partial class MainWindow : Window
    {
        Timer updateTimer;
        MACHINE_STATUS prevMachineStatus;
        Stop currentStop;
        UISTATE prevUIState;
        UISTATE nextUIState;

        DateTime prevActualTs;

        void Timer_Init()
        {
            updateTimer = new Timer(500);
            updateTimer.AutoReset = false;
            updateTimer.Elapsed += new ElapsedEventHandler(updateTimer_Elapsed);

            updateTimer.Start();

            UITimer = new Timer(3000);
            UITimer.AutoReset = false;
            UITimer.Elapsed += new ElapsedEventHandler(UITimer_Elapsed);

            currentStop = new Stop();
            prevUIState = UISTATE.NONE;
            prevActualTs = DateTime.Now;

            reconnect = new Timer(1000);
            reconnect.AutoReset = false;
            reconnect.Elapsed += reconnect_Elapsed;
        }

        void reconnect_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                reconnect.Stop();
                clientConnection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                System.Net.IPAddress ipaddr = System.Net.IPAddress.Parse(ipAddress);
                clientConnection.BeginConnect(ipaddr, port, ConnectCallBack, clientConnection);
            }
            catch (Exception exp)
            {
                reconnect.Start();
            }
        }

        void UITimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (uiState == nextUIState) return;
           
            switch (uiState)
            {
                case UISTATE.NONE:
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                               new Action(() =>
                               {
                                   uiState = nextUIState;
                                   BaseGrid.Children.Clear();
                                   BaseGrid.Children.Add(machineStatusDisplay);

                               }));
                    break;

                case UISTATE.OFF_TRANSITION:
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                               new Action(() =>
                               {
                                   uiState = nextUIState;
                                   BaseGrid.Children.Clear();
                                   BaseGrid.Children.Add(machineStatusDisplay);

                               }));
                    break;

                case UISTATE.OFF:
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                               new Action(() =>
                               {
                                  uiState = nextUIState;
                                  switch (nextUIState)
                                  {
                                      case UISTATE.MENU:
                                          BaseGrid.Children.Clear();
                                          BaseGrid.Children.Add(menu);
                                          break;

                                         
                                  }

                               }));
                    break;


                case UISTATE.PROJECT:
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                             new Action(() =>
                             {
                                 uiState = nextUIState;
                                 BaseGrid.Children.Clear();
                                 BaseGrid.Children.Add(machineStatusDisplay);
                                 
                             }));
                    break;

                case UISTATE.PROBLEM:
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                               new Action(() =>
                               {
                                   uiState = nextUIState;
                                   BaseGrid.Children.Clear();
                                   BaseGrid.Children.Add(machineStatusDisplay);

                               }));
                    break;

                case UISTATE.STATUS:
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                               new Action(() =>
                               {
                                   BaseGrid.Children.Clear();
                                   BaseGrid.Children.Add(menu);

                                   uiState = nextUIState;

                               }));
                    break;
                case UISTATE.MENU:
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                               new Action(() =>
                               {
                                   uiState = nextUIState;
                                   currentStop.ID = -1;
                                     BaseGrid.Children.Clear();
                                   BaseGrid.Children.Add(machineStatusDisplay);

                                   
                                   

                               }));
                    break;

                case UISTATE.MP_INPUT:
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                               new Action(() =>
                               {
                                   uiState = nextUIState;
                                   
                                   BaseGrid.Children.Clear();
                                   BaseGrid.Children.Add(machineStatusDisplay);

                               }));
                    break;
                default:
                    break;

            }
        
        }
        void updateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
 
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                       new Action(() =>
                       {
                           if (machineStatus.ShiftChanged)
                           {
                               machineStatusDisplay.SessionSmileyProblemGrid.Children.Clear();
                               machineStatusDisplay.textBoxSessionOR.Clear();
                               uiState = UISTATE.STATUS;
                               updateTimer.Start();
                               return;
                           }
                        
                           if (uiState == UISTATE.OFF)
                           {
                               machineStatusDisplay.SessionSmileyProblemGrid.Children.Clear();
                               machineStatusDisplay.SessionSmileyProblemGrid.Children.Add(machineOff);
                               updateTimer.Start();
                               return;
                           }

                           else
                           {
                               if (machineStatus.MPInput > 0)
                               {
                                   uiState = UISTATE.MP_INPUT;
                               }

                               if (uiState == UISTATE.MP_INPUT)
                               {

                                   machineStatusDisplay.SessionSmileyProblemGrid.Children.Clear();
                                   machineStatusDisplay.textBoxSessionOR.FontSize = 80;
                                   machineStatusDisplay.textBoxSessionOR.Foreground = Brushes.White;
                                   machineStatusDisplay.SessionSmileyProblemGrid.Children.Add(manpowerInput);

                               }


                               else
                               {
                                   machineStatusDisplay.textBoxSessionOR.FontSize = 240;

                                   Stop s = dataAccess.getStop(Machine_Id);

                                   if (s != null)
                                   {
                                       if (machineStatus.SessionPlan == 0)
                                       {

                                           machineStatusDisplay.Pointer.Fill = Brushes.White;
                                           machineStatusDisplay.Pointer.Margin = new Thickness(0);
                                           machineStatusDisplay.textBoxSessionOR.Clear();

                                       }
                                       if (currentStop == null)
                                       {
                                           machineStatusDisplay.SessionSmileyProblemGrid.Children.Clear();

                                           problemCode.textBoxStopDuration.Text = "STOP : " + s.From.Value.ToLongTimeString() + " -";
                                           if (s.To != null)
                                               problemCode.textBoxStopDuration.Text += s.To.Value.ToLongTimeString();
                                           problemCode.textBoxProblemCode.Clear();
                                           problemCode.textBoxProblemCode.Focus();
                                           machineStatusDisplay.SessionSmileyProblemGrid.Children.Add(problemCode);
                                           currentStop = s;

                                       }
                                       else
                                       {
                                           if (s.ID != currentStop.ID)
                                           {
                                               machineStatusDisplay.SessionSmileyProblemGrid.Children.Clear();

                                               problemCode.textBoxStopDuration.Text = "STOP : " + s.From.Value.ToLongTimeString() + " -";
                                               if (s.To != null)
                                                   problemCode.textBoxStopDuration.Text += s.To.Value.ToLongTimeString();
                                               problemCode.textBoxProblemCode.Clear();
                                               problemCode.textBoxProblemCode.Focus();
                                               machineStatusDisplay.SessionSmileyProblemGrid.Children.Clear();
                                               machineStatusDisplay.SessionSmileyProblemGrid.Children.Add(problemCode);
                                               currentStop = s;


                                           }
                                           else
                                           {
                                               if (s.To != null)
                                               {
                                                   if (currentStop.To == null)
                                                   {
                                                       currentStop.To = s.To;
                                                       problemCode.textBoxStopDuration.Text += s.To.Value.ToLongTimeString();
                                                   }

                                               }

                                           }
                                       }
                                   }
                                   else
                                   {
                                       currentStop.ID = -1;
                                       if (uiState == UISTATE.STATUS)
                                       {
                                           prevUIState = uiState;
                                           if (machineStatus.SessionPlan == 0)
                                           {
                                               machineStatusDisplay.SessionSmileyProblemGrid.Children.Clear();
                                               machineStatusDisplay.Pointer.Fill = Brushes.White;
                                               machineStatusDisplay.Pointer.Margin = new Thickness(0);
                                               machineStatusDisplay.textBoxSessionOR.Clear();

                                           }
                                           else
                                           {
                                               if (machineStatus.SessionOR < machineStatus.RedMax)
                                               {
                                                   machineStatusDisplay.SessionSmileyProblemGrid.Children.Clear();
                                                   machineStatusDisplay.SessionSmileyProblemGrid.Children.Add(redSmiley);
                                               }
                                               else if (machineStatus.SessionOR >= machineStatus.RedMax && machineStatus.SessionOR < machineStatus.OrangeMax)
                                               {
                                                   machineStatusDisplay.SessionSmileyProblemGrid.Children.Clear();
                                                   machineStatusDisplay.SessionSmileyProblemGrid.Children.Add(orangeSmiley);


                                               }
                                               else if (machineStatus.SessionOR >= machineStatus.OrangeMax && machineStatus.SessionOR <= 100)
                                               {
                                                   machineStatusDisplay.SessionSmileyProblemGrid.Children.Clear();
                                                   machineStatusDisplay.SessionSmileyProblemGrid.Children.Add(greenSmiley);

                                               }
                                               else
                                               {

                                                   machineStatusDisplay.SessionSmileyProblemGrid.Children.Clear();


                                               }
                                           }
                                       }

                                   }



                                   if (machineStatus.SessionOR < machineStatus.RedMax && machineStatus.SessionPlan > 0)
                                   {
                                       machineStatusDisplay.textBoxSessionOR.FontSize = 240;
                                       machineStatusDisplay.textBoxSessionOR.Foreground = Brushes.Red;
                                       machineStatusDisplay.Pointer.Fill = Brushes.Red;

                                   }
                                   else if (machineStatus.SessionOR >= machineStatus.RedMax && machineStatus.SessionOR < machineStatus.OrangeMax)
                                   {
                                       machineStatusDisplay.textBoxSessionOR.FontSize = 240;
                                       machineStatusDisplay.textBoxSessionOR.Foreground = Brushes.OrangeRed;
                                       machineStatusDisplay.Pointer.Fill = Brushes.OrangeRed;
                                   }
                                   else if (machineStatus.SessionOR >= machineStatus.OrangeMax && machineStatus.SessionOR <= 100)
                                   {
                                       machineStatusDisplay.textBoxSessionOR.FontSize = 240;
                                       machineStatusDisplay.textBoxSessionOR.Foreground = Brushes.Lime;
                                       machineStatusDisplay.Pointer.Fill = Brushes.Lime;
                                   }
                                   else if (machineStatus.SessionPlan > 0)
                                   {
                                       machineStatusDisplay.textBoxSessionOR.FontSize = 240;
                                       machineStatusDisplay.textBoxSessionOR.Foreground = Brushes.White;
                                       machineStatusDisplay.Pointer.Fill = Brushes.White;
                                   }

                               }
                               if (machineStatus.ShiftOR < machineStatus.RedMax)
                               {

                                   machineStatusDisplay.textBoxShiftOR.Foreground = Brushes.Red;
                                   machineStatusDisplay.ShiftSmileyGrid.Children.Clear();
                                   machineStatusDisplay.ShiftSmileyGrid.Children.Add(smallRedSmiley);

                               }
                               else if (machineStatus.ShiftOR >= machineStatus.RedMax && machineStatus.ShiftOR < machineStatus.OrangeMax)
                               {
                                   machineStatusDisplay.textBoxShiftOR.Foreground = Brushes.OrangeRed;
                                   machineStatusDisplay.ShiftSmileyGrid.Children.Clear();
                                   machineStatusDisplay.ShiftSmileyGrid.Children.Add(smallOrangeSmiley);
                               }
                               else if (machineStatus.ShiftOR >= machineStatus.OrangeMax && machineStatus.ShiftOR <= 100)
                               {
                                   machineStatusDisplay.textBoxShiftOR.Foreground = Brushes.Lime;
                                   machineStatusDisplay.ShiftSmileyGrid.Children.Clear();
                                   machineStatusDisplay.ShiftSmileyGrid.Children.Add(smallGreenSmiley);
                               }
                               else
                               {
                                   machineStatusDisplay.textBoxShiftOR.Foreground = Brushes.White;
                                   machineStatusDisplay.ShiftSmileyGrid.Children.Clear();
                               }
                               machineStatusDisplay.ORIndicatorGrid.ColumnDefinitions[0].Width = new GridLength(machineStatus.RedMax, GridUnitType.Star);
                               machineStatusDisplay.ORIndicatorGrid.ColumnDefinitions[1].Width = new GridLength(machineStatus.OrangeMax - machineStatus.RedMax, GridUnitType.Star);
                               machineStatusDisplay.ORIndicatorGrid.ColumnDefinitions[2].Width = new GridLength(machineStatus.GreenMax - machineStatus.OrangeMax, GridUnitType.Star);

                               double iw = machineStatusDisplay.ORIndicatorGrid.ActualWidth;

                               if (machineStatus.SessionOR <= 100)
                               {
                                   machineStatusDisplay.Pointer.Margin = new Thickness((machineStatus.SessionOR * iw) / 100, 0, 0, 0);
                               }
                               else
                                   machineStatusDisplay.Pointer.Margin = new Thickness((101 * iw) / 100, 0, 0, 0);

                           }
                           machineStatusDisplay.textBoxTime.Text = DateTime.Now.ToString("HH:mm");

                           machineStatusDisplay.textBoxTime1.Text = DateTime.Now.ToString("HH:mm");
                           machineStatusDisplay.textBoxTime2.Text = DateTime.Now.ToString("HH:mm");

                           updateTimer.Start();
                       }));
        }


        void updateInput()
        {
            DateTime ts = DateTime.Now;
            TimeSpan duration =ts - prevActualTs ;
           // if (duration.TotalSeconds < 75)
           //    return;
            
            
            dataAccess.updateMachineInput( Machine_Id, ts);
            machineStatus.updateActual();
            prevActualTs = ts;
        }


        void updateStop(DateTime ts)
        {
            
                dataAccess.updateStop_To();
                
         
        }

    }

   
}

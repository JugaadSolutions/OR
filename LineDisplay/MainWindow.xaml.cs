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
using System.Configuration;
using System.Net.Sockets;
using LineDisplay.UI;

namespace LineDisplay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    enum UISTATE { NONE, STATUS, MENU, PROJECT, OFF, PROBLEM,OFF_TRANSITION,MP_INPUT };
    public partial class MainWindow : Window
    {
        
        MachineStatus machineStatus;
        int Machine_Id;


        MachineStatusDisplay machineStatusDisplay;
        DisplayMenu menu;
        ProjectSelection projectSelection;
        ProblemCode problemCode;
        GreenSmiley greenSmiley;
        OrangeSmiley orangeSmiley;
        RedSmiley redSmiley;
        
        GreenSmiley_small smallGreenSmiley;
        RedSmiley_small smallRedSmiley;
        OrangeSmiley_small smallOrangeSmiley;

        MachineOff_Failure machineOffFailure;
        MachineOff_Success machineOffSuccess;
        MachineOffInput machineOffInput;
        MachineOff machineOff;
        ManPowerInput manpowerInput;

        ProblemCode_Failure problemCodeFailure;
        ProblemCode_Success problemCodeSuccess;

        SelectedProject selectedProject;

        DataAccess dataAccess;
        double redPercentage, orangePercentage, greenPercentage;
        UISTATE uiState;

        Timer UITimer;

        Socket clientConnection;
        int inputIndex = -1;
        String ipAddress = string.Empty;
        int port = 0;
        Timer reconnect;
        int currentKeyState = -1;
        int prevKeyState = -1;

        int ON_STATE = -1;
        int OFF_STATE = -1;

        public MainWindow()
        {
           
            try
            {
                InitializeComponent();


                //MessageBox.Show("Started App", "Info", MessageBoxButton.OK);
                dataAccess = new DataAccess();

                //MessageBox.Show("DataBase Connected", "Info", MessageBoxButton.OK);

                Machine_Id = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Machine"]);
                machineStatus = new MachineStatus(dataAccess, Machine_Id);

                machineStatus.CloseStopEvent += machineStatus_CloseStopEvent;


                OFF_STATE = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["OFF_STATE"]);
                ON_STATE = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ON_STATE"]);
                if (currentKeyState == 1)
                    prevKeyState = 0;
                else
                    prevKeyState = 1;

                machineStatusDisplay = new MachineStatusDisplay();
                menu = new DisplayMenu();

                projectSelection = new ProjectSelection();
                selectedProject = new SelectedProject();


                greenSmiley = new GreenSmiley();
                orangeSmiley = new OrangeSmiley();
                redSmiley = new RedSmiley();

                smallGreenSmiley = new GreenSmiley_small();
                smallOrangeSmiley = new OrangeSmiley_small();
                smallRedSmiley = new RedSmiley_small();


                machineOffInput = new MachineOffInput();
                machineOffFailure = new MachineOff_Failure();
                machineOffSuccess = new MachineOff_Success();
                machineOff = new MachineOff();

                manpowerInput = new ManPowerInput();

                problemCode = new ProblemCode();
                problemCodeFailure = new ProblemCode_Failure();
                problemCodeSuccess = new ProblemCode_Success();

               


                machineStatusDisplay.ORIndicatorGrid.ColumnDefinitions[0].Width = new GridLength(machineStatus.RedMax, GridUnitType.Star);
                machineStatusDisplay.ORIndicatorGrid.ColumnDefinitions[1].Width = new GridLength(machineStatus.OrangeMax-machineStatus.RedMax, GridUnitType.Star);
                machineStatusDisplay.ORIndicatorGrid.ColumnDefinitions[2].Width = new GridLength(machineStatus.GreenMax-machineStatus.OrangeMax, GridUnitType.Star);
               

                //MessageBox.Show("Timer Initialized", "Info", MessageBoxButton.OK);
                machineStatusDisplay.DataContext = machineStatus;
                uiState = UISTATE.STATUS;
                BaseGrid.Children.Add(machineStatusDisplay);


                simulation = ConfigurationSettings.AppSettings["SIMULATION"];
                inputIndex = Convert.ToInt32(ConfigurationSettings.AppSettings["INPUT_INDEX"]);

                if (simulation != "Yes")
                {
                    try
                    {
                        ipAddress = ConfigurationSettings.AppSettings["IP"];
                        port = Convert.ToInt32(ConfigurationSettings.AppSettings["PORT"]);
                        clientConnection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        System.Net.IPAddress ipaddr = System.Net.IPAddress.Parse(ipAddress);
                        clientConnection.BeginConnect(ipaddr, port, ConnectCallBack, clientConnection);
                    }
                    //MessageBox.Show("Attempting connection to Panel", "Info", MessageBoxButton.OK);

                    catch (Exception e)
                    {
                        reconnect.Start();
                    }
                }
                else
                {
                    //machineStatus.Status = MACHINE_STATUS.ACTIVE;
                    machineStatusDisplay.IsEnabled = true;
                   MessageBox.Show("Starting Simulation", "Info", MessageBoxButton.OK);
                }
                
                machineStatus.TimerStart();
                Timer_Init();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message +Environment.NewLine+ e.StackTrace, "Error", MessageBoxButton.OK);
            }

        }

        void machineStatus_CloseStopEvent(object sender, StopInfo e)
        {
            if (currentStop.ID == e.slNo)
                currentStop.ID = -1;
        }

       
    }
}

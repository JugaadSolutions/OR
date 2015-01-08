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

namespace LineDisplay
{
    public partial class MainWindow : Window
    {
        Uri stopSymbol = new Uri(@"..\Images\stop.png", UriKind.RelativeOrAbsolute);
        Uri cancelStopSymbol = new Uri(@"..\Images\cancelstop.png",UriKind.RelativeOrAbsolute);
        String simulation = String.Empty;

        String[] MenuItems_ON = { "Project Change", "Machine Off", "Return" };
        String[] MenuItems_OFF = { "Project Change", "Cancel Machine Off", "Return" };

        int currentOFF = -1;
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            
            
            
            if (e.Key == Key.Escape)
            {
                this.WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
                return;
            }

            switch (uiState)
            {
                case UISTATE.STATUS:
                    {
                        switch (e.Key)
                        {
                            case Key.Oem2:
                            case Key.Divide:
                                menu.listBoxMenuSelection.SelectedIndex = 0;
                                BaseGrid.Children.Clear();
                                BaseGrid.Children.Add(menu);

                                menu.LoadMenuList(MenuItems_ON);
                                prevUIState = uiState;
                                uiState = UISTATE.MENU;
                                
                            
                            
                                break;
                            case Key.F9:
                                if (simulation == "Yes")
                                {
                                    updateInput();
                                    
                                }
                                break;
                            default: break;
                        }

                        if (e.Key == Key.Enter && currentStop.ID != -1)
                        {
                            if (problemCode.textBoxProblemCode.Text == String.Empty)
                            {
                                return;
                            }
                            else
                            {
                                int code;
                                if( int.TryParse(problemCode.textBoxProblemCode.Text,out code) == false )
                                {
                                    problemCode.textBoxProblemCode.Clear();
                                    return;
                                }
                                String OffReason = dataAccess.getStopProblem(code, 3, Machine_Id);
                                if (OffReason == String.Empty)
                                {

                                    problemCodeFailure.labelOffCode.Text = problemCode.textBoxProblemCode.Text;
                                    BaseGrid.Children.Clear();
                                    BaseGrid.Children.Add(problemCodeFailure);
                                    
                                    uiState = UISTATE.NONE;
                                    UITimer.Start();
                                }
                                else
                                {
                                    problemCodeSuccess.labelOffCode.Text = problemCode.textBoxProblemCode.Text;
                                    problemCodeSuccess.labelOffReason.Text = OffReason;
                                    currentStop.Code = Convert.ToInt32(problemCode.textBoxProblemCode.Text);
                                    currentStop.To = DateTime.Now;
                                    currentStop.Status = "Acknowledged";
                                    
                                    dataAccess.updateStop(currentStop);
                                    currentStop.timeoutTimer.Stop();
                                    BaseGrid.Children.Clear();
                                    BaseGrid.Children.Add(problemCodeSuccess);
                                    currentStop.ID = -1;
                                    uiState = UISTATE.NONE;
                                    UITimer.Start();
                                }
                                nextUIState = UISTATE.STATUS;
                            }
                        }
                        break;



                    }

                case UISTATE.MENU:
                    if (e.Key == Key.Enter)
                    {
                        switch (menu.listBoxMenuSelection.SelectedIndex)
                        {
                            case 0:
                                BaseGrid.Children.Clear();
                                List<Project> pl = dataAccess.getProjectList(Machine_Id);
                                projectSelection.ProjectList = pl;
                                projectSelection.listBoxMenuSelection.SelectedIndex = 0;
                                BaseGrid.Children.Add(projectSelection);
                                uiState = UISTATE.PROJECT;

                                break;
                            case 1:
                                BaseGrid.Children.Clear();
                                BaseGrid.Children.Add(machineOffInput);
                                if (machineStatus.Status != MACHINE_STATUS.OFF)
                                {
                                    machineOffInput.ImageSrc = stopSymbol;
                                    machineOffInput.textBoxProblemCode.Clear();
                                    machineOffInput.textBoxProblemCode.Focus();
                                    machineOffInput.InputPanel.Visibility = System.Windows.Visibility.Visible;
                                    uiState = UISTATE.OFF_TRANSITION;
                                }
                                else if (machineStatus.Status == MACHINE_STATUS.OFF)
                                {
                                    machineOffInput.ImageSrc = cancelStopSymbol;
                                    machineOffInput.InputPanel.Visibility = System.Windows.Visibility.Collapsed;
                                    machineStatus.SWITCH_ON();
                                    nextUIState = UISTATE.STATUS;
                                    UITimer.Start();
                                }

                              

                                break;
                            case 2:
                                BaseGrid.Children.Clear();
                                BaseGrid.Children.Add(machineStatusDisplay);
                                switch (prevUIState)
                                {
                                    case UISTATE.STATUS:
                                        BaseGrid.Children.Clear();
                                        BaseGrid.Children.Add(machineStatusDisplay);
                                        break;
                                }
                                uiState = prevUIState;
                                break;


                        }
                       
                    }
                    else if (e.Key == Key.Back)
                    {
                        switch (prevUIState)
                        {
                            case UISTATE.STATUS:
                                BaseGrid.Children.Clear();
                                BaseGrid.Children.Add(machineStatusDisplay);
                                uiState = prevUIState;
                                break;
                        }
                    }


                    else if (e.Key == Key.OemPlus || e.Key == Key.Add)
                    {
                        menu.listBoxMenuSelection.SelectedIndex += 1;
                        if (menu.listBoxMenuSelection.SelectedIndex >=
                            menu.listBoxMenuSelection.Items.Count)
                            menu.listBoxMenuSelection.SelectedIndex = 0;
                    }

                    else if (e.Key == Key.OemMinus || e.Key == Key.Subtract)
                    {
                        if (menu.listBoxMenuSelection.SelectedIndex > 0)
                            menu.listBoxMenuSelection.SelectedIndex -= 1;


                    }

                    break;

                case UISTATE.PROJECT:
                    if (e.Key == Key.Enter)
                    {
                        if (projectSelection.listBoxMenuSelection.SelectedIndex !=
                            projectSelection.listBoxMenuSelection.Items.Count - 1)
                        {
                            machineStatus.Project =
                                projectSelection.ProjectList[projectSelection.listBoxMenuSelection.SelectedIndex];
                        }
                        BaseGrid.Children.Clear();
                        
                        selectedProject.SelectedProjectTB.Text = machineStatus.Project.Name +"-"
                            + machineStatus.Project.CycleTime.ToString() + "s";
                        BaseGrid.Children.Add(selectedProject);
                        
                        nextUIState = prevUIState;
                        UITimer.Start();
                        return;
                    }
                    else if (e.Key == Key.Back)
                    {
                        BaseGrid.Children.Clear();
                        BaseGrid.Children.Add(menu);
                        uiState = UISTATE.MENU;
                    }

                    else if (e.Key == Key.OemPlus || e.Key == Key.Add)
                    {
                        projectSelection.listBoxMenuSelection.SelectedIndex += 1;
                        if (projectSelection.listBoxMenuSelection.SelectedIndex >=
                            projectSelection.listBoxMenuSelection.Items.Count)
                            projectSelection.listBoxMenuSelection.SelectedIndex = 0;
                    }

                    else if (e.Key == Key.OemMinus || e.Key == Key.Subtract)
                    {
                        if (projectSelection.listBoxMenuSelection.SelectedIndex > 0)
                            projectSelection.listBoxMenuSelection.SelectedIndex -= 1;
                    }

                    break;

                case UISTATE.OFF_TRANSITION:
                    switch (e.Key)
                    {
  
                        case Key.Back:
                            BaseGrid.Children.Clear();
                            BaseGrid.Children.Add(menu);
                            uiState = UISTATE.MENU;
                            break;

                    }
                    if (e.Key == Key.Enter)
                    {


                        if (machineStatus.Status != MACHINE_STATUS.OFF)
                        {
                            if (machineOffInput.textBoxProblemCode.Text == String.Empty)
                            {
                                return;
                            }

                            int machineOffCode;

                            if (int.TryParse(machineOffInput.textBoxProblemCode.Text, out machineOffCode) == false)
                            {
                                machineOffInput.textBoxProblemCode.Clear();
                                return;
                            }

                            String OffReason = dataAccess.getProblem(
                               machineOffCode , 3, Machine_Id);
                            if (OffReason == String.Empty)
                            {

                                machineOffFailure.labelOffCode.Text = machineOffInput.textBoxProblemCode.Text;
                                BaseGrid.Children.Clear();
                                BaseGrid.Children.Add(machineOffFailure);
                                nextUIState = prevUIState;
                                UITimer.Start();
                            }
                            else
                            {

                                machineOffSuccess.ImageSrc = stopSymbol;
                                machineOffSuccess.labelOffCode.Text = machineOffInput.textBoxProblemCode.Text;
                                machineOffSuccess.labelOffReason.Text = OffReason;

                                machineStatus.SWITCH_OFF(machineOffCode);
                               
                                currentStop.ID = -1;
                                currentStop.timeoutTimer.Stop();
                                BaseGrid.Children.Clear();
                                BaseGrid.Children.Add(machineOffSuccess);

                                nextUIState = UISTATE.OFF;

                            }
                        }
                        else if (machineStatus.Status == MACHINE_STATUS.OFF)
                        {
                            machineOffSuccess.ImageSrc = cancelStopSymbol;
                            machineOffSuccess.OffCodePanel.Visibility = System.Windows.Visibility.Collapsed;
                            nextUIState = UISTATE.STATUS;
                            
                            machineStatus.SWITCH_ON();
                        }

                        UITimer.Start();

                    }

                    break;

                case UISTATE.OFF:
                    
                        switch (e.Key)
                        {
                            case Key.Oem2:

                            case Key.Divide:
                                menu.listBoxMenuSelection.SelectedIndex = 0;
                                BaseGrid.Children.Clear();
                                menu.LoadMenuList(MenuItems_OFF);
                                BaseGrid.Children.Add(menu);
                                prevUIState = uiState;
                                uiState = UISTATE.MENU;
                                break;

                            case Key.F9:
                                if (simulation == "Yes")
                                {
                                    updateInput();

                                }
                                break;
                        }
                        break;

                case UISTATE.MP_INPUT:

                     switch (e.Key)
                        {
                            case Key.Oem2:

                            case Key.Enter:
                                int mp;
                                if (int.TryParse(manpowerInput.textBoxProblemCode.Text, out mp) == false)
                                {
                                    manpowerInput.textBoxProblemCode.Clear();
                                    return;
                                }
                                dataAccess.updateManpower(machineStatus.MPInput,mp);
                                machineStatus.updateManPower();
                                machineStatusDisplay.textBoxSessionOR.FontSize = 240;
                                machineStatusDisplay.SessionSmileyProblemGrid.Children.Clear();
                                uiState  = UISTATE.STATUS;
                                currentStop.ID = -1;
                                currentStop.timeoutTimer.Stop();
                               // UITimer.Start();
                                break;
                            case Key.F9:
                                if (simulation == "Yes")
                                {
                                    updateInput();

                                }
                                break;
                        }
                
                        break;



                


            }


        }
       


    }
}

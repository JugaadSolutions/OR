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
using System.Configuration;
using System.Timers;

namespace LineDisplay
{
    public partial class MainWindow: Window
    {
        
        
        Byte[] rxBuffer = new byte[1];

        Byte[] txBuffer = new byte[1];
        public void ConnectCallBack(IAsyncResult result)
        {
            try
            {
                clientConnection.EndConnect(result);
                txBuffer[0] =(Byte) Convert.ToInt32(ConfigurationSettings.AppSettings["INPUT_KEY"]);
                  this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                               new Action(() =>
                               {
                                    machineStatusDisplay.IsEnabled = true;
                               }));
                  
                  
                  //txBuffer[0] = 2;
                  
                  clientConnection.BeginReceive(rxBuffer, 0, 1, SocketFlags.None, ReceiveCallBack, clientConnection);
                  clientConnection.Send(txBuffer);

                  
            }
            catch (Exception e)
            {
                reconnect.Start();
            }
        }

      


        private void ReceiveCallBack(IAsyncResult result)
        {
            try
            {
                int bytesreceived = clientConnection.EndReceive(result);

                if (rxBuffer[0] != currentKeyState)
                {
                    if (currentKeyState == OFF_STATE)
                    {
                        updateInput();
                    }
                        
                   
                    currentKeyState = rxBuffer[0];
                    
                    
                    
                }

                clientConnection.ReceiveTimeout = -1;
                clientConnection.Send(txBuffer);
                clientConnection.BeginReceive(rxBuffer, 0, 1, SocketFlags.None,
                    ReceiveCallBack, clientConnection);
            }
            catch (Exception e)
            {
                rxBuffer[0] = 0;
                clientConnection.Close();
                reconnect.Start();
            }


        }

        private void SendCallBack(IAsyncResult result)
        {
            try
            {
                clientConnection.EndSend(result);
            }
            catch (Exception e)
            {
                return;
            }
        }

                

               
        
    }
}

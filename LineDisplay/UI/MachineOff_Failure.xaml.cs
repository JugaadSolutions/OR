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

namespace LineDisplay
{
    /// <summary>
    /// Interaction logic for MachineOff_Failure.xaml
    /// </summary>
    public partial class MachineOff_Failure : UserControl
    {
       

        public MachineOff_Failure()
        {
            InitializeComponent();
            
        }
        
        public MachineOff_Failure(String offCode)
        {
            InitializeComponent();
            labelOffCode.Text = offCode;
        }
    }
}

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
    /// Interaction logic for ProblemCode_Success.xaml
    /// </summary>
    public partial class ProblemCode_Success : UserControl
    {
        public ProblemCode_Success()
        {
            InitializeComponent();
        }


        public ProblemCode_Success(String offcode, String offreason)
        {
            InitializeComponent();
            labelOffCode.Text = offcode;
            labelOffReason.Text = offreason;

        }
    }
}

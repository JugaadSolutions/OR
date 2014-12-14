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
    /// Interaction logic for ProblemCode_Failure.xaml
    /// </summary>
    public partial class ProblemCode_Failure : UserControl
    {
        public ProblemCode_Failure()
        {
            InitializeComponent();
        }

        public ProblemCode_Failure(String offCode)
        {
            InitializeComponent();
            labelOffCode.Text = offCode;
        }
    }
}

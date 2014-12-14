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
    /// Interaction logic for MachineOff_Success.xaml
    /// </summary>
    public partial class MachineOff_Success : UserControl
    {
        Uri imageSource;
        public Uri ImageSrc
        {
            get { return imageSource; }
            set
            {
                if (value == null)
                {
                    this.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    imageSource = value;
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = imageSource;
                    image.EndInit();
                    Symbol.Source = image;
                }
            }
        }

        public MachineOff_Success()
        {
            InitializeComponent();
        }
        public MachineOff_Success(String offcode, String offreason)
        {
            InitializeComponent();
            labelOffCode.Text = offcode;
            labelOffReason.Text = offreason;

        }


    }
}

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
    /// Interaction logic for MachineOff.xaml
    /// </summary>
    public partial class MachineOffInput : UserControl
    {
        Uri imageSource;
        bool input = false;
        public Uri ImageSrc
        {
            get { return imageSource; }
            set
            {
                imageSource = value;
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = imageSource;
                image.EndInit();
                Symbol.Source = image;
            }
        }


        public MachineOffInput()
        {
            InitializeComponent();
          

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            textBoxProblemCode.Clear();
                textBoxProblemCode.Focus();

        }



     
    }
}

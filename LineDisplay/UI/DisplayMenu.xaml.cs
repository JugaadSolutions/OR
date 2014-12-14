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
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class DisplayMenu : UserControl
    {
        public DisplayMenu()
        {
            InitializeComponent();
        }

        private void listBoxMenuSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach( ListViewItem l in listBoxMenuSelection.Items )
            {
                l.Background = Brushes.White;
            }
            foreach( ListViewItem l in e.AddedItems )
            {
                l.Background = Brushes.Blue;
                
            }

            
            
        }

        public void LoadMenuList(String[] menuitems )
        {
            listBoxMenuSelection.Items.Clear();
            foreach( String s in menuitems )
            {
                ListViewItem lv = new ListViewItem();
                lv.Content = s;
                listBoxMenuSelection.Items.Add(lv);
            }
        }

        private void listBoxMenuSelection_Loaded(object sender, RoutedEventArgs e)
        {
            this.listBoxMenuSelection.SelectedIndex = 0;
        }

        
    }
}

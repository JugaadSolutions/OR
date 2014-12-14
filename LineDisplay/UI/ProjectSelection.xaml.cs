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
    /// Interaction logic for ProjectSelection.xaml
    /// </summary>
    public partial class ProjectSelection : UserControl
    {
        List<Project> projectList;
        public List<Project> ProjectList
        {
            get { return projectList; }
            set
            {
                projectList = value;
                listBoxMenuSelection.DataContext = projectList;
            }
        }
        public ProjectSelection()
        {
            InitializeComponent();
            projectList = new List<Project>();


        }



        public ProjectSelection(List<Project> projectList)
        {
            InitializeComponent();


            ProjectList = projectList;
            listBoxMenuSelection.DataContext = ProjectList;



        }

        private void listBoxMenuSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //foreach (ListViewItem l in listBoxMenuSelection.Items)
            //{
            //    l.Background = Brushes.White;
            //}

            //((ListViewItem)listBoxMenuSelection.SelectedItem).Background = Brushes.Blue;
        }
    }
}

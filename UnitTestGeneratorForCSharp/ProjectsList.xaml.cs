using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System.ComponentModel;
namespace UnitTestGeneratorForCSharp
{

    public class CheckBoxData : INotifyPropertyChanged
    {
       public Project project { get; set; }
        private bool isselected=true;
       public bool isSelected
        {
            get { return isselected; }
            set
            {
                isselected = value;
                NotifyPropertyChanged("isSelected");
            }
        }

        public CheckBoxData(Project project)
        {
            this.project = project;            
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string strPropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(strPropertyName));
        }
    }
    /// <summary>
    /// Interaction logic for ProjectsList.xaml
    /// </summary>
    public partial class ProjectsList : Window
    {
        List<CheckBoxData> checkboxes = new List<CheckBoxData>();
        public ProjectsList(string solutionpath)
        {
            InitializeComponent();
            var workspace = MSBuildWorkspace.Create();
            var solution = workspace.OpenSolutionAsync(solutionpath).Result;
            var projects = solution.Projects;
            
            foreach(var project in projects)
            {
                checkboxes.Add(new CheckBoxData(project));
            }
            lbxProjects.DataContext = checkboxes;    
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            List<Project> selectedProjects = new List<Project>();
            foreach(var item in checkboxes)
            {
                if(item.isSelected)
                {
                    selectedProjects.Add(item.project);
                }
            }
            if (selectedProjects.Count == 0)
                lblCaption.Visibility = Visibility.Visible;
            else
            {
                lblCaption.Visibility = Visibility.Hidden;
                Window unitsList = new FilesList(selectedProjects);
                unitsList.Show();
                this.Hide();
            }
        }
    }
}

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
using Microsoft.CodeAnalysis.CSharp;
using System.ComponentModel;
namespace UnitTestGeneratorForCSharp
{

    public class FilesCheckBox : INotifyPropertyChanged
    {
        public Document CodeFile { get; set; }
        private bool isselected = true;
        public bool isSelected
        {
            get { return isselected; }
            set
            {
                isselected = value;
                NotifyPropertyChanged("isSelected");                                
            }
            
        }
        public FilesCheckBox(Document d)
        {
            CodeFile = d;           
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string strPropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(strPropertyName));
        }
    }

    public class ProjectsListBox 
    {
        public Project project { get; set; }
        private List<FilesCheckBox> units = new List<FilesCheckBox>();

        public List<FilesCheckBox> Units
        {
            get { return units; }
        }
        public ProjectsListBox(Project project)
        {
            this.project = project;       
            foreach(var d in project.Documents)
            {
                units.Add(new FilesCheckBox(d));
            }
        }        
    }

    /// <summary>
    /// Interaction logic for UnitsList.xaml
    /// </summary>
    public partial class FilesList : Window
    {
        List<ProjectsListBox> projectsList = new List<ProjectsListBox>();
        public FilesList(List<Project> selectedProjects)
        {
            InitializeComponent();
            foreach(var p in selectedProjects)
            {
                projectsList.Add(new ProjectsListBox(p));
            }
            lbxProjects.ItemsSource = projectsList;
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            List<Document> selectedFiles = new List<Document>();
            foreach(var project in projectsList)
            {
                foreach(var unit in project.Units)
                {
                    if (unit.isSelected)
                        selectedFiles.Add(unit.CodeFile);
                }
            }            
            Window progress = new Progess(selectedFiles, chkUserInput.IsChecked.Value);
            progress.Show();
            this.Hide();
        }
    }
}

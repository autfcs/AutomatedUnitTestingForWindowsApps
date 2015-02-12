using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UnitTestGeneratorForCSharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            string path = txtSolutionPath.Text;
            if (!path.EndsWith(".sln"))
            {
                lblErrorMessage.Content = "Enter a valid path to a solution file";
                lblErrorMessage.Visibility = Visibility.Visible;
            }
            else if (!File.Exists(path))
            {
                lblErrorMessage.Content = "The specified solution file does not exist";
                lblErrorMessage.Visibility = Visibility.Visible;
            }
            else
            {
                lblErrorMessage.Visibility = Visibility.Hidden;
                Window projectsList = new ProjectsList(path);
                this.Hide();
                projectsList.Show();
            }
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".sln";
            dlg.Filter = "Solution Files (*.sln)|*.sln";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                txtSolutionPath.Text = filename;
            }
        }
    }
}

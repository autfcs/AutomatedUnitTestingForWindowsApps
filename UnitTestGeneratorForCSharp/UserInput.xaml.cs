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
using System.IO;
using Microsoft.CodeAnalysis;
using UnitTestGeneratorForCSharp.CodeAnalysis;
namespace UnitTestGeneratorForCSharp
{
    /// <summary>
    /// Interaction logic for UserInput.xaml
    /// </summary>
    public partial class UserInput : Window
    {
        Progess progress;
        public UserInput(Progess progressWindow)
        {
            InitializeComponent();
            progress = progressWindow;
            textBlock.Text= progress.xmlInput;
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {           
            this.Hide();
            progress.Show();
            File.WriteAllText("F:\\xml.txt", textBlock.Text);
            TestCodeGenerator generator = new TestCodeGenerator();
            foreach (Document file in progress.selectedFiles)
            {
                progress.lblStatus.Content = "Generating unit test for " + file.Name;
                string status = generator.GenerateUnitTest(file);
                if (!status.Equals("Success"))
                    MessageBox.Show(status);
            }
            progress.lblStatus.Content = "Finished";
        }
    }
}

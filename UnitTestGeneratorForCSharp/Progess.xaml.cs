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
    /// Interaction logic for Progess.xaml
    /// </summary>
    public partial class Progess : Window
    {
        public String xmlInput;
        public List<Document> selectedFiles;        
        public Progess(List<Document> selectedFiles,bool IsUserInput)
        {
            InitializeComponent();
            this.selectedFiles = selectedFiles;
            if(IsUserInput)
            {
                this.Hide();
                InputXmlBuilder xmlbuilder = new InputXmlBuilder();
                foreach(Document file in selectedFiles)
                {
                    xmlInput=xmlbuilder.CreateInputXML(file);
                    File.AppendAllText("F:\\xml.txt", xmlInput);
                }
                Window input = new UserInput(this);
                input.Show();                               
            }
            else
            {
                TestCodeGenerator generator = new TestCodeGenerator();
                foreach (Document file in selectedFiles)
                {
                    lblStatus.Content = "Generating unit test for " + file.Name;
                    string status = generator.GenerateUnitTest(file);
                    if (!status.Equals("Success"))
                        MessageBox.Show(status);
                }
                lblStatus.Content = "Finished";
            }
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Extensions;
namespace UnitTestGeneratorForCSharp.CodeAnalysis
{
    class InputXmlBuilder
    {
        String codeString;
        SyntaxTree codeTree;
        SemanticModel codeModel;
        Compilation codeCompilation;
        Document doc;
        StringBuilder xmlbuilder = new StringBuilder();
        private void ReadSourceCode(string filename)
        {
            codeString = File.ReadAllText(filename);
        }
        private void CreateCodeModel(Document doc)
        {
            codeTree = CSharpSyntaxTree.ParseText(codeString);
            var compilation = CSharpCompilation.Create("source").AddSyntaxTrees(codeTree);
            codeModel = compilation.GetSemanticModel(codeTree);
            codeCompilation = doc.Project.GetCompilationAsync().Result;
            this.doc = doc;
        }
        public void GenerateXML()
        {
            xmlbuilder.AppendFormat("<program name={0}> \n", doc.Name);
            IEnumerable<ClassDeclarationSyntax> classes = codeTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
            foreach (var classname in classes)
            {
                xmlbuilder.AppendFormat("<class name={0}> \n", classname.Identifier);
                IEnumerable<MethodDeclarationSyntax> methods = classname.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();
                foreach (var method in methods)
                {
                    if (!method.Identifier.ToString().Equals("Main"))
                    {
                        
                        var methodInfo = codeModel.GetDeclaredSymbol(method);
                        if (methodInfo.ReturnType.Name.Equals("Void")) continue;
                        xmlbuilder.AppendFormat("<method returntype={0} name={1}> \n", methodInfo.ReturnType.Name, methodInfo.Name);
                        var param = method.ParameterList.Parameters;
                        int paramcounter = 1;                        
                        foreach (var p in param)
                        {

                            var paramInfo = codeModel.GetDeclaredSymbol(p);
                            xmlbuilder.AppendFormat("<parameter order={0} type={1} name={2}> </parameter> \n", paramcounter, paramInfo.Type, paramInfo.Name);
                            paramcounter++;
                        }
                        xmlbuilder.AppendFormat("<output type={0}> </output>", methodInfo.ReturnType.Name);
                        xmlbuilder.AppendFormat("</method> \n");                        
                    }
                }
                xmlbuilder.AppendFormat("</class> \n");
            }
            xmlbuilder.AppendFormat("</program> \n\n\n");
            
        }

        public string CreateInputXML(Document doc)
        {
            try
            {

                FileInfo file = new FileInfo(doc.FilePath);
                ReadSourceCode(file.FullName);
                CreateCodeModel(doc);
                GenerateXML();
                return xmlbuilder.ToString();       
            }
            catch (Exception e)
            {
                File.WriteAllText("F:\\exc.txt", e.Message + "\n" + e.Source + "\n" + e.TargetSite + "\n" + e.StackTrace);
                return e.Message;

            }
        }
    }
}

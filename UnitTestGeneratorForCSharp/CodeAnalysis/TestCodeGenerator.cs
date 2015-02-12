using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Extensions;
using Microsoft.CodeAnalysis.MSBuild;

namespace UnitTestGeneratorForCSharp.CodeAnalysis
{
    
    public class TestCodeGenerator
    {
        String codeString;
        SyntaxTree codeTree;
        SemanticModel codeModel;
        Compilation codeCompilation;
        Document doc;
        TestCodeBuilder builder=new TestCodeBuilder();

        /*
         * <summary>
         * Read the source code from a file
         * </summary>
         * <param name="filename"> Full Path of the file to be read
         * </param>*/
        private void ReadSourceCode(string filename)
        {
            codeString = File.ReadAllText(filename);
        }

        /*
         * <summary>
         * Creates a syntax tree and a semantic model for the source code
         * </summary>*/
        private void CreateCodeModel(Document doc)
        {
            codeTree = CSharpSyntaxTree.ParseText(codeString);
            var compilation = CSharpCompilation.Create("source").AddSyntaxTrees(codeTree);
            codeModel = compilation.GetSemanticModel(codeTree);
            codeCompilation = doc.Project.GetCompilationAsync().Result;
            this.doc = doc;

        }

        /*
         * <summary>
         * Identifies namespace, adds using statements and enumerates the classes inside the namespace
         * Throws exception if a namespace is not found
         * </summary>*/
        private void CreateUnitTests()
        {
            try
            {
                var NS = codeTree.GetRoot().DescendantNodes().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
                if (NS != null)
                {
                    builder.AddUsing("Microsoft.VisualStudio.TestTools.UnitTesting");
                    builder.AddUsing(NS.Name);
                    var usings = codeTree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>();
                    foreach (var usingdir in usings)
                        builder.AddUsing(usingdir.Name);
                    builder.StartNamespaceBlock(NS.Name);

                    IEnumerable<ClassDeclarationSyntax> classes = codeTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();

                    foreach (var classname in classes)
                    {
                        CreateTestClass(classname);
                    }
                    builder.EndBlock();
                }
                else
                {
                    throw new Exception("No Namespace Found In");
                }
            }
            catch(Exception e)
            {
                throw;
            }
        }

        /*
         * <summary>
         * Creates a test class for given class and enumerates the methods inside the class.
         * </summary>
         * <param name="classname">ClassDeclarationSyntax of given class from the Syntax Tree. Provides info about the class.
         * </param>*/
        private void CreateTestClass(ClassDeclarationSyntax classname)
        {
            bool objectcreated = false;

            builder.AddAttribute("TestClass");
            builder.StartClassBlock(classname.Identifier);

            IEnumerable<MethodDeclarationSyntax> methods = classname.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();
            foreach (var method in methods)
            {
                if (!method.Identifier.ToString().Equals("Main"))
                {
                    CreateTestMethod(classname, method, objectcreated);
                }
                        
            }

            builder.EndBlock();
        }
        
        /*
         * <summary>
         * Creates a test method and generates the method body.
         * </summary>
         * <param name="classname">Provides info about the class in which the method is present
         * </param>
         * <param name="method">MethodDeclarationSyntax of the method from SyntaxTree. Provides info about the method
         * </param>
         * <param name="objectcreated">A boolean that indicates whether an object has been created for the class inside the method body.
         * </param>*/

        private void CreateTestMethod(ClassDeclarationSyntax classname,MethodDeclarationSyntax method,bool objectcreated)
        {
            builder.AddAttribute("TestMethod");
            builder.StartMethodBlock(method.Identifier);            
            var methodInfo = codeModel.GetDeclaredSymbol(method);
            
            var param = method.ParameterList.Parameters;
            int paramcounter = 0;
            int maxparams = 0;

          
            foreach (var p in param)
            {
                
                var paramInfo = codeModel.GetDeclaredSymbol(p);                
                if (paramInfo.Type.IsPrimitiveType())
                {
                    builder.Append(@"var p" + paramcounter + " = " + RandomValueGenerator.GenerateRandomValue(paramInfo.Type) + ";\n");
                }
                else if (paramInfo.Type.IsArray())
                {
                    builder.Append("var p" + paramcounter + " = new[");
                    var arraytype = paramInfo.Type as IArrayTypeSymbol;
                    for (int i = 1; i < arraytype.Rank; i++)
                    {
                        builder.Append(",");
                    }
                    builder.Append("]");
                    RandomValueGenerator.GenerateRandomArray(builder, arraytype, arraytype.Rank);
                    builder.Append(";\n");
                }
                else
                {
                    builder.Append("var p" + paramcounter + "=" + RandomValueGenerator.GenerateRandomObject(paramInfo, builder,doc.Project));
                }
                paramcounter++;
            }
            maxparams = paramcounter;
            paramcounter = 0;
            if (methodInfo.IsStatic)
            {
                builder.Append("Assert.IsInstanceOfType(" + classname.Identifier + "." + methodInfo.Name + "(");
                foreach (var p in param)
                {
                    builder.Append("p" + paramcounter);
                    if (paramcounter != maxparams - 1)
                        builder.Append(",");
                    paramcounter++;
                }
                builder.Append(")," + "typeof("+method.ReturnType + ")); \n");
            }
            else if (methodInfo.IsAbstract)
            {

            }
            else
            {
                if (objectcreated.Equals(false))
                {
                    builder.Append(classname.Identifier + " obj=new " + classname.Identifier + "(); \n");
                }
                builder.Append("Assert.IsInstanceOfType(obj." + methodInfo.Name + "(");
                foreach (var p in param)
                {
                    builder.Append("p" + paramcounter);
                    if (paramcounter != maxparams - 1)
                        builder.Append(",");
                    paramcounter++;
                }
                builder.Append(")," + "typeof("+method.ReturnType + "));\n");

            }
            builder.EndBlock();
        }
            
        /*
         * <summary>
         * Writes the generated unit test to an output file.
         * </summary>
         * <param name="directorypath">Specifies the path of directory in which the file is present
         * </param>
         * <param name="filename">Specifies the name of file for which unit test is generated.
         * </param>*/

        public void WriteToFile(String directorypath,String filename)
        {
            String filepath = directorypath+"\\" + filename.Substring(0,filename.LastIndexOf(".")) + "Test.cs";
            File.Create(filename).Dispose();
            File.WriteAllText(filepath, builder.GetCode());
            builder.Clear();                        
        }

        public string GenerateUnitTest(Document doc)
        {
            try
            {

                FileInfo file = new FileInfo(doc.FilePath);
                ReadSourceCode(file.FullName);
                CreateCodeModel(doc);
                CreateUnitTests();
                WriteToFile(file.DirectoryName,file.Name);
                return "Success";
            }
            catch(Exception e)
            {
               File.WriteAllText("E:\\exc.txt", e.Message + "\n" + e.Source + "\n" + e.TargetSite + "\n" + e.StackTrace);
               
                return e.Message+" for "+doc.Name;
             
            }
            
        }
    }
}

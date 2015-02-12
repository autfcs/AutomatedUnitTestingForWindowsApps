using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestGeneratorForCSharp.CodeAnalysis
{
    class TestCodeBuilder
    {
        StringBuilder builder=new StringBuilder();

        public String GetCode()
        {            
            return builder.ToString();
        }

        public void Clear()
        {
            builder.Clear();
        }
        public void Append(object value)
        {
            builder.Append(value);
        }
        public void AddUsing(object usingdirective)
        {
            builder.AppendLine("using " + usingdirective + ";");
        }

        public void AddAttribute(object attribute)
        {
            builder.AppendLine("[" + attribute + "]");
        }
        public void StartNamespaceBlock(object namespacename)
        {
            builder.AppendLine("namespace " + namespacename+"Test");
            builder.AppendLine("{");
        }
        public void StartClassBlock(object classname)
        {
            builder.AppendLine("public class " +classname + "Test");
            builder.AppendLine("{");
        }
        public void StartMethodBlock(object methodname)
        {
            builder.AppendLine("public void " + methodname + "Test()");
            builder.AppendLine("{");
        }

        public void EndBlock()
        {
            builder.AppendLine("}");
        }
    }
}

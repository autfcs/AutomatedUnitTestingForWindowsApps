using System;
using Microsoft.CodeAnalysis.FindSymbols;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using System.Text;

namespace UnitTestGeneratorForCSharp.CodeAnalysis
{
    static class RandomValueGenerator
    {
        static Random randomObj = new Random(); 
       
        /*
         * <summary>
         * Generates a random value for the given type
         * </summary>
         * <param name="type">Specifies datatype of the value
         * </param>*/
        public static dynamic GenerateRandomValue(ITypeSymbol type)
        {
            switch (type.SpecialType)
            {
                case SpecialType.System_Boolean:
                {
                    return GenerateRandomBoolean();
                }
                case SpecialType.System_Byte:
                case SpecialType.System_SByte:
                {
                    return GenerateRandomByte();
                }
                case SpecialType.System_Char:
                {
                    return "\'"+GenerateRandomChar()+"\'";
                }
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                {
                    return GenerateRandomDouble();
                }
                case SpecialType.System_Int16:
                case SpecialType.System_Int32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt16:
                case SpecialType.System_UInt32:
                case SpecialType.System_UInt64:
                {
                    return GenerateRandomInteger();                  
                }                                   
                case SpecialType.System_Object:
                {
                    return "new object()";
                }
                case SpecialType.System_String:
                {
                    return "\""+GenerateRandomString()+"\"";
                }
                default:
                {                  
                    return "null";
                }
                    
            }
        }

        /*
         * <summary>
         * Gets the constructor with the least parameters
         * </summary>
         * <param name="ctors">List of constructors associated with a class
         * </param>*/

        static IMethodSymbol GetCtorWithLeastParams(System.Collections.Immutable.ImmutableArray<IMethodSymbol> ctors)
        {
            int index = 0;
            int counter=0;
            int min = 999;
            foreach(var ctor in ctors)
            {
                if(ctor.Parameters.Length<min)
                {
                    index = counter;
                    min = ctor.Parameters.Length;
                }
                counter++;
            }
            return ctors[index];
        }

        

        
        static int CreateConstructor(IMethodSymbol method,List<String> paramList,TestCodeBuilder builder,Project comp)
        {
            var param = method.Parameters;
            int paramcounter = 0;
            int maxparams = 0;
            foreach (var p in param)
            {
                String paramname = "c_"+GenerateRandomString().ToLower();
                if (p.Type.IsPrimitiveType())
                {

                    builder.Append("var "+paramname+ " = " + RandomValueGenerator.GenerateRandomValue(p.Type) + ";\n");
                    paramList.Add(paramname);
                }
                else if (p.Type.IsArray())
                {
                    builder.Append("var " +paramname+ " = new[");
                    paramList.Add(paramname);
                    var arraytype = p.Type as IArrayTypeSymbol;
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
                    paramList.Add(paramname);
                    builder.Append("var "+paramname+"="+GenerateRandomObject(p, builder,comp));
                }
                paramcounter++;
            }
            maxparams = paramcounter;
            return maxparams;
            
        }
        public static String GenerateRandomObject(IParameterSymbol type,TestCodeBuilder builder,Project comp)
        {
            string fullyQualifiedName = type.Type.Name;
            
            //var classtype = comp.GetTypeByMetadataName(fullyQualifiedName);
            var classtype = SymbolFinder.FindDeclarationsAsync(comp, fullyQualifiedName, true).Result.Where(s => s.Kind == SymbolKind.NamedType).FirstOrDefault() as INamedTypeSymbol;
            var ctors = classtype.InstanceConstructors;

            var ctor = GetCtorWithLeastParams(ctors);
            List<String> paramList = new List<String>();
            int ctorparams=CreateConstructor(ctor,paramList,builder,comp);
            int counter=0;
            StringBuilder temp = new StringBuilder();
            temp.Append("new " + type.Type + "(");
            foreach(String param in paramList)
            {
                temp.Append(param);
                if (counter != ctorparams - 1)
                    temp.Append(",");
                counter++;
            }
            temp.Append(");\n");
            return temp.ToString();
        }
        public static String GenerateRandomString()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            var result = new string(
                Enumerable.Repeat(chars, randomObj.Next(3, 20))
                          .Select(s => s[randomObj.Next(s.Length)])
                          .ToArray());
            return result;
        }

        public static int GenerateRandomByte()
        {
            return randomObj.Next(0,250);
        }
        public static int GenerateRandomInteger()
        {
            return randomObj.Next(0, 1000);
        }

        public static int GenerateRandomInteger(int min,int max)
        {
            return randomObj.Next(min,max);
        }
        public static double GenerateRandomDouble()
        {
            return randomObj.NextDouble() * 100;
        }

        public static char GenerateRandomChar()
        {
            int randomseed = randomObj.Next(0, 26);
            return (char)('a' + randomseed);
        }

        public static string GenerateRandomBoolean()
        {
            int randomval = randomObj.Next(0, 10);
            if (randomval < 5)
                return "true";
            else
                return "false";
        }

        public static void GenerateRandomArray(TestCodeBuilder builder, IArrayTypeSymbol arraytype,int rank)
        {
            builder.Append("{");
            //int limit = RandomValueGenerator.GenerateRandomInteger(1, 5);
            for (int j = 1; j <= 2; j++)
            {
                if (rank == 1)
                    builder.Append(RandomValueGenerator.GenerateRandomValue(arraytype.ElementType));
                else
                    RandomValueGenerator.GenerateRandomArray(builder, arraytype, rank - 1);
                if (j < 2)
                    builder.Append(",");
            }
            builder.Append("}");                    
        }


    }

}

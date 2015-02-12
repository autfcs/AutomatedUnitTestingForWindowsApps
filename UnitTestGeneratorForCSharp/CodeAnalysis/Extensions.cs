using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestGeneratorForCSharp.CodeAnalysis
{
    public static class ITypeSymbolExtensions
    {
        public static bool IsPrimitiveType(this ITypeSymbol type)
        {
            switch (type.SpecialType)
            {
                case SpecialType.System_Boolean:
                case SpecialType.System_Byte:
                case SpecialType.System_Char:
                case SpecialType.System_Double:
                case SpecialType.System_Int16:
                case SpecialType.System_Int32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt16:
                case SpecialType.System_UInt32:
                case SpecialType.System_UInt64:
                case SpecialType.System_IntPtr:
                case SpecialType.System_UIntPtr:
                case SpecialType.System_SByte:
                case SpecialType.System_Single:
                case SpecialType.System_String:
                case SpecialType.System_Object:
                    return true;
                default:
                    {                       
                        return false;
                    }
                    
            }
        }

        public static bool IsArray(this ITypeSymbol symbol)
        {
            if (symbol.TypeKind == TypeKind.Array)
                return true;
            else
                return false;
        }
    }
    
        
}
  


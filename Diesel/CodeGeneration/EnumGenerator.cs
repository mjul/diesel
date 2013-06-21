using System.CodeDom;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Diesel.Parsing;

namespace Diesel.CodeGeneration
{
    internal class EnumGenerator : CodeDomGenerator
    {
        public static CodeTypeDeclaration CreateEnumDeclaration(EnumDeclaration declaration)
        {
            var result = new CodeTypeDeclaration(declaration.Name)
                {
                    IsStruct = false,
                    IsEnum = true,
                    TypeAttributes = TypeAttributes.Public,
                };
            
            result.CustomAttributes.Add(CreateDataContractAttribute(declaration.Name));
            var fields = declaration.Values.Select(memberName => 
                (CodeTypeMember)new CodeMemberField() 
                {
                    Name = memberName,
                    CustomAttributes = { CreateEnumMemberAttribute(memberName) }
                });
            result.Members.AddRange(fields.ToArray());

            return result;
        }
    }
}
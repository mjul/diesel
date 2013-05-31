using System.CodeDom;
using System.Linq;
using Diesel.Parsing;

namespace Diesel.CodeGeneration
{
    public class ApplicationServiceGenerator : CodeDomGenerator
    {
        public static CodeTypeDeclaration CreateApplicationServiceInterface(ApplicationServiceDeclaration declaration)
        {
            var interfaceName = InterfaceNameFor(declaration.Name);
            var result = new CodeTypeDeclaration(interfaceName)
            {
                IsPartial = true,
                IsInterface = true,
            };
            // Define an "Execute" overload for each command
            var commandHandlerMembers =
                (from c in declaration.Commands
                 select (CodeTypeMember)new CodeMemberMethod()
                 {
                     Attributes = MemberAttributes.Public,
                     Name = "Execute",
                     Parameters = { new CodeParameterDeclarationExpression(c.Name, "command") },
                 }).ToArray();

            result.Members.AddRange(commandHandlerMembers);
            return result;
        } 
    }
}
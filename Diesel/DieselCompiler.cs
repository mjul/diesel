using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diesel.CodeGeneration;
using Diesel.Parsing;
using Diesel.Transformations;
using Sprache;

namespace Diesel
{
    public static class DieselCompiler
    {

        public static string Compile(string modelSourceCode)
        {
            var ast = Grammar.AbstractSyntaxTree.Parse(modelSourceCode);
            var transformed = ModelTransformations.Transform(ast);
            return CompileToSource(CodeDomGenerator.Compile(transformed), GetCSharpProvider());
        }

        public static string CompileToSource(CodeCompileUnit actual, CodeDomProvider codeDomProvider)
        {
            var output = new StringWriter();
            var provider = codeDomProvider;
            provider.GenerateCodeFromCompileUnit(actual, output,
                                                 new CodeGeneratorOptions { BlankLinesBetweenMembers = true });
            var source = output.GetStringBuilder().ToString();
            return source;
        }

        public static CodeDomProvider GetCSharpProvider()
        {
            return CodeDomProvider.CreateProvider("CSharp");
        }
    }
}

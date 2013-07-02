using System;
using System.Collections.Generic;
using Diesel.CodeGeneration;
using Diesel.Parsing;

namespace Diesel.Transformations
{
    /// <summary>
    /// The semantic model of the code being compiled.
    /// </summary>
    public class SemanticModel
    {
        public AbstractSyntaxTree AbstractSyntaxTree { get; private set; }

        private readonly Lazy<IReadOnlyCollection<KnownType>> _knownTypes;
        public IReadOnlyCollection<KnownType> KnownTypes { get { return _knownTypes.Value; } }

        public SemanticModel(AbstractSyntaxTree abstractSyntaxTree)
        {
            AbstractSyntaxTree = abstractSyntaxTree;
            _knownTypes = new Lazy<IReadOnlyCollection<KnownType>>(
                () => KnownTypesHarvester.GetKnownTypes(abstractSyntaxTree));
        }
    }
}
﻿using System.Collections.Generic;
using Diesel.Parsing.CSharp;

namespace Diesel.Parsing
{
    public class ConventionsDeclaration : IDieselExpression
    {
        public DomainEventConventions DomainEventConventions { get; private set; }

        public ConventionsDeclaration(DomainEventConventions domainEventConventions)
        {
            DomainEventConventions = domainEventConventions;
        }

        public IEnumerable<ITreeNode> Children
        {
            get { yield return DomainEventConventions; }
        }

        public void Accept(IDieselExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
        public ConventionsDeclaration ApplyOverridesFrom(ConventionsDeclaration other)
        {
            if (other != null)
            {
                var newDomainEventConventions = 
                    (DomainEventConventions ?? new DomainEventConventions(new TypeName[] {}))
                    .ApplyOverridesFrom(other.DomainEventConventions);
                return new ConventionsDeclaration(newDomainEventConventions);
            }
            else
            {
                return this;
            }
        }
    }
}
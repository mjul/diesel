using System;
using System.Collections.Generic;
using System.Linq;

namespace Diesel.Parsing.CSharp
{
    public class RankSpecifiers : ITreeNode, IEquatable<RankSpecifiers>
    {
        public IEnumerable<RankSpecifier> Ranks { get; private set; }

        public RankSpecifiers(IEnumerable<RankSpecifier> rankSpecifiers)
        {
            Ranks = rankSpecifiers;
        }
        public IEnumerable<ITreeNode> Children { get { yield break; } }

        public bool Equals(RankSpecifiers other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            var myRanks = Ranks.ToList();
            var otherRanks = other.Ranks.ToList();
            if (myRanks.Count != otherRanks.Count) return false;
            return !Enumerable.Zip(myRanks, otherRanks, (x, y) => (x == y))
                              .Any(pairEqual => pairEqual == false);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RankSpecifiers) obj);
        }

        public override int GetHashCode()
        {
            return 1;
        }

        public static bool operator ==(RankSpecifiers left, RankSpecifiers right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RankSpecifiers left, RankSpecifiers right)
        {
            return !Equals(left, right);
        }
    }
}
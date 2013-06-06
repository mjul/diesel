using System;
using Sprache;

namespace Diesel.Parsing
{
    public static class TokenGrammar
    {
        private static Parser<Char> Char(char c, string name)
        {
            return Parse.Char(c).Named(name);
        }

        public static readonly Parser<Char> LeftParen = Char('(', "LeftParen");
        public static readonly Parser<Char> RightParen = Char(')', "RightParen");
        public static readonly Parser<Char> Letter = Parse.Letter;
        public static readonly Parser<Char> LetterOrDigit = Parse.LetterOrDigit;
        public static readonly Parser<Char> Comma = Char(',', "Comma");
        public static readonly Parser<Char> Period = Char('.', "Period");
        public static readonly Parser<Char> QuestionMark = Char('?', "Question Mark");
        public static readonly Parser<Char> Colon = Char(':', "Colon");

        public static readonly Parser<Char> LeftCurlyBrace = Char('{', "LeftCurlyBrace");
        public static readonly Parser<Char> RightCurlyBrace = Char('}', "RightCurlyBrace");
        public static readonly Parser<Char> LeftSquareBracket = Char('[', "LeftSquareBracket");
        public static readonly Parser<Char> RightSquareBracket = Char(']', "RightSquareBracket");
 
    }
}
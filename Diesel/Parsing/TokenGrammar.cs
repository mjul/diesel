using System;
using System.Collections.Generic;
using Sprache;

namespace Diesel.Parsing
{
    public static class TokenGrammar
    {
        private static Parser<Char> Char(char c, string name)
        {
            return Parse.Char(c).Named(name);
        }

        public static Parser<String> String(string value)
        {
            return Parse.String(value).Text().Named(value);
        }

        public static readonly Parser<Char> LeftParen = Char('(', "LeftParen");
        public static readonly Parser<Char> RightParen = Char(')', "RightParen");
        public static readonly Parser<Char> Letter = Parse.Letter;
        public static readonly Parser<Char> LetterOrDigit = Parse.LetterOrDigit;
        public static readonly Parser<Char> Comma = Char(',', "Comma");
        public static readonly Parser<Char> Period = Char('.', "Period");
        public static readonly Parser<Char> QuestionMark = Char('?', "Question Mark");
        public static readonly Parser<Char> Colon = Char(':', "Colon");
        public static readonly Parser<Char> Underscore = Char('_', "Underscore");

        public static readonly Parser<Char> LeftCurlyBrace = Char('{', "LeftCurlyBrace");
        public static readonly Parser<Char> RightCurlyBrace = Char('}', "RightCurlyBrace");
        public static readonly Parser<Char> LeftSquareBracket = Char('[', "LeftSquareBracket");
        public static readonly Parser<Char> RightSquareBracket = Char(']', "RightSquareBracket");

        public static readonly Parser<Char> Semicolon = Char(';', "Semicolon");
        
        public static readonly Parser<Char> CarriageReturn = Char('\u000D' , "CR");
        public static readonly Parser<Char> LineFeed = Char('\u000A', "LineFeed");
        public static readonly Parser<Char> LineSeparator = Char('\u2028', "LineSeparator");
        public static readonly Parser<Char> ParagraphSeparator = Char('\u2029', "ParagraphSeparator");

        private static readonly char[] NewLineChars = {'\u000D', '\u000A', '\u2028', '\u2029'};

        public static readonly Parser<string> CarriageReturnLineFeed
            = (from cr in CarriageReturn
               from lf in LineFeed
               select Environment.NewLine);

        public static readonly Parser<String> NewLine =
            CarriageReturnLineFeed
                .Or(CarriageReturn.Select(c => Environment.NewLine))
                .Or(LineFeed.Select(c => Environment.NewLine))
                .Or(LineSeparator.Select(c => Environment.NewLine))
                .Or(ParagraphSeparator.Select(c => Environment.NewLine));

        public static readonly Parser<String> RestOfLine = Parse.CharExcept(NewLineChars).Many().Text();
    }
}
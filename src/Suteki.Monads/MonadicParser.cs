using System;
using System.Collections.Generic;
using System.Linq;

namespace Suteki.Monads
{
    public class MonadicParser
    {
        public static Parser<IEnumerable<string>> MakeHelloWorldParser()
        {
            return
                from token in "Hello".Find().Or("World".Find())
                from _ in Parsers.WhiteSpace()
                from list in Parsers.End(Enumerable.Empty<string>()).Or(MakeHelloWorldParser())
                select token.Cons(list);
        }

        public void ParseHelloWorld()
        {
            var helloWorldParser = MakeHelloWorldParser();

            Action<Maybe<Tuple<IEnumerable<string>, string>>> writeResult = 
                s => Console.WriteLine(s.AsString(t => t.Aggregate("", (a, b) => a + " - " + b)));

            var r1 = helloWorldParser("Hello World Hello World");
            writeResult(r1);

            var r2 = helloWorldParser("Hello      Hello Hello HelloWorld");
            writeResult(r2);
            
            var r3 = helloWorldParser("Hello x World");
            writeResult(r3);
        }

        public void SimpleHelloWorldParser()
        {
            var helloWorldParser =
                from hello in "Hello".Find()
                from world in "World".Find()
                select new {Hello = hello, World = world};

            var result = helloWorldParser("HelloWorld");

            Console.WriteLine(result.AsString(x => x.Hello));
            Console.WriteLine(result.AsString(x => x.World));

            // outputs 
            // Hello
            // World
        }
    }

    public delegate Maybe<Tuple<T, string>> Parser<T>(string input);

    public static class ParserExtensions
    {
        public static Parser<T> ToParser<T>(this T value)
        {
            return s => new Just<Tuple<T, string>>(Tuple.Create(value, s));
        }

        public static Parser<B> Bind<A, B>(this Parser<A> a, Func<A, Parser<B>> func)
        {
            return s =>
            {
                var aMaybe = a(s);
                var aResult = aMaybe as Just<Tuple<A, string>>;
                
                // short circuit if parse fails
                if (aResult == null) return new Nothing<Tuple<B, string>>();

                var aValue = aResult.Value.Item1;
                var sString = aResult.Value.Item2;

                var bParser = func(aValue);
                return bParser(sString);
            };
        }

        public static Parser<C> SelectMany<A, B, C>(this Parser<A> a, Func<A, Parser<B>> func, Func<A, B, C> select)
        {
            return a.Bind(aval => func(aval).Bind(bval => select(aval, bval).ToParser()));
        }

        public static Parser<T> Or<T>(this Parser<T> a, Parser<T> b)
        {
            return s => 
            {
                var aMaybe = a(s);
                return aMaybe is Just<Tuple<T,string>> ? aMaybe : b(s);
            };
        }
    }

    public static class Parsers
    {
        public static Parser<string> WhiteSpace()
        {
            return s => new Just<Tuple<string, string>>(Tuple.Create<string, string>(null, s.TrimStart()));
        }

        public static Parser<string> Find(this string stringToFind)
        {
            return s => s.StartsWith(stringToFind)
                ? new Just<Tuple<string, string>>(Tuple.Create(stringToFind,s.Skip(stringToFind.Length).AsString()))
                : (Maybe<Tuple<string, string>>)new Nothing<Tuple<string, string>>();
        }

        public static Parser<T> End<T>(T initialValue)
        {
            return s => (s == "") 
                ? new Just<Tuple<T, string>>(Tuple.Create(initialValue, s))
                : (Maybe<Tuple<T, string>>)new Nothing<Tuple<T, string>>();
        }

        public static string AsString(this IEnumerable<char> chars)
        {
            return new string(chars.ToArray());
        }

        public static string AsString<T>(this Maybe<Tuple<T,string>> parseResult, Func<T, string> unwrap)
        {
            var justParseResult = parseResult as Just<Tuple<T, string>>;

            return (justParseResult != null)
                       ? unwrap(justParseResult.Value.Item1)
                       : "Nothing";
        }
    }
}
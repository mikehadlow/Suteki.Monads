using System;
using System.Collections.Generic;
using System.Linq;

namespace Suteki.Monads
{
    public class FunctionComposition2
    {
        public void SimpleComposition()
        {
            Func<int, int> add2 = x => x + 2;
            Func<int, int> mult2 = x => x*2;

            Func<int, int> addMult2 = x => mult2(add2(x));
            Console.Out.WriteLine("addMult2(4) = {0}", addMult2(4));
        }

        public void AmplifiedFunctionComposition()
        {
            Func<int, Ident<int>> add2 = x => (x + 2).ToIdent();
            Func<int, Ident<int>> mult2 = x => (x*2).ToIdent();

            Func<int, Ident<int>> addMult2 = x => add2(x).Bind(mult2);

            Console.Out.WriteLine("addMult2(4).Value = {0}", addMult2(4).Value);
        }

        public void SomeMoreCompositionWithIdent()
        {
            var result =
                from a in 5.ToIdent()
                from b in "Hello".ToIdent()
                from c in new DateTime(2010, 1, 18).ToIdent()
                select a.ToString() + " " + b + " " + c.ToShortDateString();

            Console.Out.WriteLine("result.Value = {0}", result.Value);
        }

        public void ComposeEnumerables()
        {
            var result =
                from a in new[] {1, 2, 3}.AsEnumerable()
                from b in new[] {4, 5, 6}.AsEnumerable()
                select a + ", " + b;

            foreach (var item in result)
            {
                Console.Out.WriteLine("item = {0}", item);
            }
        }
    }

    public class Ident<T>
    {
        public T Value { get; private set; }
        public Ident(T value)
        {
            Value = value;
        }
    }

    public static class IdentExtensions
    {
        public static Ident<T> ToIdent<T>(this T value)
        {
            return new Ident<T>(value);
        }

        public static Ident<B> Bind<A,B>(this Ident<A> a, Func<A, Ident<B>> func)
        {
            return func(a.Value);
        }

        public static Ident<C> SelectMany<A, B, C>(this Ident<A> a, Func<A, Ident<B>> func, Func<A,B,C> select)
        {
            return a.Bind(aval => func(aval).Bind(bval => select(aval, bval).ToIdent()));
        }
    }

    public static class EnumerableExtensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this T value)
        {
            yield return value;
        }

        public static IEnumerable<B> Bind<A, B>(this IEnumerable<A> a, Func<A, IEnumerable<B>> func)
        {
            foreach (var aval in a)
            {
                foreach (var bval in func(aval))
                {
                    yield return bval;
                }
            }
        }
    }
}
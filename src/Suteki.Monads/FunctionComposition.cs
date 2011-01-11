using System;

namespace Suteki.Monads
{
    public class FunctionComposition
    {
        public void ComposingSimpleFunctions()
        {
            // two simple functions that take an int and return an int
            Func<int, int> add2 = x => x + 2;
            Func<int, int> mult2 = x => x*2;

            var a = 5;

            // we can execute them immediately, one after another
            var r1 = mult2(add2(a));
            Console.Out.WriteLine("r1 = {0}", r1);

            // or we can 'compose' them
            Func<int, int> add2Mult2 = x => mult2(add2(x));

            // and then use the new function later
            var r2 = add2Mult2(a);
            Console.Out.WriteLine("r2 = {0}", r2);
        }

        public void ComposingFunctionsWithAmplifiedValues()
        {
            // two simple functions that take an int and return an Identity<int>
            Func<int, Identity<int>> add2 = x => new Identity<int>(x + 2);
            Func<int, Identity<int>> mult2 = x => new Identity<int>(x * 2);

            var a = 5;

            // we can't compose them directly, the types don't match. This won't compile:
            // Func<int, Identity<int>> add2Mult2 = x => mult2(add2(x));

            // we need a 'Bind' function to compose them:
            Func<int, Identity<int>> add2Mult2 = x => add2(x).Bind(mult2);

            // we can now use add2Mult2 at some later date
            var r1 = add2Mult2(a);
            Console.Out.WriteLine("r1.Value = {0}", r1.Value);
        }

        public void WriteArbitraryIdentityExpressions()
        {
            var result = 
                "Hello World!".ToIdentity().Bind(                   a => 
                7.ToIdentity().Bind(                                b =>
                (new DateTime(2010, 1, 11)).ToIdentity().Bind(      c =>
                (a + ", " + b.ToString() + ", " + c.ToShortDateString())
                .ToIdentity())));

            Console.WriteLine(result.Value);
        }
    }

    // The simplest possible Monad, Identity
    // it does nothing more than contain a value of the given the type
    public class Identity<T>
    {
        public T Value { get; private set; }

        public Identity(T value)
        {
            Value = value;
        }
    }

    public static class IdentityExtensions
    {
        // These two functions make Identity<T> a Monad.

        // a function 'Unit' or 'Return' or 'ToIdentity' that creates a new instance of Identity
        public static Identity<T> ToIdentity<T>(this T value)
        {
            return new Identity<T>(value);
        }

        // a function 'Bind', that allows us to compose Identity returning functions
        public static Identity<B> Bind<A, B>(this Identity<A> a, Func<A, Identity<B>> func)
        {
            return func(a.Value);
        }
    }
}
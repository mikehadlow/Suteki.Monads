using System;
using System.Collections.Generic;
using System.Linq;

namespace Demo
{
    public class Composition
    {
        public void SimpleComposition()
        {
            Func<int, int> add2 = x => x + 2;
            Func<int, int> mult2 = x => x*2;

            Func<int, int> add2Mult2 = x => mult2(add2(x));
            Console.Out.WriteLine("add2Mult2 = {0}", add2Mult2(3));
        }

        public void MaybeComposition()
        {
            Func<int, Maybe<int>> add2 = x => (x == 0) ? Maybe<int>.Nothing() : Maybe<int>.Just(x+2);
            Func<int, Maybe<int>> mult2 = x => (x == 0) ? Maybe<int>.Nothing() : Maybe<int>.Just(x*2);

            Func<int, Maybe<int>> add2Mult2 = x => add2(x).Bind(mult2);
            var result = add2Mult2(3);
            Console.Out.WriteLine("result = {0}", result);
        }

        public void PlayWithBind()
        {
            var maybeInt = Maybe<int>.Just(5);
            var maybeString = Maybe<string>.Just("Hello");
            var maybeDate = Maybe<DateTime>.Just(DateTime.Now);
            var nothing = Maybe<Guid>.Nothing();

            var result =
                maybeInt.Bind(a =>
                maybeString.Bind(b =>
                maybeDate.Bind(c =>
                    string.Format("{0} {1} {2}", a, b, c).ToMaybe()
                )));

            Console.Out.WriteLine("result = {0}", result);

            var linq =
                from a in maybeInt
                from b in maybeString
                from c in maybeDate
                select string.Format("{0} {1} {2}", a, b, c);

            Console.Out.WriteLine("linq = {0}", linq);
        }
    }

    public static class MaybeExtensions
    {
        public static Maybe<T> ToMaybe<T>(this T value)
        {
            return Maybe<T>.Just(value);
        }

        public static Maybe<B> Bind<A,B>(this Maybe<A> a, Func<A, Maybe<B>> func)
        {
            return a.HasValue ? func(a.Value) : Maybe<B>.Nothing();
        }

        public static Maybe<C> SelectMany<A,B,C>(this Maybe<A> a, Func<A, Maybe<B>> func, Func<A,B,C> select)
        {
            return a.Bind(aval => func(aval).Bind(bval => select(aval, bval).ToMaybe()));
        }
    }

    public class Maybe<T>
    {
        public T Value { get; private set; }
        public bool HasValue { get; private set; }
        public static Maybe<T> Just(T value)
        {
            return new Maybe<T> {Value = value, HasValue = true};
        }

        public static Maybe<T> Nothing()
        {
            return new Maybe<T> {Value = default(T), HasValue = false};
        }
        public override string ToString()
        {
            return HasValue ? Value.ToString() : "Nothing";
        }
    }
}

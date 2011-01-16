using System;

namespace Suteki.Monads
{
    public class MaybeMonad
    {
        public Maybe<int> DoSums(int denominator)
        {
            return from a in 12.Div(denominator)
                   from b in a.Div(2)
                   select b;
        }

        public void UseMaybe()
        {
            var result = from a in "Hello World!".ToMaybe()
                         from b in DoSums(2)
                         from c in (new DateTime(2010, 1, 14)).ToMaybe()
                         select a + " " + b.ToString() + " " + c.ToShortDateString();

            Console.WriteLine(result);
        }

        public void UseMaybeWithANothingValue()
        {
            var result = from a in "Hello World!".ToMaybe()
                         from b in new Nothing<int>()
                         from c in (new DateTime(2010, 1, 14)).ToMaybe()
                         select a + " " + b.ToString() + " " + c.ToShortDateString();

            Console.WriteLine(result);
        }
    }

    public interface Maybe<T>
    {
        bool HasValue { get; }
    }

    public class Nothing<T> : Maybe<T>
    {
        
        public override string ToString()
        {
            return "Nothing";
        }

        public bool HasValue
        {
            get { return false; }
        }
    }

    public class Just<T> : Maybe<T>
    {
        public T Value { get; private set; }
        public Just(T value)
        {
            Value = value;
        }
        public override string ToString()
        {
            return Value.ToString();
        }

        public bool HasValue
        {
            get { return true; }
        }
    }

    public static class MaybeExtensions
    {
        public static Maybe<int> Div(this int numerator, int denominator)
        {
            return denominator == 0
                       ? (Maybe<int>)new Nothing<int>()
                       : new Just<int>(numerator/denominator);
        }

        public static Maybe<T> ToMaybe<T>(this T value)
        {
            return new Just<T>(value);
        }

        public static Maybe<B> Bind<A, B>(this Maybe<A> a, Func<A, Maybe<B>> func)
        {
            var justa = a as Just<A>;
            return justa == null ? 
                new Nothing<B>() : 
                func(justa.Value);
        }

        public static Maybe<C> SelectMany<A, B, C>(this Maybe<A> a, Func<A, Maybe<B>> func, Func<A, B, C> select)
        {
            return  a.Bind(             aval => 
                    func(aval).Bind(    bval => 
                    select(aval, bval).ToMaybe()));
        }
    }
}
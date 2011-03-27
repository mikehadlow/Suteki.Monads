using System;
using System.Collections.Generic;

namespace Suteki.Monads
{
    public class Scratchpad
    { }

    public class Whatever<T>{}

    public static class WhateverExtensions
    {
        public static Whatever<T> ToWhatever<T>(T value) // AKA unit
        {
            return new Whatever<T>();
        }

        public static Whatever<B> Bind<A,B>(Whatever<A> a, Func<A, Whatever<B>> func)
        {
            return new Whatever<B>();
        }
    }

}

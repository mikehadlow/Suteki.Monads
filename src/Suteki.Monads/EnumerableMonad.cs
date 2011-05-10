using System;
using System.Linq;
using System.Collections.Generic;

namespace Suteki.Monads
{
	public class EnumerableMonad
	{
		public static void EnumerableComposition()
		{
			var aValues = Enumerable.Range(0,2);
			var bValues = Enumerable.Range(3,5);
			
			var results =
				aValues.Bind(a => 
				bValues.Bind(b =>
					string.Format("{0} {1}", a, b).ToEnumerable()
					));
			
			foreach(var result in results)
			{
				Console.WriteLine(result);	
			}
		}
	}
	
	public static class EnumerableExtensions
	{
		public static IEnumerable<T> ToEnumerable<T>(this T value)
		{
			yield return value;
		}
		
		public static IEnumerable<B> Bind<A,B>(this IEnumerable<A> a, Func<A, IEnumerable<B>> func)
		{
			foreach(var aval in a)
			{
				foreach(var bval in func(aval))
				{
					yield return bval;	
				}
			}
		}
		
        public static IEnumerable<C> SelectMany<A,B,C>(this IEnumerable<A> a, Func<A, IEnumerable<B>> func, Func<A,B,C> select)
        {
            return a.Bind(aval => func(aval).Bind(bval => select(aval, bval).ToEnumerable()));
        }
	}
}


using System;
using System.Threading.Tasks;

namespace Suteki.Monads
{
	public class TaskMonad
	{
		public static void TaskComposition()
		{
			var aTask = Task.Factory.StartNew(() => 3);
			var bTask = Task.Factory.StartNew(() => 4);
			
			var result =
				aTask.Bind(a =>
				bTask.Bind(b =>
					(a + b).ToTask()
					));
			
			result.ContinueWith(t => Console.WriteLine("t.Result = {0}", t.Result));
			result.Wait();
		}
	}
	
	public static class TaskExtensions
	{
		public static Task<T> ToTask<T>(this T value)
		{
			return Task.Factory.StartNew(() => value);
		}
		
		public static Task<B> Bind<A,B>(this Task<A> a, Func<A, Task<B>> func)
		{
			return a.ContinueWith(aTask => func(aTask.Result)).Unwrap();
		}
		
        public static Task<C> SelectMany<A,B,C>(this Task<A> a, Func<A, Task<B>> func, Func<A,B,C> select)
        {
            return a.Bind(aval => func(aval).Bind(bval => select(aval, bval).ToTask()));
        }
		
	}
}


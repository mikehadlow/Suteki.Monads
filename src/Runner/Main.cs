using System;
using Suteki.Monads;

namespace Runner
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			EnumerableMonad.EnumerableComposition();
			TaskMonad.TaskComposition();
		}
	}
}

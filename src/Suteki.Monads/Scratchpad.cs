using System;
using System.Collections.Generic;

namespace Suteki.Monads
{
    public class Scratchpad
    {
        public void IntroExamples()
        {
            var inputValue = " some input value ";
            var trimmedValue = Trim(inputValue);
            if (trimmedValue == null)
            {
                // so something different here
            }

            foreach (var number in Range(5, 10))
            {
                // do something with the number
            }

            if (IsAFactorOfTwelve(4))
            {
                Console.WriteLine("Yes it is");
            }
        }

        bool IsAFactorOfTwelve(int value)
        {
            if (value == 0) throw new Exception("Can't divide by zero");
            return (12%value) == 0;
        }

        string Trim(string value)
        {
            if (value == null) return null;
            return value.Trim();
        }

        IEnumerable<int> Range(int from, int to)
        {
            for (int i = from; i <= to; i++)
            {
                yield return i;
            }
        }


    }
}
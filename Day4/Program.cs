using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Day4
{
    class Program
    {
        static void Main(string[] args)
        {
            Part1BruteForce();
            Part2BruteForce();
            Part2GroupBy();
        }

        private static void Part1BruteForce()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var count = 0;
            for (int i = 183564; i < 657475; i++)
            {
                var s = i.ToString(CultureInfo.InvariantCulture);

                //verify monotonous increase
                if (s[0] > s[1] || s[1] > s[2] || s[2] > s[3] || s[3] > s[4] || s[4] > s[5])
                    continue;

                //verify double digits
                if (s[0] != s[1] && s[1] != s[2] && s[2] != s[3] && s[3] != s[4] && s[4] != s[5])
                    continue;


                //Console.WriteLine($"{i} is a valid password!");
                count++;
            }

            Console.WriteLine($"Result: {count}. It took {sw.ElapsedMilliseconds} ms.");
        }

        private static void Part2BruteForce()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var count = 0;
            for (int i = 183564; i < 657475; i++)
            {
                var s = i.ToString(CultureInfo.InvariantCulture);

                //verify double digits
                var continuousSimilarDigits = 1;
                bool has2ContinuousDigits = false;
                for (int j = 1; j < 6; j++)
                {
                    if (s[j] == s[j - 1])
                        continuousSimilarDigits++;
                    else
                    {
                        if (continuousSimilarDigits == 2)
                            has2ContinuousDigits = true;

                        continuousSimilarDigits = 1;
                    }
                }

                if (!has2ContinuousDigits && continuousSimilarDigits != 2)
                    continue;

                ////verify monotonous increase
                if (s[0] > s[1] || s[1] > s[2] || s[2] > s[3] || s[3] > s[4] || s[4] > s[5])
                    continue;

                //Console.WriteLine($"{i} is a valid password!");
                count++;
            }

            Console.WriteLine($"Result: {count}. It took {sw.ElapsedMilliseconds} ms.");
        }

        private static void Part2GroupBy()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var count = 0;
            for (int i = 183564; i < 657475; i++)
            {
                var s = i.ToString(CultureInfo.InvariantCulture);

                //verify double digits
                if (s.GroupBy(c => c).Select(g => g.Count()).All(groupSize => groupSize != 2))  //10x slower!
                    continue;

                ////verify monotonous increase
                if (s[0] > s[1] || s[1] > s[2] || s[2] > s[3] || s[3] > s[4] || s[4] > s[5])
                    continue;

                //Console.WriteLine($"{i} is a valid password!");
                count++;
            }

            Console.WriteLine($"Result: {count}. It took {sw.ElapsedMilliseconds} ms.");
        }


    }
}

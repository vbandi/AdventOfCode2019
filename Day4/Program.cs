using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Day4
{
    class Program
    {
        static void Main(string[] args)
        {
            Part1BruteForce();
            Part2BruteForce();
            Part2Linq();
            ParallelFor();
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

        private static void Part2Linq() //slow but readable
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var result = Enumerable.Range(183564, 657475 - 183564)
                .Select(i => i.ToString(CultureInfo.InvariantCulture))
                .Count(s =>
                    s.SequenceEqual(s.OrderBy(c => c)) &&
                    s.GroupBy(c => c).Select(g => g.Count()).Any(groupSize => groupSize == 2)
                );

            Console.WriteLine($"Result: {result}. It took {sw.ElapsedMilliseconds} ms.");
        }

        private static object lck = new object();

        private static void ParallelFor()
        {
            Stopwatch sw = Stopwatch.StartNew();
            var count = 0;
            Parallel.For(183564, 657475, new ParallelOptions {MaxDegreeOfParallelism = 16 }, (i) =>
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

                if (s[0] <= s[1] && s[1] <= s[2] && s[2] <= s[3] && s[3] <= s[4] && s[4] <= s[5])
                    if (has2ContinuousDigits || continuousSimilarDigits == 2)
                        lock (lck)
                            count++;

            });
            Console.WriteLine($"Result: {count}. It took {sw.ElapsedMilliseconds} ms.");

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Day7
{
    public static class Amplifiers
    {
        /// <summary>
        /// Executes the specified program as pert Part1
        /// </summary>
        public static async Task<int> ExecuteAsync(string program)
        {
            IEnumerable<IEnumerable<int>> permutations = GetPermutations(new [] {0, 1, 2, 3, 4}, 5);
            int max = int.MinValue;

            foreach (var permutation in permutations)
            {
                var z = await ExecutePermutationWithBuffersAsync(program, permutation.ToArray());

                if (z > max)
                    max = z;
            }

            return max;
        }

        /// <summary>Executes the specified
        /// program as per Part2</summary>
        /// <param name="program">The program.</param>
        public static int ExecuteWithFeedback(string program)
        {
            IEnumerable<IEnumerable<int>> permutations = GetPermutations(new[] { 5, 6, 7, 8, 9 }, 5);
            return permutations.Select(p => ExecutePermutationWithFeedback(program, p.ToArray())).Max();
        }

        private static async Task<int> ExecutePermutationWithBuffersAsync(string program, int[] phaseSettings)
        {
            var computers = CreateAndLinkComputers();
            var tasks = new List<Task>();

            for (int i = 0; i < computers.Length; i++)
            {
                computers[i].input.Post(phaseSettings[i]);
                tasks.Add(computers[i].ExecuteAsync(program));
            }

            computers.First().input.Post(0);

            Task.WaitAll(tasks.ToArray());

            return computers.Last().OutputList.Last(); //last computer's output
        }

        private static int ExecutePermutationWithFeedback(string program, int[] phaseSettings)
        {
            var computers = CreateAndLinkComputers();
            computers.Last().output.LinkTo(computers.First().input);  //the feedback

            var tasks = new List<Task>();

            for (int i = 0; i < computers.Length; i++)
            {
                computers[i].input.Post(phaseSettings[i]);
                tasks.Add(computers[i].ExecuteAsync(program));
            }

            computers.First().input.Post(0);
            
            Task.WaitAll(tasks.ToArray());

            return computers.Last().OutputList.Last(); //last computer's output
        }

        private static IntCodeComputer[] CreateAndLinkComputers()
        {
            var computers = new IntCodeComputer[5];

            for (int i = 0; i < computers.Length; i++)
            {
                computers[i] = new IntCodeComputer();
                computers[i].Name = $"Comp{i}";

                if (i > 0)
                    computers[i - 1].output.LinkTo(computers[i].input);
            }

            return computers;
        }

        // From https://stackoverflow.com/questions/756055/listing-all-permutations-of-a-string-integer
        private static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });

            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }
    }
}

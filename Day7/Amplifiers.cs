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
        public static async Task<int> ExecuteAsync(string program)
        {
            IEnumerable<IEnumerable<int>> permutations = GetPermutations(new [] {0, 1, 2, 3, 4}, 5);


            //Part 1 solution
            //return permutations.Select(p => ExecutePermutation(program, p)).Max();

            //Part 1 solution with input-output buffers
            return await permutations.Select(async p => await ExecutePermutationWithBuffersAsync(program, p.ToArray())).Max();
        }

        private static async Task<int> ExecutePermutationWithBuffersAsync(string program, int[] phaseSettings)
        {
            var outputObserver = new IntObserver();
            var computers = CreateAndLinkComputers();
            computers.Last().output.AsObservable().Subscribe(outputObserver);

            for (int i = 0; i < computers.Length; i++)
            {
                computers[i].input.Post(phaseSettings[i]);
                await computers[i].ExecuteAsync(program);
            }

            return outputObserver.LastValue;
        }

        class IntObserver : IObserver<int>
        {
            public int LastValue { get; private set; } = int.MinValue;
            public void OnNext(int value)
            {
                LastValue = value;
            }

            public void OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            public void OnCompleted()
            {
            }
        }

        private static IntCodeComputer[] CreateAndLinkComputers()
        {
            var computers = new IntCodeComputer[5];

            for (int i = 0; i < computers.Length; i++)
            {
                computers[i] = new IntCodeComputer();

                if (i > 1)
                    computers[i - 1].output.LinkTo(computers[i].input);
            }

            return computers;
        }

        public static int ExecuteWithFeedback(string program)
        {
            IEnumerable<IEnumerable<int>> permutations = GetPermutations(new[] { 5, 6, 7, 8, 9 }, 5);
            return permutations.Select(p => ExecutePermutationWithFeedback(program, p)).Max();
        }

        //private static async Task<int> ExecutePermutation(string program, IEnumerable<int> phaseSettings)
        //{


        //    return phaseSettings.Aggregate(0, async (current, phaseSetting) => 
        //        (await computer.ExecuteAsync(program)).output.First());
        //}

        private static int ExecutePermutationWithFeedback(string program, IEnumerable<int> phaseSettings)
        {
            throw  new NotImplementedException();
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

﻿using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Day7
{
    [TestClass]
    public class Day7Tests
    {
        [DataTestMethod]
        [DataRow("3,15,3,16,1002,16,10,16,1,16,15,15,4,15,99,0,0", 43210)]
        [DataRow("3,23,3,24,1002,24,10,24,1002,23,-1,23,101, 5, 23, 23, 1, 24, 23, 23, 4, 23, 99, 0, 0", 54321)]
        [DataRow("3,31,3,32,1002,32,10,32,1001,31,-2,31,1007,31,0,33,1002, 33, 7, 33, 1, 33, 31, 31, 1, 32, 31, 31, 4, 31, 99, 0, 0, 0", 65210)]
        public async Task RunPart1Examples(string program, int expectedOutput)
        {
            (await Amplifiers.ExecuteAsync(program)).ShouldBe(expectedOutput);
        }

        [Ignore]
        [DataTestMethod]
        [DataRow("3,26,1001,26,-4,26,3,27,1002,27,2,27,1,27,26,27, 4, 27, 1001, 28, -1, 28, 1005, 28, 6, 99, 0, 0, 5", 139629729)]
        [DataRow("3,52,1001,52,-5,52,3,53,1,52,56,54,1007,54,5,55,1005,55,26,1001,54,- 5, 54, 1105, 1, 12, 1, 53, 54, 53, 1008, 54, 0, 55, 1001, 55, 1, 55, 2, 53, 55, 53, 4,53, 1001, 56, -1, 56, 1005, 56, 6, 99, 0, 0, 0, 0, 10", 
            54321)]
        public void RunPart2Examples(string program, int expectedOutput)
        {
            Amplifiers.ExecuteWithFeedback(program).ShouldBe(expectedOutput);
        }

        [TestMethod]
        public async Task RunDay7Part1()
        {
            string program =
                "3,8,1001,8,10,8,105,1,0,0,21,30,55,76,97,114,195,276,357,438,99999,3,9,102,3,9,9,4,9,99,3,9,1002,9,3,9,1001,9,5,9,1002,9,2,9,1001,9,2,9,102,2,9,9,4,9,99,3,9,1002,9,5,9,1001,9,2,9,102,5,9,9,1001,9,4,9,4,9,99,3,9,1001,9,4,9,102,5,9,9,101,4,9,9,1002,9,4,9,4,9,99,3,9,101,2,9,9,102,4,9,9,1001,9,5,9,4,9,99,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,101,1,9,9,4,9,99,3,9,1002,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,101,1,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,101,2,9,9,4,9,3,9,102,2,9,9,4,9,99,3,9,101,1,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,1001,9,1,9,4,9,3,9,101,1,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,1001,9,1,9,4,9,99,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,101,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,101,2,9,9,4,9,3,9,101,1,9,9,4,9,3,9,1001,9,1,9,4,9,99,3,9,101,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,101,2,9,9,4,9,3,9,101,1,9,9,4,9,3,9,101,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,1001,9,1,9,4,9,3,9,101,2,9,9,4,9,99";

            Console.WriteLine(await Amplifiers.ExecuteAsync(program));
        }

        [TestMethod]
        public async Task BufferBlockTests()
        {
            var output = new BufferBlock<int>();
            var input = new BufferBlock<int>();

            output.Post(5);
            (await output.ReceiveAsync()).ShouldBe(5);

            input.LinkTo(output);
            input.Post(2);
            (await output.ReceiveAsync()).ShouldBe(2);

            input.Post(42);
            input.Post(43);
            (await output.ReceiveAsync()).ShouldBe(42);
            (await output.ReceiveAsync()).ShouldBe(43);

        }


    }
}

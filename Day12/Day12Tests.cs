using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Day12
{
    [TestClass]
    public class Day12Tests
    {
        [TestMethod]
        public void Part1ExampleTest()
        {
            var scanData = new BodyState[]
            {
                new BodyState(-1, 0, 2),
                new BodyState(2, -10, -7),
                new BodyState(4, -8, 8),
                new BodyState(3, 5, -1),
            };

            var sim = new Simulation(scanData);
            sim.ExecuteStep();
            sim.Bodies[0].Position.ShouldBe((2, -1, 1));
            sim.Bodies[1].Position.ShouldBe((3, -7, -4));
            sim.Bodies[2].Position.ShouldBe((1, -7, 5));
            sim.Bodies[3].Position.ShouldBe((2, 2, 0));

            sim.Bodies[0].Velocity.ShouldBe((3, -1, -1));
            sim.Bodies[1].Velocity.ShouldBe((1, 3, 3));
            sim.Bodies[2].Velocity.ShouldBe((-3, 1, -3));
            sim.Bodies[3].Velocity.ShouldBe((-1, -3, 1));

            for (int i = 0; i < 9; i++)
                sim.ExecuteStep();

            sim.CalculateTotalEnergy().ShouldBe(179);
        }

        [TestMethod]
        public void Part1()
        {
            var scanData = new BodyState[]
            {
                new BodyState(10, 15, 7),
                new BodyState(15, 10, 0),
                new BodyState(20, 12, 3),
                new BodyState(0, -3, 13),
            };

            var sim = new Simulation(scanData);
            for (int i = 0; i < 1000; i++)
                sim.ExecuteStep();

            Console.WriteLine(sim.CalculateTotalEnergy());
        }

        [TestMethod]
        public void Part2ExampleTests()
        {
            var scanData = new BodyState[]
            {
                new BodyState(-1, 0, 2),
                new BodyState(2, -10, -7),
                new BodyState(4, -8, 8),
                new BodyState(3, 5, -1),
            };

            var sim = new Simulation(scanData);

            sim.CalculateFirstRepeatOfHistory().ShouldBe(2772);
        }

        [TestMethod]
        public void Part2()
        {
            var scanData = new BodyState[]
            {
                new BodyState(10, 15, 7),
                new BodyState(15, 10, 0),
                new BodyState(20, 12, 3),
                new BodyState(0, -3, 13),
            };

            var sim = new Simulation(scanData);
         
            Console.WriteLine(sim.CalculateFirstRepeatOfHistory());
        }
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace Day1
{
    class Program
    {
        static void Main(string[] args)
        {
            int totalFuel = 0;
            int mass = 0;
            int fuel = 0;
            var lines = File.ReadAllLines("input.txt");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            foreach (var line in lines)
            {
                mass = int.Parse(line);
                fuel = CalculateRecursiveFuel(mass);
                totalFuel += fuel;
            }

            Console.WriteLine($"Total fuel: {totalFuel} - in {sw.Elapsed}");

            sw.Restart();
            Console.WriteLine($"Total fuel: {lines.Sum(line => CalculateRecursiveFuel(int.Parse(line)))} - in {sw.Elapsed}");

            sw.Restart();
            totalFuel = 0;

            Parallel.ForEach(lines,  l => totalFuel += CalculateRecursiveFuel(int.Parse(l)));
            Console.WriteLine($"Total fuel: {totalFuel} - in {sw.Elapsed}");
        }

        private static int CalculateFuel(int mass)
        {
            return (mass / 3) - 2;
        }

        private static int CalculateRecursiveFuel(int mass)
        {
            var fuel = CalculateFuel(mass);
            var total = fuel;

            while ((fuel = CalculateFuel(fuel)) > 0)
            {
                total += fuel;
            }

            return total;
        }
    }
}

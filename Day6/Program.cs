using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;

namespace Day6
{
    class Program
    {
        static void Main(string[] args)
        {
            Part1();
            Part2();
        }

        private static void Part1()
        {
            var orbits = GetOrbits();

            var count = 0;

            foreach (var orbitingObject in orbits.Keys)
            {
                var o = orbitingObject;

                while (orbits.ContainsKey(o))
                {
                    count++;
                    o = orbits[o];
                }
            }

            Console.WriteLine(count);
        }

        private static Dictionary<string, string> GetOrbits()
        {
            var orbitDescriptions = File.ReadAllLines("input.txt");

            var orbits = orbitDescriptions.Select(d =>
            {
                var split = d.Split(')');
                return (centerObject: split[0], orbitingObject: split[1]);
            }).ToDictionary(e => e.orbitingObject, e => e.centerObject);

            return orbits;
        }

        private static void Part2()
        {
            var orbits = GetOrbits();

            var youParents = GetAncestors("YOU");
            var sanParents = GetAncestors("SAN");

            var common = youParents.Intersect(sanParents).First();
            var travelToCommon = youParents.IndexOf(common);
            var travelFromCommon = sanParents.IndexOf(common);

            Console.WriteLine(travelToCommon + travelFromCommon);
            
            List<string> GetAncestors(string node)
            {
                var result = new List<string>();
                var o = orbits[node];

                while (orbits.ContainsKey(o))
                {
                    result.Add(o);
                    o = orbits[o];
                }

                return result;
            }


        }

    }
}

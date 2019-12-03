using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day3
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("input.txt");

            var wire1 = new OccupiedCoordinates(lines[0]);
            var wire2 = new OccupiedCoordinates(lines[1]);

            var intersectionCoords = wire1.Keys.Intersect(wire2.Keys);

            var closestDistance = intersectionCoords.Min(c => wire1[c] + wire2[c]);
            Console.WriteLine(closestDistance); 
        }
    }

    public class OccupiedCoordinates : Dictionary<(int x, int y), int>
    {
        public OccupiedCoordinates(string path)
        {
            (int x, int y) coord = (0, 0);
            int steps = 0;

            foreach (var segment in path.Split(','))
            {
                switch (segment[0])
                {
                    case 'R':
                        AddItems((1, 0), segment);
                        break;
                    case 'L':
                        AddItems((-1, 0), segment);
                        break;
                    case 'U':
                        AddItems((0, 1), segment);
                        break;
                    case 'D':
                        AddItems((0, -1), segment);
                        break;
                    default: 
                        throw new InvalidOperationException("Invalid operation");
                }
            }

            void AddItems((int x, int y) direction, string segment)
            {
                var segmentLength = int.Parse(segment.Substring(1));

                for (int i = 0; i < segmentLength; i++)
                {
                    coord.x += direction.x;
                    coord.y += direction.y;

                    //This is how signals actually propagate. Once they reach a certain part, it spreads everywhere from that point.
                    //This is correct for the samples, but not the actual input.
                    //if (ContainsKey(coord))
                    //    steps = this[coord];
                    //else
                    //{
                    //    steps++;
                    //    Add(coord, steps);
                    //}

                    steps++;

                    //This works, but doesn't feel like signals travel like this.
                    if (!ContainsKey(coord))
                    {
                        Add(coord, steps);   
                    }


                }
            }

        }

    }
}

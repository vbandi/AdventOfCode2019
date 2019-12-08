using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day8
{
    class Program
    {
        const int width = 25;
        const int height = 6;

        static void Main(string[] args)
        {
            Part1();
            Part2();
        }

        private static void Part1()
        {
            var layers = GetLayers();

            Stopwatch sw = Stopwatch.StartNew();
            //pretty fast
            var fewest0Digits = layers.Min(l => l.Count(c => c == '0')); 
            var layerWithFewest0Digits = layers.Single(l => l.Count(c => c == '0') == fewest0Digits);
            Console.WriteLine(layerWithFewest0Digits.Count(c => c == '1') * layerWithFewest0Digits.Count(c => c == '2'));
            Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms with LINQ");

            sw.Restart();

            //much faster
            var min0s = int.MaxValue;
            string min0Layer = null;

            for (int i = 0; i < layers.Count; i++)
            {
                var zeros = layers[i].Count(c => c == '0');

                if (zeros < min0s)
                {
                    min0s = zeros;
                    min0Layer = layers[i];
                }
            }

            Console.WriteLine(min0Layer.Count(c => c == '1') * min0Layer.Count(c => c == '2'));
            Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms with less LINQ\n\n\n");
        }

        private static List<string> GetLayers()
        {
            string input = File.ReadAllText("Input.txt");

            var layers = new List<string>();

            for (var i = 0; i < input.Length; i += width * height)
                layers.Add(input.Substring(i, width * height));

            return layers;
        }

        private static void Part2()
        {
            var layers = GetLayers();

            var img = new char[height, width];

            foreach (var layer in layers)
            {
                for (int row = 0; row < height; row++)
                for (int column = 0; column < width; column++)
                {
                    var pixel = layer[row * width + column];

                    if (pixel != '2' && img[row,column] == 0)
                        img[row,column] = pixel == '1' ? '*' : ' ';
                }
            }

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                    Console.Write(img[i,j]);

                Console.WriteLine();
            }

        }
    }
}

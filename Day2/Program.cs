using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Day2
{
    class Program
    {
        private const int HCF = 99;
        private const int ADD = 1;
        private const int MUL = 2;


        static void Main(string[] args)
        {
            var text = File.ReadAllText("Input.txt");
            int[] memory;
            
            for (int noun = 0; noun <= 99; noun++)
                for (int verb = 0; verb <= 99; verb++)
                {
                    memory = ParseInput(text);
                    memory[1] = noun;
                    memory[2] = verb;
                    Execute(memory);

                    if (memory[0] == 19690720)
                    {
                        Console.WriteLine($"Got it! Result: {100*noun + verb}");
                        return;
                    }
                }


            //Console.WriteLine("Program finished. ");
            //Dump(memory);
        }

        private static void Dump(int[] memory)
        {
            Console.WriteLine("---------------");
            Console.WriteLine(String.Join(',', memory));
        }

        private static int[] ParseInput(string text)
        {
            int[] result = text.Split(',').Select(t => int.Parse(t)).ToArray();
            return result;
        }

        private static void Execute(int[] memory)
        {
            int instructionPointer = 0;

            while (memory[instructionPointer] != HCF)
            {
                switch (memory[instructionPointer])
                {
                    case ADD:
                        memory[memory[instructionPointer + 3]] =
                            memory[memory[instructionPointer + 1]] + memory[memory[instructionPointer + 2]];

                        instructionPointer += 4;
                        break;
                    case MUL:
                        memory[memory[instructionPointer + 3]] =
                            memory[memory[instructionPointer + 1]] * memory[memory[instructionPointer + 2]];

                        instructionPointer += 4;
                        break;
                    default: throw new InvalidOperationException($"Invalid instruction at {instructionPointer} : {memory[instructionPointer]}");
                }
                //Dump(memory);
            }
        }
    }
}

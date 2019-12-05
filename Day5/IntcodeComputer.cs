using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.Utilities;

namespace Day5
{
    public class IntcodeComputer
    {
        public const int HCF = 99;
        public const int ADD = 1;
        public const int MUL = 2;
        public const int INP = 3;
        public const int OUT = 4;

        public static (int[] memory, List<int> output) Execute(string text, int[] inputs = null)
        {
            var memory = ParseInput(text);
            var output = Execute(memory, inputs);
            return (memory, output);
        }

        private static int[] ParseInput(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new InvalidOperationException("Missing HCF");

            int[] result = text.Split(',').Select(t => int.Parse(t)).ToArray();
            return result;
        }

        private static List<int> Execute(int[] memory, IEnumerable<int> inputs)
        {
            List<int> output = new List<int>();
            int instructionPointer = 0;

            IEnumerator<int> inputEnumerator = null;

            if (inputs != null)
            {
                inputEnumerator = inputs.GetEnumerator();
            }

            while (memory[instructionPointer] != HCF)
            {
                var (opCode, parameterModes) = CalculateOpCodeAndParameterModes(memory[instructionPointer]);

                switch (opCode)
                {
                    case ADD:
                        memory[memory[instructionPointer + 3]] =
                            GetValue(memory[instructionPointer + 1], parameterModes[0]) +
                            GetValue(memory[instructionPointer + 2], parameterModes[1]);

                        instructionPointer += 4;
                        break;
                    
                    case MUL:
                        memory[memory[instructionPointer + 3]] =
                            GetValue(memory[instructionPointer + 1], parameterModes[0]) *
                            GetValue(memory[instructionPointer + 2], parameterModes[1]);

                        instructionPointer += 4;
                        break;
                    
                    case INP:
                        
                        if (inputEnumerator == null)
                            throw new InvalidOperationException("Encountered INP while inputs was null");
                        
                        if (!inputEnumerator.MoveNext())
                            throw new InvalidOperationException("Encountered INP while inputs has ran out of data");

                        memory[memory[instructionPointer + 1]] = inputEnumerator.Current;

                        instructionPointer += 2;
                        break;

                    case OUT:
                        output.Add(GetValue(memory[instructionPointer + 1], parameterModes[0]));
                        instructionPointer += 2;
                        break;


                    default: throw new InvalidOperationException($"Invalid instruction at {instructionPointer} : {memory[instructionPointer]}");
                }

                if (memory.Length < instructionPointer + 1)
                    throw new InvalidOperationException("Missing HCF");
            }

            return output;

            int GetValue(int parameter, ParameterModes parameterMode)
            {
                return parameterMode == ParameterModes.Immediate ? parameter : memory[parameter];
            }
        }



        public static (int opCode, ParameterModes[] parameterModes) CalculateOpCodeAndParameterModes(int i)
        {
            //always 3 parameter modes for now
            var s = i.ToString("D5", CultureInfo.InvariantCulture);
            
            var opCode = int.Parse(s.Substring(3));
            
            var parameterModes = new ParameterModes[3];
            parameterModes[0] = (ParameterModes) int.Parse(s[2].ToString());
            parameterModes[1] = (ParameterModes) int.Parse(s[1].ToString());
            parameterModes[2] = (ParameterModes) int.Parse(s[0].ToString());

            return (opCode, parameterModes);
        }
    }

    public enum ParameterModes
    {
        Position = 0,
        Immediate = 1
    }
}

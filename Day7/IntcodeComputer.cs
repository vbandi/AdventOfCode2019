using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Day7
{
    public class IntCodeComputer
    {
        public const int HCF = 99;
        public const int ADD = 1;
        public const int MUL = 2;
        public const int INP = 3;
        public const int OUT = 4;
        public const int JNZ = 5;
        public const int JZ = 6;
        public const int LT = 7;
        public const int EQ = 8;

        private int[] memory;
        private int instructionPointer;

        public BufferBlock<int> input { get; } = new BufferBlock<int>();
        public BufferBlock<int> output{ get; } = new BufferBlock<int>();
        public List<int> OutputList { get; } = new List<int>();

        public string Name { get; set; }

        public async Task<(int[] memory, List<int> output)> ExecuteAsync(string text)
        {
            memory = ParseInput(text);
            await ExecuteAsync();
            return (memory, OutputList);
        }

        private static int[] ParseInput(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new InvalidOperationException("Missing HCF");

            int[] result = text.Split(',').Select(t => int.Parse(t)).ToArray();
            return result;
        }

        private async Task ExecuteAsync()
        {
            instructionPointer = 0;

            while (memory[instructionPointer] != HCF)
            {
                var (opCode, parameterModes) = CalculateOpCodeAndParameterModes(memory[instructionPointer]);

                switch (opCode)
                {
                    case ADD:
                        memory[memory[instructionPointer + 3]] = GetParameterValue(0) + GetParameterValue(1);
                        instructionPointer += 4;
                        break;
                    
                    case MUL:
                        memory[memory[instructionPointer + 3]] = GetParameterValue(0) * GetParameterValue(1);
                        instructionPointer += 4;
                        break;
                    
                    case INP:
                        //throw new InvalidOperationException("Encountered INP while inputs has ran out of data");

                        memory[memory[instructionPointer + 1]] = await input.ReceiveAsync();
                        instructionPointer += 2;
                        break;

                    case OUT:
                        var o = GetParameterValue(0);
                        output.Post(o);
                        OutputList.Add(o);
                        instructionPointer += 2;
                        break;

                    case JNZ:
                        instructionPointer = GetParameterValue(0) != 0 ? GetParameterValue(1) : instructionPointer + 3;
                        break;

                    case JZ:
                        instructionPointer = GetParameterValue(0) == 0 ? GetParameterValue(1) : instructionPointer + 3;
                        break;

                    case LT:
                        memory[memory[instructionPointer + 3]] = GetParameterValue(0) < GetParameterValue(1) ? 1 : 0;
                        instructionPointer += 4;
                        break;
                    
                    case EQ:
                        memory[memory[instructionPointer + 3]] = GetParameterValue(0) == GetParameterValue(1) ? 1 : 0;
                        instructionPointer += 4;
                        break;

                    default: throw new InvalidOperationException($"Invalid instruction at {instructionPointer} : {memory[instructionPointer]}");

                        // returns parameter value in both immediate and position mode
                    int GetParameterValue(int parameterIndex)
                    {
                        int parameter = memory[instructionPointer + parameterIndex + 1];
                        return parameterModes[parameterIndex] == ParameterModes.Immediate ? parameter : memory[parameter];
                    }
                }

                if (memory.Length < instructionPointer + 1)
                    throw new InvalidOperationException("Missing HCF");
            }
        }

        public static (int opCode, ParameterModes[] parameterModes) CalculateOpCodeAndParameterModes(int i)
        {
            // HACK: always 3 parameter modes for now
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

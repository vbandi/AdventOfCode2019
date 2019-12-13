using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Day11
{
    public class IntCodeComputer
    {
        public const int HCF = 99;   //halt and catch fire
        public const int ADD = 1;    //add param 0 and 1, and store it in param 2
        public const int MUL = 2;    //multiply param 0 and 1, and store it in param 2
        public const int INP = 3;    //input a number and store it in param 0
        public const int OUT = 4;    //output param 0
        public const int JNZ = 5;    //jump if param 0 is not zero
        public const int JZ = 6;     //jump if param 0 is zero
        public const int LT = 7;     //store 1 to param 3 if param 0 is less than param 1, or 0 otherwise
        public const int EQ = 8;     //store 1 to param 3 if param 0 equals param 1
        public const int SETREL = 9; //set the relative base to param 0

        private long[] memory;
        private long instructionPointer;
        private long relativeBase = 0;

        private Dictionary<long, long> additionalMemory = new Dictionary<long, long>();

        public BufferBlock<long> input { get; } = new BufferBlock<long>();
        public BufferBlock<long> output{ get; } = new BufferBlock<long>();
        public List<long> OutputList { get; } = new List<long>();

        private long? inputOverride = null;

        public string Name { get; set; }

        public async Task<(long[] memory, List<long> output)> ExecuteAsync(string text)
        {
            memory = ParseInput(text);
            await ExecuteAsync();
            return (memory, OutputList);
        }

        private static long[] ParseInput(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new InvalidOperationException("Missing HCF");

            var result = text.Split(',').Select(t => long.Parse(t)).ToArray();
            return result;
        }

        public void OverrideInputBuffer(long? value)
        {
            inputOverride = value;
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
                        SetParameterValue(2,GetParameterValue(0) + GetParameterValue(1));
                        instructionPointer += 4;
                        break;
                    
                    case MUL:
                        SetParameterValue(2, GetParameterValue(0) * GetParameterValue(1));
                        instructionPointer += 4;
                        break;
                    
                    case INP:
                        //throw new InvalidOperationException("Encountered INP while inputs has ran out of data");
                        if (inputOverride.HasValue)
                            SetParameterValue(0, inputOverride.Value);
                        else
                            SetParameterValue(0, await input.ReceiveAsync());

                        instructionPointer += 2;
                        break;

                    case OUT:
                        var o = GetParameterValue(0);
                        output.Post(o);
                        OutputList.Add(o);
                        OutputHappened?.Invoke(this, o);
                        instructionPointer += 2;
                        break;

                    case JNZ:
                        instructionPointer = GetParameterValue(0) != 0 ? GetParameterValue(1) : instructionPointer + 3;
                        break;

                    case JZ:
                        instructionPointer = GetParameterValue(0) == 0 ? GetParameterValue(1) : instructionPointer + 3;
                        break;

                    case LT:
                        SetParameterValue(2, GetParameterValue(0) < GetParameterValue(1) ? 1 : 0);
                        instructionPointer += 4;
                        break;
                    
                    case EQ:
                        SetParameterValue(2, GetParameterValue(0) == GetParameterValue(1) ? 1 : 0);
                        instructionPointer += 4;
                        break;

                    case SETREL:
                        relativeBase += GetParameterValue(0);
                        instructionPointer += 2;
                        break;

                    default: throw new InvalidOperationException($"Invalid instruction at {instructionPointer} : {memory[instructionPointer]}");

                    // returns parameter value in both immediate and position mode
                    long GetParameterValue(int parameterIndex)
                    {
                        long parameter = ReadMemory(instructionPointer + parameterIndex + 1);
                        long result;

                        switch (parameterModes[parameterIndex])
                        {
                            case ParameterModes.Position:
                                result = ReadMemory(parameter);
                                break;
                                case ParameterModes.Immediate:
                                    result = parameter;
                                    break;
                            case ParameterModes.Relative:
                                result = ReadMemory(relativeBase + parameter);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        return result;
                    }

                    void SetParameterValue(int parameterIndex, long value)
                    {
                        var parameter = ReadMemory(instructionPointer + parameterIndex + 1);
                        int result;

                        switch (parameterModes[parameterIndex])
                        {
                            case ParameterModes.Position:
                                WriteMemory(parameter, value);
                                break;
                            case ParameterModes.Relative:
                                WriteMemory(relativeBase + parameter, value);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }

                if (memory.Length < instructionPointer + 1)
                    throw new InvalidOperationException("Missing HCF");
            }
        }

        private long ReadMemory(long address)
        {
            if (address < memory.Length)
                return memory[address];
            else
            {
                return additionalMemory.ContainsKey(address) ? additionalMemory[address] : 0;
            }

        }

        private void WriteMemory(long address, long value)
        {
            if (address < memory.Length)
                memory[address] = value;
            else
                additionalMemory[address] = value;
        }

        public static (int opCode, ParameterModes[] parameterModes) CalculateOpCodeAndParameterModes(long i)
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

        public event EventHandler<long> OutputHappened;
    }

    public enum ParameterModes
    {
        Position = 0,
        Immediate = 1,
        Relative = 2
    }
}

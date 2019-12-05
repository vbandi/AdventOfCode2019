using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Day5
{
    [TestClass]
    public class Day5Tests
    {
        [DataTestMethod]
        [DataRow("1,9,10,3,2,3,11,0,99,30,40,50", 0, 3500)]
        [DataRow("1,0,0,0,99", 0, 2)]
        [DataRow("2,3,0,3,99", 3, 6)]
        [DataRow("2,4,4,5,99,0", 5, 9801)]
        [DataRow("1,1,1,4,99,5,6,0,99", 0, 30)]
        public void VerifySimpleAdditionAndMultiplication(string input, int positionToVerify, int expectedValueAtPosition)
        {
            var result = IntcodeComputer.Execute(input, null);
            result.output.ShouldBeEmpty();
            result.memory[positionToVerify].ShouldBe(expectedValueAtPosition);
        }

        [TestMethod]
        public void VerifyMissingHCF()
        {
            Action a = () => IntcodeComputer.Execute("");
            a.ShouldThrow(typeof(InvalidOperationException));
            
            a = () => IntcodeComputer.Execute("1,0,0,0");
            a.ShouldThrow(typeof(InvalidOperationException));
        }

        [DataTestMethod]
        [DataRow("3,1,99", new[] {42}, 1, 42)]
        [DataRow("3,1,3,1,99", new[] {42, 43}, 1, 43)]
        public void VerifyINP(string input, int[] inputs, int positionToVerify, int expectedValueAtPosition)
        {
            var result = IntcodeComputer.Execute(input, inputs); 
            result.output.ShouldBeEmpty();
            result.memory[positionToVerify].ShouldBe(expectedValueAtPosition);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void INPShouldThrowIfInputsNull()
        {
            IntcodeComputer.Execute("3,1,99", null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void INPShouldThrowIfInputsEmpty()
        {
            IntcodeComputer.Execute("3,1,99", new int[]{});
        }

        [TestMethod]
        public void VerifyOUT()
        {
            var result = IntcodeComputer.Execute("4,0,99");
            result.output.Count.ShouldBe(1);
            result.output[0].ShouldBe(4);
        }

        [TestMethod]
        public void VerifyIINPOUT()
        {
            var result = IntcodeComputer.Execute("3,0,4,0,99", new[] {42});
            result.output.SequenceEqual(new [] {42});
        }

        [TestMethod]
        public void VerifyImmediateOUT()
        {
            var result = IntcodeComputer.Execute("104,42,99");
            result.output.SequenceEqual(new[] {42});
        }

        [TestMethod]
        public void VerifyImmediateADD()
        {
            var result = IntcodeComputer.Execute("1101,41,1,0,99");
            result.memory[0].ShouldBe(42);
        }

        [TestMethod]
        public void VerifyImmediateMUL()
        {
            var result = IntcodeComputer.Execute("1102,21,2,0,99");
            result.memory[0].ShouldBe(42);
        }

        [DataTestMethod]
        [DataRow(99, 99, new ParameterModes[]{ParameterModes.Position, ParameterModes.Position, ParameterModes.Position})]
        [DataRow(00099, 99, new ParameterModes[]{ParameterModes.Position, ParameterModes.Position, ParameterModes.Position})]
        [DataRow(00199, 99, new ParameterModes[]{ParameterModes.Immediate, ParameterModes.Position, ParameterModes.Position})]
        [DataRow(10199, 99, new ParameterModes[]{ParameterModes.Immediate, ParameterModes.Position, ParameterModes.Immediate})]
        [DataRow(11199, 99, new ParameterModes[]{ParameterModes.Immediate, ParameterModes.Immediate, ParameterModes.Immediate})]
        public void VerifyCalculateOpCodeAndParameterModes(int instruction, int expectedOpCode, ParameterModes[] expectedParameterModes )
        {
            var result = IntcodeComputer.CalculateOpCodeAndParameterModes(instruction);
            result.opCode.ShouldBe(expectedOpCode);
            result.parameterModes.SequenceEqual(expectedParameterModes).ShouldBeTrue();
        }

        [TestMethod]
        public void RunDay5()
        {
            var program =
                "3,225,1,225,6,6,1100,1,238,225,104,0,101,67,166,224,1001,224,-110,224,4,224,102,8,223,223,1001,224,4,224,1,224,223,223,2,62,66,224,101,-406,224,224,4,224,102,8,223,223,101,3,224,224,1,224,223,223,1101,76,51,225,1101,51,29,225,1102,57,14,225,1102,64,48,224,1001,224,-3072,224,4,224,102,8,223,223,1001,224,1,224,1,224,223,223,1001,217,90,224,1001,224,-101,224,4,224,1002,223,8,223,1001,224,2,224,1,223,224,223,1101,57,55,224,1001,224,-112,224,4,224,102,8,223,223,1001,224,7,224,1,223,224,223,1102,5,62,225,1102,49,68,225,102,40,140,224,101,-2720,224,224,4,224,1002,223,8,223,1001,224,4,224,1,223,224,223,1101,92,43,225,1101,93,21,225,1002,170,31,224,101,-651,224,224,4,224,102,8,223,223,101,4,224,224,1,223,224,223,1,136,57,224,1001,224,-138,224,4,224,102,8,223,223,101,2,224,224,1,223,224,223,1102,11,85,225,4,223,99,0,0,0,677,0,0,0,0,0,0,0,0,0,0,0,1105,0,99999,1105,227,247,1105,1,99999,1005,227,99999,1005,0,256,1105,1,99999,1106,227,99999,1106,0,265,1105,1,99999,1006,0,99999,1006,227,274,1105,1,99999,1105,1,280,1105,1,99999,1,225,225,225,1101,294,0,0,105,1,0,1105,1,99999,1106,0,300,1105,1,99999,1,225,225,225,1101,314,0,0,106,0,0,1105,1,99999,1107,226,226,224,102,2,223,223,1006,224,329,1001,223,1,223,1007,226,677,224,1002,223,2,223,1005,224,344,101,1,223,223,108,677,677,224,1002,223,2,223,1006,224,359,101,1,223,223,1008,226,226,224,1002,223,2,223,1005,224,374,1001,223,1,223,108,677,226,224,1002,223,2,223,1006,224,389,101,1,223,223,7,226,226,224,102,2,223,223,1006,224,404,101,1,223,223,7,677,226,224,1002,223,2,223,1005,224,419,101,1,223,223,107,226,226,224,102,2,223,223,1006,224,434,1001,223,1,223,1008,677,677,224,1002,223,2,223,1005,224,449,101,1,223,223,108,226,226,224,102,2,223,223,1005,224,464,1001,223,1,223,1108,226,677,224,1002,223,2,223,1005,224,479,1001,223,1,223,8,677,226,224,102,2,223,223,1006,224,494,1001,223,1,223,1108,677,677,224,102,2,223,223,1006,224,509,1001,223,1,223,1007,226,226,224,1002,223,2,223,1005,224,524,1001,223,1,223,7,226,677,224,1002,223,2,223,1005,224,539,1001,223,1,223,8,677,677,224,102,2,223,223,1005,224,554,1001,223,1,223,107,226,677,224,1002,223,2,223,1006,224,569,101,1,223,223,1107,226,677,224,102,2,223,223,1005,224,584,1001,223,1,223,1108,677,226,224,102,2,223,223,1006,224,599,1001,223,1,223,1008,677,226,224,102,2,223,223,1006,224,614,101,1,223,223,107,677,677,224,102,2,223,223,1006,224,629,1001,223,1,223,1107,677,226,224,1002,223,2,223,1005,224,644,101,1,223,223,8,226,677,224,102,2,223,223,1005,224,659,1001,223,1,223,1007,677,677,224,102,2,223,223,1005,224,674,1001,223,1,223,4,223,99,226";

            var inputs = new int[] {1};
            var result = IntcodeComputer.Execute(program, inputs);
            Console.WriteLine(String.Join(',', result.output));
        }

    }
}

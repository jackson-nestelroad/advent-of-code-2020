using AdventOfCode2020.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2020.Days
{
    [Solution(8)]
    class Day08 : ISolution
    {
        struct Instruction
        {
            public enum Opcode
            {
                acc,
                jmp,
                nop
            }

            public readonly Opcode Operation { get; init; }
            public readonly int Argument { get; init; }
        }

        class HandheldGameConsole
        {
            private List<Instruction> Program = null;
            private int PC;
            public int Accumulator { get; private set; }

            public bool Finished => PC >= Program.Count;

            public HandheldGameConsole()
            {
                Reset();
            }

            public void Load(List<Instruction> program)
            {
                Program = program;
            }

            public void Reset()
            {
                PC = 0;
                Accumulator = 0;
            }

            private void Execute(Instruction instr)
            {
                switch (instr.Operation)
                {
                    case Instruction.Opcode.acc: Accumulator += instr.Argument; break;
                    case Instruction.Opcode.jmp: PC += instr.Argument - 1; break;
                    case Instruction.Opcode.nop: break;
                }
            }

            public void Run()
            {
                while (PC < Program.Count)
                {
                    Execute(Program[PC]);
                    ++PC;
                }
            }

            public void RunUntilInfiniteLoop()
            {
                HashSet<int> history = new HashSet<int>();
                while (!history.Contains(PC) && PC < Program.Count)
                {
                    history.Add(PC);
                    Execute(Program[PC]);
                    ++PC;
                }
            }

            public bool FixInfiniteLoop()
            {
                // Get infinite loop and order of execution through the loop
                Dictionary<int, int> history = new Dictionary<int, int>();
                int time = 0;
                while (!history.ContainsKey(PC))
                {
                    history.Add(PC, time++);
                    Execute(Program[PC]);
                    ++PC;
                }

                int timeEnteredInfiniteLoop = history[PC];
                IEnumerable<int> infiniteLoop = history.Where(pair => pair.Value >= timeEnteredInfiniteLoop).OrderBy(pair => pair.Value).Select(pair => pair.Key);
                IEnumerable<int> potentialCorrupted = infiniteLoop.Where(addr => Program[addr].Operation == Instruction.Opcode.jmp || Program[addr].Operation == Instruction.Opcode.nop);

                // Attempt to switch each potentially corrupted instruction and check for infinite loop
                // We fixed the loop when the program finishes
                foreach (int addr in potentialCorrupted)
                {
                    Reset();
                    Instruction original = Program[addr];
                    Program[addr] = new Instruction() { 
                        Operation = original.Operation == Instruction.Opcode.jmp ? Instruction.Opcode.nop : Instruction.Opcode.jmp,
                        Argument = original.Argument
                    };
                    RunUntilInfiniteLoop();
                    if (Finished)
                    {
                        return true;
                    }
                    Program[addr] = original;
                }

                return false;
            }
        }

        private static List<Instruction> ParseInput(string input)
        {
            return input.Lines().Select((line, i) =>
            {
                string opcodeString = line[0..3];
                if (!Enum.TryParse(opcodeString, out Instruction.Opcode opcode))
                {
                    throw new InputParseException($"Invalid opcode \"{opcodeString}\" at index {i}");
                }
                return new Instruction() { Operation = opcode, Argument = int.Parse(line[4..]) };
            }).ToList();
        }

        public object PartA(string input)
        {
            HandheldGameConsole console = new HandheldGameConsole();
            console.Load(ParseInput(input));
            console.RunUntilInfiniteLoop();
            return console.Accumulator;
        }

        public object PartB(string input)
        {
            HandheldGameConsole console = new HandheldGameConsole();
            console.Load(ParseInput(input));
            if (!console.FixInfiniteLoop())
            {
                throw new SolutionFailedException("Could not fix infinite loop");
            }
            return console.Accumulator;
        }
    }
}

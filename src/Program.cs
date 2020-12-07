using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AdventOfCode2020
{
    class Program
    {
        private static readonly int FirstDay = 1;
        private static readonly int LastDay = 25;

        private static void PrintUsage()
        {
            Console.Error.WriteLine($"USAGE: {Environment.GetCommandLineArgs()[0]} [DAY]");
        }

        public static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine("Missing day command-line parameter.");
                PrintUsage();
                return 1;
            }

            // Command-line parameter must be a valid day to run
            bool parsed = int.TryParse(args[0], out int day);
            if (!parsed || day < FirstDay || day > LastDay)
            {
                Console.Error.WriteLine($"Day parameter must be an integer between {FirstDay} and {LastDay}.");
                return 1;
            }

            bool test = false;
            if (args.Length >= 2 && bool.TryParse(args[1], out bool test_param))
            {
                test = test_param;
            }

            // Find class for the given day's solution
            // Must implement ISolution and have SolutionAttribute
            var types = typeof(Program).Assembly.GetTypes();
            var solutionClass = Array.Find(Assembly.GetEntryAssembly().GetTypes(), type =>
            {
                return type.GetInterfaces().Contains(typeof(ISolution))
                    && day == ((SolutionAttribute)Attribute.GetCustomAttribute(type, typeof(SolutionAttribute)))?.Day;
            });

            if (solutionClass == null)
            {
                Console.Error.WriteLine($"Solution class for day {day} could not be found.");
                return 1;
            }

            // Read input file
            string fileName = $"./{(test ? "Test " : "")}Input/{day:00}.txt";
            if (!File.Exists(fileName))
            {
                Console.Error.WriteLine($"File \"{fileName}\" does not exist.");
                return 1;
            }

            string input = File.ReadAllText(fileName);
            ISolution solution = (ISolution)Activator.CreateInstance(solutionClass);

            try
            {
                // Run part A
                var watch = System.Diagnostics.Stopwatch.StartNew();
                object partA = solution.PartA(input);
                watch.Stop();
                Console.WriteLine($"Part A: {partA} (took {watch.ElapsedMilliseconds}ms)");

                // Run part B
                watch.Restart();
                object partB = solution.PartB(input);
                watch.Stop();
                Console.WriteLine($"Part B: {partB} (took {watch.ElapsedMilliseconds}ms)");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return 1;
            }

            return 0;
        }
    }
}

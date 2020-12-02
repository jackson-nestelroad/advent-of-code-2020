using System;

namespace AdventOfCode2020
{
    public interface ISolution
    {
        public abstract object PartA(string[] input);
        public abstract object PartB(string[] input);
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class SolutionAttribute : Attribute
    {
        public SolutionAttribute(int day)
        {
            Day = day;
        }

        public int Day { get; }
    }

    [Serializable()]
    public class SolutionFailedException : Exception
    {
        public SolutionFailedException(string message = "Solution failed to produce any result.") : base(message) { }
    }

    [Serializable()]
    public class InputParseException : Exception
    {
        public InputParseException(string message = "Solution failed to parse the given input.") : base(message) { }
    }
}

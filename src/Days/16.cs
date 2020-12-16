using AdventOfCode2020.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2020.Days
{
    [Solution(16)]
    class Day16 : ISolution
    {
        private static readonly Regex FieldRegex = new Regex(@"^([a-z ]+): (\d+)-(\d+) or (\d+)-(\d+)$");

        private struct Range
        {
            public int Min;
            public int Max;

            public Range(int min, int max)
            {
                Min = min;
                Max = max;
            }

            public bool Contains(int n) => n >= Min && n <= Max;
        }

        private class TicketField
        {
            public string Name { get; init; }
            private Range FirstRange;
            private Range SecondRange;

            public TicketField(string name, int min1, int max1, int min2, int max2)
            {
                Name = name;
                FirstRange = new Range(min1, max1);
                SecondRange = new Range(min2, max2);
            }

            public bool ValidateValue(int n) => FirstRange.Contains(n) || SecondRange.Contains(n);
        }

        private static List<int> ParseTicket(string ticket)
        {
            return ticket.Split(',').Select(n => int.Parse(n)).ToList();
        }

        private static void ParseInput(string input, out List<TicketField> fields, out List<int> yourTicket, out List<List<int>> nearbyTickets)
        {
            IEnumerable<string[]> inputGroups = input.Lines(2).Select(str => str.Lines());

            // Parse ticket fields
            fields = new List<TicketField>();
            foreach (string field in inputGroups.ElementAt(0))
            {
                Match match = FieldRegex.Match(field);
                if (!match.Success)
                {
                    throw new InputParseException("Invalid ticket field");
                }
                GroupCollection groups = match.Groups;
                fields.Add(new TicketField(
                    groups[1].Value,
                    int.Parse(groups[2].Value),
                    int.Parse(groups[3].Value),
                    int.Parse(groups[4].Value),
                    int.Parse(groups[5].Value)
                ));
            }

            // Parse your ticket
            yourTicket = ParseTicket(inputGroups.ElementAt(1).Last());

            // Parse nearby tickets
            nearbyTickets = inputGroups.ElementAt(2).Skip(1).Select(ticket => ParseTicket(ticket)).ToList();
        }

        public object PartA(string input)
        {
            ParseInput(input, out List<TicketField> fields, out List<int> yourTicket, out List<List<int>> nearbyTickets);

            int errorRate = 0;
            foreach(List<int> ticket in nearbyTickets)
            {
                foreach (int value in ticket)
                {
                    if (fields.All(field => !field.ValidateValue(value)))
                    {
                        errorRate += value;
                    }
                }
            }
            return errorRate;
        }

        public object PartB(string input)
        {
            ParseInput(input, out List<TicketField> fields, out List<int> yourTicket, out List<List<int>> nearbyTickets);

            // Discard all invalid tickets
            nearbyTickets.RemoveAll(ticket => ticket.Any(value => fields.All(field => !field.ValidateValue(value))));

            // Maps a ticket field to the index it is assigned to
            Dictionary<TicketField, int> fieldAssignments = new Dictionary<TicketField, int>();

            // Map of indices with potential tickets that could not be assigned when iterated over
            // This means the set contains 2 or more potential fields
            // A later iteration must narrow the selection down by assigning those fields until only one remains
            Dictionary<int, HashSet<TicketField>> unassigned = new Dictionary<int, HashSet<TicketField>>();

            int totalFields = fields.Count;
            for (int f = 0; f < totalFields; ++f)
            {
                // Keep a set valid fields that could potentially be assigned to index f
                bool assigned = false;
                HashSet<TicketField> potential = fields.ToHashSet();
                for (int i = 0; i < nearbyTickets.Count && !assigned; ++i)
                {
                    potential.IntersectWith(fields.Where(field => field.ValidateValue(nearbyTickets[i][f])));

                    // Stop iterating when narrowed down to 1
                    if (potential.Count == 1)
                    {
                        TicketField field = potential.First();
                        fieldAssignments[field] = f;
                        fields.Remove(field);
                        assigned = true;
                    }
                }

                // Field assignment could not be determined with one iteration
                if (!assigned)
                {
                    if (potential.Count == 0)
                    {
                        throw new SolutionFailedException($"No possible field assignments for ticket index {f}");
                    }

                    // Save potential assignments
                    unassigned[f] = potential;
                }
            }

            // Loop until no potential assignments left
            while (unassigned.Count > 0)
            {
                foreach ((int index, HashSet<TicketField> potential) in unassigned)
                {
                    // Remove fields that have been assigned
                    potential.RemoveWhere(field => fieldAssignments.ContainsKey(field));

                    // This field can now be assigned
                    if (potential.Count == 1)
                    {
                        TicketField field = potential.First();
                        fieldAssignments[field] = index;
                        unassigned.Remove(index);
                    }
                }
            }

            // Use assignments to calculate product
            return fieldAssignments.Where(pair => pair.Key.Name.StartsWith("departure")).Aggregate(1L, (prod, pair) => prod * yourTicket[pair.Value]);
        }
    }
}

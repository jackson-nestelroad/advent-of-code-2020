using AdventOfCode2020.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2020.Days
{
    [Solution(23)]
    class Day23 : ISolution
    {
        private static IEnumerable<int> ParseInput(string input) => input.Lines().First().ToCharArray().Select(c => c - '0');
        public object PartA(string input)
        {
            const int moves = 100;

            // Custom circular linked list implementation for part A
            // It models the problem perfectly, and it was a good implementation exercise

            CircularLinkedList<int> cups = new CircularLinkedList<int>(ParseInput(input));
            int highestValue = cups.Max();

            CircularLinkedList<int>.Node current = cups.Head;
            for (int move = 0; move < moves; ++move, ++current)
            {
                int currentCup = current.Value;
                
                // Remove next three cups
                CircularLinkedList<int> removed = cups.Splice(current.Next, 3);

                // Find destination cup
                CircularLinkedList<int>.Node destination = null;
                for (int target = currentCup == 1 ? highestValue : currentCup -  1; destination == null; target = target == 1 ? highestValue : target - 1)
                {
                    destination = cups.Find(target);
                }

                cups.InsertAfter(destination, removed);
            }

            int result = 0;
            CircularLinkedList<int>.Node oneNode = cups.Find(1);
            CircularLinkedList<int>.Node it = oneNode.Next;
            while (it != oneNode)
            {
                result *= 10;
                result += it.Value;
                ++it;
            }

            return result;
        }
        public object PartB(string input)
        {
            const int highestCup = 1_000_000;
            const int moves = 10_000_000;

            // Part B is an optimization of Part A
            // Opposed to using a circular linked list to model the problem,
            // a single array that that maps a cup to the next cup in the circle is used

            List<int> initialCups = ParseInput(input).ToList();
            int[] nextCup = new int[highestCup + 1];

            nextCup[highestCup] = initialCups[0];
            nextCup[initialCups[^1]] = initialCups.Count + 1;
            for (int i = 0; i < initialCups.Count - 1; ++i)
            {
                nextCup[initialCups[i]] = initialCups[i + 1];
            }
            for (int i = initialCups.Count + 1; i < highestCup; ++i)
            {
                nextCup[i] = i + 1;
            }

            for (int move = 0, currentCup = initialCups[0]; move < moves; ++move, currentCup = nextCup[currentCup])
            {
                int firstNext = nextCup[currentCup];
                int secondNext = nextCup[firstNext];
                int thirdNext = nextCup[secondNext];
                int fourthNext = nextCup[thirdNext];

                // Get next destination cup
                // Huge optimization by just finding the first destination that is not in the group of 3 being removed
                int destinationCup = currentCup == 1 ? highestCup : currentCup - 1;
                while (destinationCup == firstNext || destinationCup == secondNext || destinationCup == thirdNext)
                {
                    destinationCup = destinationCup == 1 ? highestCup : destinationCup - 1;
                }

                // Update "pointers"
                int afterDestination = nextCup[destinationCup];
                nextCup[currentCup] = fourthNext;
                nextCup[destinationCup] = firstNext;
                nextCup[thirdNext] = afterDestination;
            }

            return (long)nextCup[1] * nextCup[nextCup[1]];
        }
    }
}

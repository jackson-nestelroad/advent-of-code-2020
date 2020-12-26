using AdventOfCode2020.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2020.Days
{
    [Solution(22)]
    class Day22 : ISolution
    {
        private class Player
        {
            public int Id;
            public Queue<int> Hand;

            public Player(int id)
            {
                Id = id;
                Hand = new Queue<int>();
            }

            public Player(Player copy, int handSize)
            {
                Id = copy.Id;
                Hand = new Queue<int>(copy.Hand.Take(handSize));
            }
        }

        private static (Player Player1, Player Player2) ParseInput(string input)
        {
            List<IEnumerable<int>> lines = input.Lines(2).Select(group => group.Lines().Skip(1).Select(s => int.Parse(s))).ToList();
            return 
                (
                    new Player(1) { Hand = new Queue<int>(lines[0]) },
                    new Player(2) { Hand = new Queue<int>(lines[1]) }
                );
        }

        public object PartA(string input)
        {
            (Player player1, Player player2) = ParseInput(input);

            while (player1.Hand.Count > 0 && player2.Hand.Count > 0)
            {
                int player1Card = player1.Hand.Dequeue();
                int player2Card = player2.Hand.Dequeue();

                if (player1Card > player2Card)
                {
                    player1.Hand.Enqueue(player1Card);
                    player1.Hand.Enqueue(player2Card);
                }
                else
                {
                    player2.Hand.Enqueue(player2Card);
                    player2.Hand.Enqueue(player1Card);
                }
            }

            Player winner = player1.Hand.Count == 0 ? player2 : player1;
            int handSize = winner.Hand.Count;
            return winner.Hand
                .Select((card, index) => (Card: card, Index: handSize - index))
                .Aggregate(0, (score, pair) => score + pair.Card * pair.Index);
        }

        private class RecursiveCombat
        {
            public Player Player1 { get; }
            public Player Player2 { get; }

            private HashSet<int> player1History = new HashSet<int>();
            private HashSet<int> player2History = new HashSet<int>();

            public RecursiveCombat(Player player1, Player player2)
            {
                Player1 = player1;
                Player2 = player2;
            }

            private int HashHand(Queue<int> hand)
            {
                // Make sure these are prime numbers...
                const int seed = 51;
                const int modifier = 31;

                return hand.Aggregate(seed, (current, item) => (current * modifier) + item.GetHashCode());
            }

            public Player Play()
            {
                while (Player1.Hand.Count > 0 && Player2.Hand.Count > 0)
                {
                    int player1HandHash = HashHand(Player1.Hand);
                    int player2HandHash = HashHand(Player2.Hand);

                    // Avoid infinite recursion
                    if (player1History.Contains(player1HandHash) && player2History.Contains(player2HandHash))
                    {
                        return Player1;
                    }

                    // Add the current state
                    player1History.Add(player1HandHash);
                    player2History.Add(player2HandHash);

                    int player1Card = Player1.Hand.Dequeue();
                    int player2Card = Player2.Hand.Dequeue();

                    Player winner;

                    // Recursive game
                    if (player1Card <= Player1.Hand.Count && player2Card <= Player2.Hand.Count)
                    {
                        RecursiveCombat nextGame = new RecursiveCombat(new Player(Player1, player1Card), new Player(Player2, player2Card));
                        winner = nextGame.Play();
                    }
                    else
                    {
                        winner = player1Card > player2Card ? Player1 : Player2;
                    }

                    if (winner.Id == Player1.Id)
                    {
                        Player1.Hand.Enqueue(player1Card);
                        Player1.Hand.Enqueue(player2Card);
                    }
                    else
                    {
                        Player2.Hand.Enqueue(player2Card);
                        Player2.Hand.Enqueue(player1Card);
                    }
                }

                return Player1.Hand.Count == 0 ? Player2 : Player1;
            }
        }

        public object PartB(string input)
        {
            (Player player1, Player player2) = ParseInput(input);

            Player winner = new RecursiveCombat(player1, player2).Play();
            int handSize = winner.Hand.Count;
            return winner.Hand
                .Select((card, index) => (Card: card, Index: handSize - index))
                .Aggregate(0, (score, pair) => score + pair.Card * pair.Index);
        }
    }
}

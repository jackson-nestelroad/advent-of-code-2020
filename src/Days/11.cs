using AdventOfCode2020.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2020.Days
{
    [Solution(11)]
    class Day11 : ISolution
    {
        private enum SeatSystem {
            Floor = '.',
            Empty = 'L',
            Occupied = '#'
        }

        private static SeatSystem[,] ParseInput(string input)
        {
            string[] lines = input.Lines();
            int m = lines.Length;
            int n = lines[0].Length;
            SeatSystem[,] seats = new SeatSystem[m, n];
            for (int i = 0; i < m; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    seats[i, j] = (SeatSystem)lines[i][j];
                }
            }
            return seats;
        }

        private delegate SeatSystem? SeatRule(SeatSystem[,] seats, int i, int j);

        private (SeatSystem[,] NewState, bool Changed) TransformSeats(SeatSystem[,] seats, SeatRule emptySeatRule, SeatRule occupiedSeatRule)
        {
            int m = seats.GetLength(0);
            int n = seats.GetLength(1);

            bool changed = false;
            SeatSystem[,] nextState = new SeatSystem[m, n];
            for (int i = 0; i < m; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    nextState[i, j] = seats[i, j];
                    SeatSystem? update;
                    switch (seats[i, j])
                    {
                        case SeatSystem.Empty: 
                            update = emptySeatRule(seats, i, j);
                            if (update != null)
                            {
                                nextState[i, j] = (SeatSystem)update;
                                changed = true;
                            }
                            break;
                        case SeatSystem.Occupied:
                            update = occupiedSeatRule(seats, i, j);
                            if (update != null)
                            {
                                nextState[i, j] = (SeatSystem)update;
                                changed = true;
                            }
                            break;
                        default:  break;
                    }
                }
            }

            return (nextState, changed);
        }

        private int CountAdjacentOfType(SeatSystem[,] seats, int i, int j, SeatSystem type, int target = int.MaxValue)
        {
            int rowStart = Math.Max(0, i - 1);
            int rowStop = Math.Min(seats.GetLength(0), i + 2);
            int colStart = Math.Max(0, j - 1);
            int colStop = Math.Min(seats.GetLength(1), j + 2);
            int count = 0;
            for (int a = rowStart; a < rowStop; ++a)
            {
                for (int b = colStart; b < colStop; ++b)
                {
                    if (a != i || b != j)
                    {
                        // Return early if we met out target count already
                        if (seats[a, b] == type && ++count >= target)
                        {
                            return count;
                        }
                    }
                }
            }
            return count;
        }

        private int CountType(SeatSystem[,] seats, SeatSystem type)
        {
            int m = seats.GetLength(0);
            int n = seats.GetLength(1);

            int count = 0;
            for (int i = 0; i < m; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    if (seats[i, j] == type)
                    {
                        ++count;
                    }
                }
            }
            return count;
        }

        public object PartA(string input)
        {
            SeatSystem[,] seats = ParseInput(input);
            int m = seats.GetLength(0);
            int n = seats.GetLength(1);

            bool changed = true;
            do
            {
                (seats, changed) = TransformSeats(seats, (seats, i, j) =>
                {
                    return CountAdjacentOfType(seats, i, j, SeatSystem.Occupied, 0) == 0 ? SeatSystem.Occupied : null;
                }, (seats, i, j) =>
                {
                    return CountAdjacentOfType(seats, i, j, SeatSystem.Occupied, 4) >= 4 ? SeatSystem.Empty : null;
                });
            } while (changed);

            return CountType(seats, SeatSystem.Occupied);
        }

        private int CountFirstVisibleOccupied(SeatSystem[,] seats, int i, int j, int target = int.MaxValue)
        {
            int m = seats.GetLength(0);
            int n = seats.GetLength(1);

            int count = 0;
            for (int y = -1; y < 2; ++y)
            {
                for (int x = -1; x < 2; ++x)
                {
                    if (x != 0 || y != 0)
                    {
                        // Follow line with slope (x, y) until a seat is found
                        for (int a = i + y, b = j + x; a >= 0 && a < m && b >= 0 && b < n; a += y, b += x)
                        {
                            // Any seat blocks seats behind it
                            if (seats[a, b] != SeatSystem.Floor)
                            {
                                // Return early if we hit our target
                                if (seats[a, b] == SeatSystem.Occupied && ++count >= target)
                                {
                                    return count;
                                }
                                break;
                            }
                        }
                    }
                }
            }
            return count;
        }

        public object PartB(string input)
        {
            // Only a small modification of part A

            SeatSystem[,] seats = ParseInput(input);
            int m = seats.GetLength(0);
            int n = seats.GetLength(1);

            bool changed = true;
            do
            {
                (seats, changed) = TransformSeats(seats, (seats, i, j) =>
                {
                    return CountFirstVisibleOccupied(seats, i, j, 0) == 0 ? SeatSystem.Occupied : null;
                }, (seats, i, j) =>
                {
                    return CountFirstVisibleOccupied(seats, i, j, 5) >= 5 ? SeatSystem.Empty : null;
                });
            } while (changed);

            return CountType(seats, SeatSystem.Occupied);
        }
    }
}

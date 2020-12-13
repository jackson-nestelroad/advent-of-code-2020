using AdventOfCode2020.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2020.Days
{
    [Solution(12)]
    class Day12 : ISolution
    {
        private enum Direction
        {
            North = 'N',
            South = 'S',
            East = 'E',
            West = 'W',
            Left = 'L',
            Right = 'R',
            Forward = 'F'
        }

        private readonly struct Instruction
        {
            public Direction Direction { get; init; }
            public int Units { get; init; }
        }

        private class Ship
        {
            public Point2D Position { get; private set; }
            public Direction Facing { get; private set; }

            private static readonly Direction[] RotationArray =
            {
                Direction.North,
                Direction.East,
                Direction.South,
                Direction.West
            };

            public Ship()
            {
                Position = new Point2D(0, 0);
                Facing = Direction.East;
            }

            protected void Rotate(int degrees)
            {
                // Mathematical modulo, where negative numbers are not allowed (-1 % 4 == 3)
                int index = (Array.IndexOf(RotationArray, Facing) + (degrees / 90)) % RotationArray.Length;
                index = index < 0 ? index + RotationArray.Length : index;
                Facing = RotationArray[index];
            }

            public void Maneuver(Instruction instr)
            {
                switch (instr.Direction)
                {
                    case Direction.North: Position = Position.Translate(y: instr.Units); break;
                    case Direction.South: Position = Position.Translate(y: -instr.Units); break;
                    case Direction.East: Position = Position.Translate(x: instr.Units); break;
                    case Direction.West: Position = Position.Translate(x: -instr.Units); break;
                    case Direction.Left: Rotate(-instr.Units); break;
                    case Direction.Right: Rotate(instr.Units); break;
                    case Direction.Forward: Maneuver(new Instruction() { Direction = Facing, Units = instr.Units }); break;
                }
            }
        }

        private static Instruction[] ParseInput(string input)
        {
            return input.Lines().Select(line => new Instruction() { Direction = (Direction)line[0], Units = int.Parse(line[1..]) }).ToArray();
        }

        public object PartA(string input)
        {
            Instruction[] instructions = ParseInput(input);
            Ship ship = new Ship();
            foreach (Instruction instr in instructions)
            {
                ship.Maneuver(instr);
            }
            return Math.Abs(ship.Position.X) + Math.Abs(ship.Position.Y);
        }

        private class WaypointShip
        {
            public Point2D ShipPosition { get; private set; }
            public Point2D WaypointPosition { get; private set; }

            private static readonly Point2D[] UnitCirclePoints =
            {
                new Point2D(1, 0),
                new Point2D(0, 1),
                new Point2D(-1, 0),
                new Point2D(0, -1)
            };

            public WaypointShip()
            {
                ShipPosition = new Point2D(0, 0);
                WaypointPosition = new Point2D(10, 1);
            }

            private void RotateWaypoint(int degrees)
            {
                int rotations = (degrees / 90) % 4;
                rotations = rotations < 0 ? rotations + 4 : rotations;
                (int x, int y) = WaypointPosition;
                (int cos, int sin) = UnitCirclePoints[rotations];
                WaypointPosition = new Point2D(cos * x - sin * y, sin * x + cos * y);
            }

            public void Maneuver(Instruction instr)
            {
                switch (instr.Direction)
                {
                    case Direction.North: WaypointPosition = WaypointPosition.Translate(y: instr.Units); break;
                    case Direction.South: WaypointPosition = WaypointPosition.Translate(y: -instr.Units); break;
                    case Direction.East: WaypointPosition = WaypointPosition.Translate(x: instr.Units); break;
                    case Direction.West: WaypointPosition = WaypointPosition.Translate(x: -instr.Units); break;
                    case Direction.Left: RotateWaypoint(instr.Units); break;
                    case Direction.Right: RotateWaypoint(-instr.Units); break;
                    case Direction.Forward: ShipPosition += WaypointPosition * instr.Units; break;
                }
            }
        }

        public object PartB(string input)
        {
            Instruction[] instructions = ParseInput(input);
            WaypointShip ship = new WaypointShip();
            foreach (Instruction instr in instructions)
            {
                ship.Maneuver(instr);
            }
            return Math.Abs(ship.ShipPosition.X) + Math.Abs(ship.ShipPosition.Y);
        }
    }
}

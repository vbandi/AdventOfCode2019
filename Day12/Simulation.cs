using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Day12
{
    public class Simulation
    {
        public BodyState[] Bodies { get; }

        public Simulation(BodyState[] bodies)
        {
            Bodies = bodies;
        }
        public void ExecuteStep()
        {
            // update velocities
            foreach (BodyState body in Bodies)
            {
                body.Velocity.X += Bodies.Count(b => b.Position.X > body.Position.X);
                body.Velocity.X -= Bodies.Count(b => b.Position.X < body.Position.X);
                body.Velocity.Y += Bodies.Count(b => b.Position.Y > body.Position.Y);
                body.Velocity.Y -= Bodies.Count(b => b.Position.Y < body.Position.Y);
                body.Velocity.Z += Bodies.Count(b => b.Position.Z > body.Position.Z);
                body.Velocity.Z -= Bodies.Count(b => b.Position.Z < body.Position.Z);
            }

            foreach (BodyState body in Bodies)
            {
                body.Position.X += body.Velocity.X;
                body.Position.Y += body.Velocity.Y;
                body.Position.Z += body.Velocity.Z;
            }
        }

        public long CalculateTotalEnergy()
        {
            return Bodies.Sum(b => b.KineticEnergy * b.PotentialEnergy);
        }

        public long CalculateFirstRepeatOfHistory()
        {
            var comparer = new HistoryEqualityComparer();

            // The key is to realize all axes are independent. So, we can calculate the 
            // cycle length for all axes separately. 
            // Then take the lowest common multiplier of them, which is when they all return to the same state.

            var xCycleLength = GetCycleLength(b => (b.Position.X, b.Velocity.X));
            var yCycleLength = GetCycleLength(b => (b.Position.Y, b.Velocity.Y));
            var zCycleLength = GetCycleLength(b => (b.Position.Z, b.Velocity.Z));

            BigInteger gcd = BigInteger.GreatestCommonDivisor(BigInteger.GreatestCommonDivisor(xCycleLength, yCycleLength),
                zCycleLength);

            return (long) ((xCycleLength / gcd) * (yCycleLength / gcd) * (zCycleLength / gcd));

            long GetCycleLength(Func<BodyState, (long, long)> stateFunc)
            {
                HashSet<(long, long)[]> history = new HashSet<(long, long)[]>(comparer);
                (long, long)[] state = null;

                while (!history.Contains(state))
                {
                    history.Add(state);

                    ExecuteStep();

                    state = Bodies.Select(stateFunc).ToArray();
                }

                int firstOccurenceIndex = 0;

                while (!history.Contains(state))   // HashSet doesn't have an "indexof", so...
                    firstOccurenceIndex++; 

                var result = history.Count - firstOccurenceIndex - 1;
                return result;

            }
        }
    }

    public class HistoryEqualityComparer : IEqualityComparer<(long, long)[]>
    {
        public bool Equals((long, long)[] x, (long, long)[] y)
        {
            if (x == null && y == null)
                return true;

            if (x == null || y == null)
                return false;

            if (x.Length != y.Length)
                return false;

            for (var i = 0; i < x.Length; i++)
                if (x[i] != y[i])
                    return false;

            return true;
        }

        public int GetHashCode((long, long)[] obj)
        {
            long result = 0;

            for (int i = 0; i < obj.Length; i++)
                result += obj[i].Item1 * obj[i].Item2;

            return (int) result;
        }
    }

    public class BodyState
    {
        public (long X, long Y, long Z) Position;
        public (long X, long Y, long Z) Velocity;

        public BodyState(long x, long y, long z)
        {
            Position.X = x;
            Position.Y = y;
            Position.Z = z;
        }

        public long PotentialEnergy => Math.Abs(Position.X) + Math.Abs(Position.Y) + Math.Abs(Position.Z);
        public long KineticEnergy => Math.Abs(Velocity.X) + Math.Abs(Velocity.Y) + Math.Abs(Velocity.Z);
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

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
            return Bodies.Sum(b => b.KinecticEnergy * b.PotentialEnergy);
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
        public long KinecticEnergy => Math.Abs(Velocity.X) + Math.Abs(Velocity.Y) + Math.Abs(Velocity.Z);
    }

}

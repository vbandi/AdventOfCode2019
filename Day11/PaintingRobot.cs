using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Day11
{
    public class PaintingRobot
    {
        public List<(int X, int Y)> WhitePlates { get; }

        public List<(int X, int Y)> PaintedPlates { get; } = new List<(int X, int Y)>();

        public IntCodeComputer Computer { get; }
        
        public (int X, int Y) Coordinates = (0, 0);

        private List<(int X, int Y)> ClockwiseDirections = new List<(int, int)>
        {
            (0, -1),
            (1, 0),
            (0, 1),
            (-1, 0)
        };

        public (int X, int Y) Direction { get; private set; }
        
        public PaintingRobot(List<(int X, int Y)> whitePlates)
        {
            Computer = new IntCodeComputer();
            WhitePlates = whitePlates;
            Direction = (0, -1); //initial direction is up
         
            SetLightSensor();

            Computer.OutputHappened += HandleOutput;

        }

        private bool LastOutputWasPaint = false;
        
        private void HandleOutput(object sender, long output)
        {
            if (LastOutputWasPaint)
                HandleTurnInstruction(output);
            else
                HandlePaintInstruction(output);

            LastOutputWasPaint = !LastOutputWasPaint;
        }

        private void HandleTurnInstruction(long turnInstruction)
        {
            switch (turnInstruction)
            {
                case 1:
                    TurnRight();
                    MoveForward();
                    break;
                case 0:
                    TurnLeft();
                    MoveForward();
                    break;
            }
        }

        private void HandlePaintInstruction(long paintInstruction)
        {
            if (!PaintedPlates.Contains(Coordinates))
                PaintedPlates.Add(Coordinates);

            switch (paintInstruction)
            {
                case 1 when !WhitePlates.Contains(Coordinates):
                    WhitePlates.Add(Coordinates);
                    break;
                case 0 when WhitePlates.Contains(Coordinates):
                    WhitePlates.Remove(Coordinates);
                    break;
            }
        }

        private void SetLightSensor()
        {
            Computer.OverrideInputBuffer(WhitePlates.Contains(Coordinates) ? 1 : 0);
        }

        public void TurnLeft()
        {
            var dirIndex = ClockwiseDirections.IndexOf(Direction);

            if (dirIndex == 0)
                dirIndex = ClockwiseDirections.Count;

            dirIndex--;

            Direction = ClockwiseDirections[dirIndex];
        }


        public void TurnRight()
        {
            var dirIndex = ClockwiseDirections.IndexOf(Direction);

            if (dirIndex == ClockwiseDirections.Count - 1)
                dirIndex = -1;

            dirIndex++;

            Direction = ClockwiseDirections[dirIndex];
        }

        public void MoveForward()
        {
            Coordinates.X += Direction.X;
            Coordinates.Y += Direction.Y;
            SetLightSensor();
        }


    }
}

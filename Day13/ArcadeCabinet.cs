using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Day13
{
    public class ArcadeCabinet
    {
        public Dictionary<(int X, int Y), Tile> Screen { get; } = new Dictionary<(int X, int Y), Tile>();
        public long Score { get; private set; }
        private IntCodeComputer Computer;

        public async Task ExecuteProgram(string program)
        {
            Computer = new IntCodeComputer();
            Computer.OutputHappened += HandleOutput;
            Computer.InputRequested += (sender, block) => JoystickInputRequested?.Invoke(this, null);
            await Computer.ExecuteAsync(program);
        }

        public void TapJoystick(long input)
        {
            Computer.input.Post(input);
        }

        private int outputCount = 0;
        private int x;
        private int y;

        private void HandleOutput(object sender, long e)
        {
            switch (outputCount)
            {
                case 0:
                    x = (int)e;
                    break;
                case 1:
                    y = (int)e;
                    break;
                case 2 when x > -1:
                    Tile tile = (Tile)e;
                    Screen[(x, y)] = tile;
                    ScreenChanged?.Invoke(this, (x, y, tile));
                    break;
                case 2 when x == -1 && y == 0:
                    Score = e;
                    ScoreChanged?.Invoke(this, e);
                    break;
                default:
                    throw new InvalidOperationException();
            }

            outputCount = (outputCount + 1) % 3;
        }

        public event EventHandler<(int x, int y, Tile tile)> ScreenChanged;
        public event EventHandler<long> ScoreChanged;
        public event EventHandler JoystickInputRequested;

    }





    public enum Tile
    {
        Empty = 0,
        Wall = 1,
        Block = 2,
        Paddle = 3,
        Ball = 4
    }
}

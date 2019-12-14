using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Day13
{
    class Program
    {
        static void Main(string[] args)
        {
            //Part1();
            Part2();
        }

        private static async Task Part2()
        {
            Console.CursorVisible = false;
            Console.ReadKey();

            var program = File.ReadAllText("input.txt");
            program = "2" + program.Substring(1);

            ArcadeCabinet arcade = new ArcadeCabinet();

            arcade.ScoreChanged += (sender, l) =>
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine($"Score: {arcade.Score}      ");
            };

            arcade.ScreenChanged += (sender, screenDelta) => UpdateScreen(screenDelta);
            arcade.JoystickInputRequested += (sender, args) =>
                arcade.TapJoystick(Math.Sign(BallX - PaddleX));

            await arcade.ExecuteProgram(program);

            Console.ReadKey();
        }

        private static long BallX = 0;
        private static long PaddleX = 0;

        private static void UpdateScreen((int x, int y, Tile tile) screenDelta)
        {
            Console.SetCursorPosition(screenDelta.x, screenDelta.y + 2);

            switch (screenDelta.tile)
            {
                case Tile.Empty:
                    Console.Write(" ");
                    break;
                case Tile.Wall:
                    Console.Write("#");
                    break;
                case Tile.Block:
                    Console.Write("X");
                    break;
                case Tile.Paddle:
                    PaddleX = screenDelta.x;
                    Console.Write("=");
                    break;
                case Tile.Ball:
                    BallX = screenDelta.x;
                    Console.Write("o");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static async Task Part1()
        {

            var program = File.ReadAllText("input.txt");
            var arcade = new ArcadeCabinet();
            await arcade.ExecuteProgram(program);

            Console.WriteLine($"Total number of block tiles: {arcade.Screen.Values.Count(t => t == Tile.Block)}");
        }
    }
}

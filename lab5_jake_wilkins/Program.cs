using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GDIDrawer;
using Utility_Library;

namespace Lab5_jake_wilkins
{
    class Program
    {
        public const int winWidth = 800;
        public const int winHeight = 600;
        public static CDrawer gdi = new CDrawer(winWidth, winHeight, false);
        public const int ballSize = 1;
        public const int rowCount = winHeight / ballSize;
        public const int columnCount = winWidth / ballSize;
        public static Balls[,] game = new Balls[rowCount, columnCount];

        public enum BallState { Alive, Dead };
        public enum Mode { Easy, Medium, Hard };

        public struct Balls
        {
            public Color _ballColour;
            public BallState _ballsReality;

            public Balls(Color shade, BallState live)
            {
                _ballColour = shade;
                _ballsReality = live;
            }

        }

        static void Main(string[] args)
        {
            //variables
            gdi.Scale = 50;
            Mode state = Mode.Easy;
            bool running = true;
            bool playing = false;
            bool difficulty = false;
            Point choice;

            do
            {
                gdi.GetLastMouseLeftClickScaled(out choice);
                gdi.BBColour = Color.Black;
                gdi.AddText("Ballz", 30, Color.WhiteSmoke);
                gdi.AddRectangle(3, 9, 3, 2, Color.Empty, 1, Color.WhiteSmoke);
                gdi.AddText("Play", 20, 3, 9, 3, 2, Color.WhiteSmoke);
                gdi.AddRectangle(10, 9, 3, 2, Color.Empty, 1, Color.WhiteSmoke);
                gdi.AddText("Quit", 20, 10, 9, 3, 2, Color.WhiteSmoke);

                do
                {
                    if (choice.X >= 3 && choice.X <= 5 && choice.Y >= 9 && choice.Y <= 10)
                    {
                        do
                        {
                            gdi.GetLastMouseLeftClickScaled(out choice);
                            gdi.Clear();
                            gdi.AddText("Difficulty Select", 30, 4, 1, 7, 2, Color.WhiteSmoke);
                            gdi.AddRectangle(1, 4, 4, 2, Color.Empty, 1, Color.WhiteSmoke);
                            gdi.AddText("Easy", 20, 1, 4, 4, 2, Color.WhiteSmoke);

                            gdi.AddRectangle(6, 4, 4, 2, Color.Empty, 1, Color.WhiteSmoke);
                            gdi.AddText("Medium", 20, 6, 4, 4, 2, Color.WhiteSmoke);

                            gdi.AddRectangle(11, 4, 4, 2, Color.Empty, 1, Color.WhiteSmoke);
                            gdi.AddText("Hard", 20, 11, 4, 4, 2, Color.WhiteSmoke);

                            if (choice.X >= 1 && choice.X <= 4 && choice.Y >= 4 && choice.Y <= 5)
                            {
                                state = Mode.Easy;
                                gdi.Clear();
                                Randomize(state);
                            }
                            else if (choice.X >= 6 && choice.X <= 9 && choice.Y >= 4 && choice.Y <= 5)
                            {
                                state = Mode.Medium;
                                gdi.Clear();
                                Randomize(state);
                            }
                            else if (choice.X >= 11 && choice.X <= 14 && choice.Y >= 4 && choice.Y <= 5)
                            {
                                state = Mode.Hard;
                                gdi.Clear();
                                Randomize(state);
                            }
                            else
                            {
                                difficulty = true;
                            }

                            System.Threading.Thread.Sleep(100);
                            gdi.Render();
                            gdi.Clear();

                        } while (difficulty);
                    }
                    else if (choice.X >= 10 && choice.X <= 12 && choice.Y >= 9 && choice.Y <= 10)
                    {
                        running = false;
                    }
                } while (playing);

                System.Threading.Thread.Sleep(100);
                gdi.Render();
                gdi.Clear();

            } while (running);
        }
        public static void Randomize(Mode state)
        {
            Color[] shader = new Color[] { Color.AntiqueWhite, Color.HotPink, Color.Orchid, Color.Aqua, Color.LawnGreen };
            Random rng = new Random();
            int column = 0;
            int row = 0;

            if (state == Mode.Easy)
            {
                for (row = 0; row < game.GetLength(0); row++)
                    for (column = 0; column < game.GetLength(1); column++)
                    {
                        game[row, column] = new Balls(shader[rng.Next(3)], BallState.Alive);
                    }
            }
            else if (state == Mode.Medium)
            {
                for (row = 0; row < game.GetLength(0); row++)
                    for (column = 0; column < game.GetLength(1); column++)
                    {
                        game[row, column] = new Balls(shader[rng.Next(4)], BallState.Alive);
                    }
            }
            else if (state == Mode.Hard)
            {
                for (row = 0; row < game.GetLength(0); row++)
                    for (column = 0; column < game.GetLength(1); column++)
                    {
                        game[row, column] = new Balls(shader[rng.Next(5)], BallState.Alive);
                    }
            }
        }
        public static void Display()
        {
            gdi.Clear();
            int row = 0;
            int column = 0;

            for (row = 0; row < game.GetLength(0); row++)
                for (column = 0; column < game.GetLength(1); column++)
                {
                    if (game[row,column]._ballsReality == BallState.Alive)
                    {
                        gdi.AddEllipse(column, row, ballSize, ballSize, game[row, column]._ballColour, 0, game[row, column]._ballColour);
                    }
                }

        }
        public static void Pick()
        {

        }
        public static void CheckBalls()
        {

        }
        public static void StepDown()
        {

        }
        public static void FallDown()
        {

        }
        public static int BallsAlive()
        {
            int row = 0;
            int column = 0;
            int counter = 0;

            for (row = 0; row < game.GetLength(0); row++)
                for (column = 0; column < game.GetLength(1); column++)
                {
                    if (game[row, column]._ballsReality == BallState.Alive)
                        counter++;
                }
            return counter;
        }
    }
}
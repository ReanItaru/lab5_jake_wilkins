//*******************************************************************************************************************
//Program: Ballz
//Description:  Creates a bunch of coloured balls on screen, user clicks on 1 at a time to elimiate it and all
//              surrounding balls of the same colour. Provides score at end based on number and size of combos
//Lab:          5
//Date:         December 14/2016
//Author:       Jake Wilkins
//Class:        A01
//Instructor:   JD Silver
//******************************************************************************************************************

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
        public const int winWidth = 800;                                        //the width of the GDIDrawer window
        public const int winHeight = 600;                                       //the height of the GDIDrawer window
        public const int ballSize = 1;                                          //the size of each ball, adjusted for scale
        const int rowCount = winHeight / ballSize / 50;                  //number of rows that the balls will be in, adjusted for scale
        const int columnCount = winWidth / ballSize / 50;                //number of columns the balls will be in, adjusted for scale
        public static CDrawer gdi = new CDrawer(winWidth, winHeight, false);    //the window in which the game will actually be played
        public static Balls[,] game = new Balls[rowCount, columnCount];         //the multi-dimensional array that represents each ball 

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
            gdi.Scale = 50;                     //window scaled by 50 to simplify dimensions 
            gdi.RedundaMouse = true;            //allows user to select the same spot twice when picking balls
            Mode state = Mode.Easy;             //the option selected by user on how difficult the game will be
            bool running = true;                //entire program is in this loop, clicking quit is only way to exit this loop
            bool difficulty = false;            //difficulty choice loop, is entered if play option selected
            bool gaming = false;                //loop used to prevent game from starting till a difficulty is selected
            int score = 0;                      //final score to be displayed on screen at end of game
            Point choice;                       //where the user clicks

            //title screen stuff, outside of loop to avoid interference at end of game with Game Over screen
            gdi.AddText("Ballz", 30, Color.WhiteSmoke);
            gdi.AddRectangle(3, 9, 3, 2, Color.Empty, 1, Color.WhiteSmoke);
            gdi.AddText("Play", 20, 3, 9, 3, 2, Color.WhiteSmoke);
            gdi.AddRectangle(10, 9, 3, 2, Color.Empty, 1, Color.WhiteSmoke);
            gdi.AddText("Quit", 20, 10, 9, 3, 2, Color.WhiteSmoke);

            do
            {
                gdi.GetLastMouseLeftClickScaled(out choice);

                //this is where the 'play button' is located
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

                        //each option corresponds with where each difficulty button is
                        if (choice.X >= 1 && choice.X <= 4 && choice.Y >= 4 && choice.Y <= 5)
                        {
                            state = Mode.Easy;
                            gaming = true;
                            gdi.Clear();
                        }
                        else if (choice.X >= 6 && choice.X <= 9 && choice.Y >= 4 && choice.Y <= 5)
                        {
                            state = Mode.Medium;
                            gaming = true;
                            gdi.Clear();
                        }
                        else if (choice.X >= 11 && choice.X <= 14 && choice.Y >= 4 && choice.Y <= 5)
                        {
                            state = Mode.Hard;
                            gaming = true;
                            gdi.Clear();
                        }
                        else
                            difficulty = false;

                        //this is where the game actually starts 
                        Randomize(state);
                        gdi.Render();
                        System.Threading.Thread.Sleep(100);
                        gdi.Clear();
                        while (gaming)
                        {
                            score = TimingLoop();

                            gaming = false;
                            difficulty = true;

                            gdi.AddText("Game Over\nScore:" + score, 40, Color.WhiteSmoke);
                            gdi.AddRectangle(3, 9, 3, 2, Color.Empty, 1, Color.WhiteSmoke);
                            gdi.AddText("Play Again", 20, 3, 9, 3, 2, Color.WhiteSmoke);
                            gdi.AddRectangle(10, 9, 3, 2, Color.Empty, 1, Color.WhiteSmoke);
                            gdi.AddText("Quit", 20, 10, 9, 3, 2, Color.WhiteSmoke);
                        }
                    } while (!difficulty);
                }

                //this is where the 'quit button' is located
                else if (choice.X >= 10 && choice.X <= 12 && choice.Y >= 9 && choice.Y <= 10)
                    running = false;

                gdi.Render();
                System.Threading.Thread.Sleep(100);

            } while (running);
        }
        //loops through all other methods directly or otherwise until all balls are cleared from screen then exits back to main
        public static int TimingLoop()
        {
            bool running = true;        //used to loop till all balls are eliminated
            int score = 0;              //the final score to return to main program
            Point click;                //where the user clicks

            do
            {
                if (BallsAlive() == 0)
                    running = false;

                else
                {
                    Display();

                    //prevents entering the Pick method unless a new left click has been registered
                    if (gdi.GetLastMouseLeftClickScaled(out click))
                        score += Pick(click);
                }

                gdi.Render();
                System.Threading.Thread.Sleep(100);
                gdi.Clear();

            } while (running);

            return score;
        }
        //iterates through the entire multi-dimensional array of balls at beginning of game to assign random colour based on difficulty selected
        public static void Randomize(Mode state)
        {
            Color[] shader = new Color[] { Color.LawnGreen, Color.AntiqueWhite, Color.Aqua, Color.Red, Color.Purple };
                                            //the colours to be randomly selected
            Random rng = new Random();      //needed to randomly select which colour to assign to each ball
            int column = 0;                 //iterates through each 'column' of array
            int row = 0;                    //iterates through each 'row' of array

            if (state == Mode.Easy)
            {
                for (row = 0; row < game.GetLength(0); row++)
                    for (column = 0; column < game.GetLength(1); column++)
                        game[row, column] = new Balls(shader[rng.Next(3)], BallState.Alive);

            }
            else if (state == Mode.Medium)
            {
                for (row = 0; row < game.GetLength(0); row++)
                    for (column = 0; column < game.GetLength(1); column++)
                        game[row, column] = new Balls(shader[rng.Next(4)], BallState.Alive);

            }
            else if (state == Mode.Hard)
            {
                for (row = 0; row < game.GetLength(0); row++)
                    for (column = 0; column < game.GetLength(1); column++)
                        game[row, column] = new Balls(shader[rng.Next(5)], BallState.Alive);

            }
        }

        //removes old balls then draws any living balls to the screen at the approprate locations
        public static void Display()
        {
            int column = 0;                 //iterates through each 'column' of array
            int row = 0;                    //iterates through each 'row' of array

            gdi.Clear();
            for (row = 0; row < game.GetLength(0); row++)
                for (column = 0; column < game.GetLength(1); column++)
                    if (game[row, column]._ballsReality == BallState.Alive)
                        gdi.AddEllipse(column, row, ballSize, ballSize, game[row, column]._ballColour, 0, Color.Empty);

        }
        //based on the cursor location passed down, eliminates all appropraite balls then assigns a fair, combo based score
        public static int Pick(Point chioce)
        {
            int score = 0;              //score that will get passed up chain of command to main
            int row = chioce.Y;         //row location of ball user selected for elimination
            int column = chioce.X;      //column location of ball user selected for elimination

            if (game[row, column]._ballsReality == BallState.Dead)
            {
                Console.Beep();
            }
            else
            {
                score = CheckBalls(row, column, game[row, column]._ballColour);
                if (score > 1 && score < 5)
                {
                    score *= 50;
                }
                else if (score > 4 && score < 10)
                {
                    score *= 150;
                }
                else if (score > 9)
                {
                    score *= 300;
                }
                else
                    score *= 5;
                StepDown();
            }
            return score;
        }
        //recursivly used to check all connecting balls for similar colour then passes number of balls that were eliminated up chain
        public static int CheckBalls(int row, int column, Color shade)
        {
            int killCount = 0;      //number of balls that are converted from Alive to Dead

            //checks to make sure row is within game screen, is still alive and the right colour then recursivly calls itself till no criteria is met
            if (row < 12 && row > -1 && column < 16 && column > -1)
            {
                if (game[row, column]._ballsReality == BallState.Alive)
                {
                    if (game[row, column]._ballColour == shade)
                    {
                        game[row, column]._ballsReality = BallState.Dead;
                        killCount++;
                        return CheckBalls(row + 1, column, shade) + CheckBalls(row - 1, column, shade) + CheckBalls(row, column + 1, shade) + CheckBalls(row, column - 1, shade) + killCount;
                    }
                }
            }

            return killCount;
        }
        //moves the balls the have nothing under them down over top of the dead balls, recursive to ensure balls moved all the way down
        public static int StepDown()
        {
            int row = 0;
            int column = 0;
            int ballsDropped = 0;

            for (row = game.GetLength(0) - 1; row > 0; row--)
                for (column = game.GetLength(1) - 1; column > -1; column--)
                {
                    if (game[row, column]._ballsReality == BallState.Dead)
                    {
                        if (game[(row - 1), column]._ballsReality == BallState.Alive)
                        {
                            game.SetValue(game[row - 1, column], row, column);
                            game[row - 1, column]._ballsReality = BallState.Dead;
                            ballsDropped++;
                            return StepDown() + ballsDropped;
                        }
                    }
                }

            return ballsDropped;
        }
        //used to see if there are any remaining balls on screen
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
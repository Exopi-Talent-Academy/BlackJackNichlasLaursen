using System;
using System.Reflection.Metadata.Ecma335;
using Cards;
using BlackjackGameLoop;
using Dealing;
using Bets;

namespace BlackjackProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Blackjack by Bender!");

            // Create a new game instance
            BlackjackGame game = new();

            // Start the game loop
            while (true)
            {
                game.PlayRound();

                Console.WriteLine("Do you want to play another round? (y/n)");
                string input = Console.ReadLine() ?? "";
                if (!input.Equals("y", StringComparison.CurrentCultureIgnoreCase))
                {
                    break;
                }
            }

            Console.WriteLine("Thanks for playing!");
        }
    }
}
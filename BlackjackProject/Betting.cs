using System;
using Cards;
using BlackjackGameLoop;
using Dealing;
using BlackjackProject;

namespace Bets
{
    public class Bet
    {
        //Initial number of chips that the player can bet with
        public int InitialChips { get; set; } = 1000;

        // Current number of chips the player has
        public int CurrentChips { get; set; } = 1000;

        // Amount the player bets for the current round
        public int BetAmount { get; set; }

        internal readonly float blackjackPayoutMultiplier = 1.5f;

        internal readonly float surrenderLossMultiplier = 0.5f;

        // Method to place bet for the current round
        public void PlaceBet()
        {
            Console.WriteLine($"You have {CurrentChips} chips.");
            Console.WriteLine("Enter your bet amount for this round:");
            while (true)
            {
                string input = Console.ReadLine() ?? "";
                if (int.TryParse(input, out int bet) && bet > 0 && bet <= CurrentChips)
                {
                    BetAmount = bet;
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid bet amount. Be realistic mate.");
                }
            }
        }

        public void BlackjackWin()
        {
            CurrentChips += (int)(BetAmount * blackjackPayoutMultiplier);
            Console.WriteLine($"You win {BetAmount * blackjackPayoutMultiplier} chips.");
        }

        public void Win()
        {
            CurrentChips += BetAmount;
            Console.WriteLine($"You win {BetAmount} chips.");
        }

        public void Lose()
        {
            CurrentChips -= BetAmount;
            Console.WriteLine($"You lose {BetAmount} chips.");
        }

        public static void Push()
        {
            Console.WriteLine("It's a push! Your bet is returned.");
        }

        public void Surrender()
        {
            CurrentChips -= (int)(BetAmount * surrenderLossMultiplier);
            Console.WriteLine($"You lose {BetAmount * surrenderLossMultiplier} chips.");
        }
    }
}
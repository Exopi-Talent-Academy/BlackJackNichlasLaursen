using System;
using BlackjackGameLoop;
using BlackjackProject;
using Dealing;
using Bets;

namespace Cards
{
    public class Card
    {
        public string Suit { get; set; }
        public string Rank { get; set; }

        // List of possible suits and ranks
        public static readonly string[] Suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
        public static readonly string[] Ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King", "Ace" };

        //Translate the card's rank to its Blackjack value
        public int GetValue()
        {
            if (int.TryParse(Rank, out int numericValue))
            {
                return numericValue;
            }
            else if (Rank == "Jack" || Rank == "Queen" || Rank == "King")
            {
                return 10;
            }
            else if (Rank == "Ace")
            {
                return 11; // Ace can be 1 or 11, handled in game logic
            }
            return 0; // Default case, should not happen
        }

        public Card(string suit, string rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public override string ToString()
        {
            return $"{Rank} of {Suit}";
        }

        private static string GetRandomSuit(Random rand)
        {
            return Suits[rand.Next(Suits.Length)];
        }

        private static string GetRandomRank(Random rand)
        {
            return Ranks[rand.Next(Ranks.Length)];
        }
    }
}
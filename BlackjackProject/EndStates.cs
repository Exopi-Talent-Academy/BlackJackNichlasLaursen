using System;
using Cards;
using BlackjackProject;
using Dealing;
using Bets;
using BlackjackGameLoop;


namespace EndStates
{
    public class EndState()
    {


       public required Dealer dealer;

        static readonly int blackjackLimit = 21;

        public static void EndStateNoSplit(int playerValue, int dealerValue, Bet Bet)
        {
            if (Bet == null)
                throw new InvalidOperationException("Bet instance is required to end the state.");

            switch (true)
            {
                //Player busts if over 21
                case true when playerValue > blackjackLimit:
                    Console.WriteLine("Player busts! Dealer wins.");

                    //deduct bet from player's chips
                    Bet.Lose();
                    Console.WriteLine($"Player's chips: {Bet.CurrentChips}");
                    break;
                //Player hits blackjack if exactly 21
                case true when playerValue == blackjackLimit:
                    Console.WriteLine("Player hits Blackjack! Player wins.");

                    // Pay out 1.5 times the bet
                    Bet.BlackjackWin();
                    Console.WriteLine($"Player's chips: {Bet.CurrentChips}");
                    break;
                //Dealer busts if over 21
                case true when dealerValue > blackjackLimit:
                    Console.WriteLine("Dealer busts! Player wins.");

                    // Pay out bet to player
                    Bet.Win();
                    Console.WriteLine($"Player's chips: {Bet.CurrentChips}");
                    break;
                //Dealer hits blackjack if exactly 21
                case true when dealerValue == blackjackLimit:
                    Console.WriteLine("Dealer hits Blackjack! Dealer wins.");

                    //deduct bet from player's chips
                    Bet.Lose();
                    Console.WriteLine($"Player's chips: {Bet.CurrentChips}");
                    break;
                //Player has a higher hand
                case true when playerValue > dealerValue:
                    Console.WriteLine("Player has a higher hand! Player wins!");

                    // Pay out bet to player
                    Bet.Win();
                    Console.WriteLine($"Player's chips: {Bet.CurrentChips}");
                    break;
                //Dealer has a higher hand
                case true when dealerValue > playerValue:
                    Console.WriteLine("Dealer has a higher hand! Dealer wins");

                    //deduct bet from player's chips
                    Bet.Lose();
                    Console.WriteLine($"Player's chips: {Bet.CurrentChips}");
                    break;
                //In case of a tie, dealer wins
                case true when dealerValue == playerValue:
                    Console.WriteLine("It's a tie, Dealer wins!");

                    //deduct bet from player's chips
                    Bet.Lose();
                    Console.WriteLine($"Player's chips: {Bet.CurrentChips}");
                    break;
            }
        }

        public static void EndStateSplit(int playerValue1, int playerValue2, int dealerValue, Bet bet)
        {
            if (bet == null)
                throw new InvalidOperationException("Bet instance is required to end the state.");

            switch (true)
            {
                //Players first hand busts if over 21
                case true when playerValue1 > blackjackLimit:
                    Console.WriteLine("Player's first hand busts! Dealer wins.");
                    bet.Lose();
                    Console.WriteLine($"Player's chips: {bet.CurrentChips}");
                    break;
                //Players second hand busts if over 21
                case true when playerValue2 > blackjackLimit:
                    Console.WriteLine("Player's second hand busts! Dealer wins.");
                    bet.Lose();
                    Console.WriteLine($"Player's chips: {bet.CurrentChips}");
                    break;
                //Player's first hand hits blackjack if exactly 21
                case true when playerValue1 == blackjackLimit:
                    Console.WriteLine("Player's first hand hits Blackjack! Player wins.");
                    bet.BlackjackWin();
                    Console.WriteLine($"Player's chips: {bet.CurrentChips}");
                    break;
                //Player's second hand hits blackjack if exactly 21
                case true when playerValue2 == blackjackLimit:
                    Console.WriteLine("Player's second hand hits Blackjack! Player wins.");
                    bet.BlackjackWin();
                    Console.WriteLine($"Player's chips: {bet.CurrentChips}");
                    break;
                //Dealer busts if over 21
                case true when dealerValue > blackjackLimit:
                    Console.WriteLine("Dealer busts! Player wins.");
                    bet.Win();
                    Console.WriteLine($"Player's chips: {bet.CurrentChips}");
                    break;
                //Dealer hits blackjack if exactly 21
                case true when dealerValue == blackjackLimit:
                    Console.WriteLine("Dealer hits Blackjack! Dealer wins.");
                    bet.Lose();
                    Console.WriteLine($"Player's chips: {bet.CurrentChips}");
                    break;
                //Player's first hand has a higher hand than the dealer
                case true when playerValue1 > dealerValue:
                    Console.WriteLine("Player's first hand has a higher hand! Player wins!");
                    bet.Win();
                    Console.WriteLine($"Player's chips: {bet.CurrentChips}");
                    break;
                //Player's second hand has a higher hand than the dealer
                case true when playerValue2 > dealerValue:
                    Console.WriteLine("Player's second hand has a higher hand! Player wins!");
                    bet.Win();
                    Console.WriteLine($"Player's chips: {bet.CurrentChips}");
                    break;
                //Dealer has a higher hand than both player hands
                case true when dealerValue > playerValue1 && dealerValue > playerValue2:
                    Console.WriteLine("Dealer has a higher hand than both player hands! Dealer wins");
                    bet.Lose();
                    Console.WriteLine($"Player's chips: {bet.CurrentChips}");
                    break;
                //In case of a tie, dealer wins
                case true when dealerValue == playerValue1 && dealerValue == playerValue2:
                    Console.WriteLine("Both player hands tie with the dealer, Dealer wins!");
                    bet.Lose();
                    Console.WriteLine($"Player's chips: {bet.CurrentChips}"); break;
            }
        }
    }
}
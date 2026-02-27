using System;
using Cards;
using BlackjackGameLoop;
using BlackjackProject;
using System.ComponentModel.DataAnnotations;
using Bets;

namespace Dealing
{
    public class Dealer
    {

        //List of cards in hand for player and dealer
        public List<Card> playerHand = [];
        public List<Card> dealerHand = [];

        //List instance for if player splits
        public List<Card> playerHand1 = [];
        public List<Card> playerHand2 = [];

        public bool playerHasSplit = false;

        //List of dealt cards to ensure uniqueness
        readonly List<Card> dealtCards = [];

        //Reference to game instance
        readonly BlackjackGame game;

        //Reference to bet instance
        readonly Bet bet;

        // Card generator (injectable for tests)
        readonly Cards.ICardGenerator cardGenerator;

        public Dealer(BlackjackGame game, Bet bet, Cards.ICardGenerator? generator = null)
        {
            this.game = game;
            this.bet = bet;
            this.cardGenerator = generator ?? new Cards.CardGenerator();
            blackjackLimit = 21;
        }

        readonly int dealerThreshold = 17; // Dealer must hit until reaching this hand value
        internal readonly int blackjackLimit = 21;

        public void DealCardsForStart()
        {

            Console.WriteLine("Dealing cards...");

            //Randomly generate cards for player and dealer, append to a list to ensure uniqueness
            Random rand = new();

            //Deal two cards to the player face-up
            Card playerCard1 = cardGenerator.GenerateUniqueCard(rand, dealtCards);
            dealtCards.Add(playerCard1);
            playerHand.Add(playerCard1);

            Card playerCard2 = cardGenerator.GenerateUniqueCard(rand, dealtCards);
            dealtCards.Add(playerCard2);
            playerHand.Add(playerCard2);

            //Deal two cards to the dealer, one face-up and one face-down
            Card dealerCard1 = cardGenerator.GenerateUniqueCard(rand, dealtCards);
            dealtCards.Add(dealerCard1);
            dealerHand.Add(dealerCard1);

            Card dealerCard2 = cardGenerator.GenerateUniqueCard(rand, dealtCards);
            dealtCards.Add(dealerCard2);
            dealerHand.Add(dealerCard2);

            Console.WriteLine($"Player's cards: {playerCard1}, {playerCard2}");
            Console.WriteLine($"Player's hand value: {game.GetPlayerValue(playerHand)}");

            Console.WriteLine($"Dealer's card: {dealerCard1} and one face-down card");
        }

        public void DealCards()
        {
            //Logic for dealing and calculating hand values after initial deal
            while (true)
            {
                Console.WriteLine("Do you want to hit (h), stand (s), split (p), double down (dd), or surrender (sr)?");

                int playerValue = game.GetPlayerValue(playerHand);
                int dealerValue = game.GetDealerValue(dealerHand);

                string input = (Console.ReadLine() ?? "").ToLower();

                switch (input)
                {
                    case "h":
                        (playerValue, dealerValue) = HandleHit(playerValue, dealerValue);

                        // If someone busted or hit 21, stop the loop 
                        if (playerValue >= blackjackLimit || dealerValue >= blackjackLimit)
                        {
                            return;
                        } 
                        else
                        {
                            break; // Ask player for input again
                        }
                    case "s":
                        dealerValue = HandleStand(dealerValue);
                        return;
                    case "p":
                        if (playerHand.Count == 2 && playerHand[0].Rank == playerHand[1].Rank)
                        {
                            dealerValue = HandleSplit(dealerValue);
                            return;
                        }
                        else
                        {
                            Console.WriteLine("You cannot split. Your first two cards must be of the same rank.");
                            break;
                        }
                    case "dd":
                    // Check if player has enough chips to double the bet
                        if (bet.CurrentChips >= bet.BetAmount * 2)
                        {
                        (playerValue, dealerValue) = HandleDoubleDown(playerValue, dealerValue);
                        return;
                        }
                        else
                        {
                            Console.WriteLine("You do not have enough chips to double down.");
                            break;
                        }
                    case "sr":
                        HandleSurrender();
                        return;
                    default:
                        Console.WriteLine("Invalid input. Please enter 'h' to hit, 's' to stand, 'p' to split, 'dd' to double down, or 'sr' to surrender.");
                        break;
                }
            }
        }

        private (int playerValue, int dealerValue) HandleHit(int playerValue, int dealerValue)
        {
            Console.WriteLine("You chose to hit.");

            Card newPlayerCard = cardGenerator.GenerateUniqueCard(new Random(), dealtCards);
            playerHand.Add(newPlayerCard);
            Console.WriteLine($"You received: {newPlayerCard}");

            playerValue = game.GetPlayerValue(playerHand);
            Console.WriteLine($"New player hand value: {playerValue}");

            if (game.GetDealerValue(dealerHand) < dealerThreshold)
            {
                Card newDealerCard = cardGenerator.GenerateUniqueCard(new Random(), dealtCards);
                dealtCards.Add(newDealerCard);
                dealerHand.Add(newDealerCard);
                Console.WriteLine($"Dealer's face down card was: {dealerHand[1]} and received: {newDealerCard}");

                dealerValue = game.GetDealerValue(dealerHand);
            }

            Console.WriteLine($"Dealer hand value: {dealerValue}");

            return (playerValue, dealerValue);
        }

        private int HandleStand(int dealerValue)
        {
            Console.WriteLine("You chose to stand.");

            if (game.GetDealerValue(dealerHand) < dealerThreshold)
            {
                Card newDealerCard = cardGenerator.GenerateUniqueCard(new Random(), dealtCards);
                dealtCards.Add(newDealerCard);
                dealerHand.Add(newDealerCard);
                Console.WriteLine($"Dealer received: {newDealerCard}");

                dealerValue = game.GetDealerValue(dealerHand);
            }

            Console.WriteLine($"Dealer's face down card was: {dealerHand[1]}, with hand value: {dealerValue}");

            return dealerValue;
        }

        private int HandleSplit(int dealerValue)
        {
            Console.WriteLine("You chose to split.");

            playerHasSplit = true;

            playerHand1.Add(playerHand[0]);
            playerHand2.Add(playerHand[1]);

            //Deal a new card to each hand
            Card newCard1 = cardGenerator.GenerateUniqueCard(new Random(), dealtCards);
            dealtCards.Add(newCard1);
            playerHand1.Add(newCard1);

            Card newCard2 = cardGenerator.GenerateUniqueCard(new Random(), dealtCards);
            dealtCards.Add(newCard2);
            playerHand2.Add(newCard2);

            Console.WriteLine($"Player hand 1: {playerHand1[0]}, {playerHand1[1]} with value: {game.GetPlayerValue(playerHand1)}");
            Console.WriteLine($"Player hand 2: {playerHand2[0]}, {playerHand2[1]} with value: {game.GetPlayerValue(playerHand2)}");

            //Dealer plays out hand according to standard rules
            if (game.GetDealerValue(dealerHand) < dealerThreshold)
            {
                Card newDealerCard = cardGenerator.GenerateUniqueCard(new Random(), dealtCards);
                dealtCards.Add(newDealerCard);
                dealerHand.Add(newDealerCard);
                Console.WriteLine($"Dealer's face down card was: {dealerHand[1]}, for a total of {game.GetDealerValue(dealerHand)}");
                Console.WriteLine($"Dealer received: {newDealerCard}");

                dealerValue = game.GetDealerValue(dealerHand);
            }

            Console.WriteLine($"Dealer hand value: {dealerValue}");

            return dealerValue;
        }

        private (int playerValue, int dealerValue) HandleDoubleDown(int playerValue, int dealerValue)
        {
            //Check player has enough chips to double the bet
            Console.WriteLine("You chose to double down.");

            // Double the bet
            bet.BetAmount *= 2;
            Console.WriteLine($"Bet doubled to: {bet.BetAmount}");

            // Deal one more card
            Card newPlayerCard = this.cardGenerator.GenerateUniqueCard(new Random(), dealtCards);
            dealtCards.Add(newPlayerCard);
            playerHand.Add(newPlayerCard);
            Console.WriteLine($"You received: {newPlayerCard}");

            playerValue = game.GetPlayerValue(playerHand);
            Console.WriteLine($"Your hand value: {playerValue}");

            // Dealer plays out hand
            if (game.GetDealerValue(dealerHand) < dealerThreshold)
            {
                Card newDealerCard = this.cardGenerator.GenerateUniqueCard(new Random(), dealtCards);
                dealtCards.Add(newDealerCard);
                dealerHand.Add(newDealerCard);
                Console.WriteLine($"Dealer's face down card was: {dealerHand[1]} and received: {newDealerCard}");

                dealerValue = game.GetDealerValue(dealerHand);
            }

            Console.WriteLine($"Dealer hand value: {dealerValue}");

            return (playerValue, dealerValue);
        }

        private void HandleSurrender()
        {
            Console.WriteLine("You chose to surrender.");

            // Subtract half the bet amount from current chips
            int surrenderAmount = bet.BetAmount / 2;
            bet.CurrentChips -= surrenderAmount;

            Console.WriteLine($"You forfeit half your bet: {surrenderAmount} chips");
            Console.WriteLine($"Remaining chips: {bet.CurrentChips}");

            // Set a flag to indicate surrender and skip the result calculation
            // This will be handled in PlayRound by returning early
            // We'll use playerHand.Clear() as a signal to the game that round is surrendered
            playerHand.Clear();
            dealerHand.Clear();
        }
    }
}
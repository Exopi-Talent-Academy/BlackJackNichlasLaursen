using System;
using Cards;
using BlackjackProject;
using Dealing;
using Bets;
using EndStates;

namespace BlackjackGameLoop
{
    public class BlackjackGame
    {
        public EndState endState;

        public Dealer dealer;

        readonly Bet bet;

        public BlackjackGame(Cards.ICardGenerator? generator = null, Bets.Bet? betInstance = null)
        {
            bet = betInstance ?? new Bet();
            dealer = new Dealer(this, bet, generator);
            endState = new EndState() { dealer = dealer };
        }

        public void PlayRound()
        {
            Console.WriteLine("Starting Blackjack round...");

            // Place bet for the round
            bet.PlaceBet();

            // Reset hands for new round
            dealer.playerHand.Clear();
            dealer.dealerHand.Clear();

            dealer.DealCardsForStart();

            // Check if player has blackjack on initial deal
            int initialPlayerValue = GetPlayerValue(dealer.playerHand);
            if (initialPlayerValue == dealer.blackjackLimit)
            {
                // Reveal dealer's second card
                Console.WriteLine($"Dealer's second card: {dealer.dealerHand[1]}");
                int initialDealerValue = GetDealerValue(dealer.dealerHand);
                if (initialDealerValue == dealer.blackjackLimit)
                {
                    Console.WriteLine("Both have Blackjack! It's a push.");

                    // Return player's bet
                    //bet.currentChips += bet.betAmount;
                    Console.WriteLine($"Player's chips: {bet.CurrentChips}");
                }
                else
                {
                    Console.WriteLine("Player has Blackjack! Player wins.");

                    // Pay out 1.5 times the bet
                    bet.CurrentChips += (int)(bet.BetAmount * bet.blackjackPayoutMultiplier);
                    Console.WriteLine($"Player's chips: {bet.CurrentChips}");
                }
                return; // End the round early
            }

            dealer.DealCards();

            // Check if player surrendered
            if (dealer.playerHand.Count == 0)
            {
                Console.WriteLine("Surrender accepted!");
                return;
            }

            //Switch statement to determine the outcome of the round
            int playerValue = GetPlayerValue(dealer.playerHand);
            int dealerValue = GetDealerValue(dealer.dealerHand);

            //in case og a split
            int playerValue1 = GetPlayerValue(dealer.playerHand1);
            int playerValue2 = GetPlayerValue(dealer.playerHand2);

            //Switch with end states, depending on whether or not player has split
            if (!dealer.playerHasSplit)
            {
                EndState.EndStateNoSplit(playerValue, dealerValue, bet);
            }
            else if (dealer.playerHasSplit)
            {
                EndState.EndStateSplit(playerValue1, playerValue2, dealerValue, bet);

                dealer.playerHasSplit = false; //Reset split bool for next round
            }
        }

        //Calculate the total value of a hand, accounting for Aces
        public int GetPlayerValue(List<Card> handCards)
        {
            int totalValue = 0;
            int aceCount = 0;
            foreach (Card card in handCards)
            {
                if (card.Rank == "Ace")
                {
                    aceCount++;
                    totalValue += 11;
                }
                else
                {
                    totalValue += card.GetValue();
                }
            }
            // Adjust aces to 1 if bust
            while (totalValue > 21 && aceCount > 0)
            {
                totalValue -= 10;
                aceCount--;
            }
            return totalValue;
        }

        public int GetDealerValue(List<Card> dealerHandCards)
        {
            int dealerTotalValue = 0;
            int aceCount = 0;
            foreach (Card card in dealerHandCards)
            {
                if (card.Rank == "Ace")
                {
                    aceCount++;
                    dealerTotalValue += 11;
                }
                else
                {
                    dealerTotalValue += card.GetValue();
                }
            }
            // Adjust aces if bust
            while (dealerTotalValue > dealer.blackjackLimit && aceCount > 0)
            {
                dealerTotalValue -= 10;
                aceCount--;
            }
            return dealerTotalValue;
        }
    }
}
using System;
using System.Collections.Generic;

namespace Cards
{
    public class CardGenerator : ICardGenerator
    {
        public Card GenerateUniqueCard(Random rand, List<Card> existingCards)
        {
            Card newCard;
            do
            {
                var suit = Card.Suits[rand.Next(Card.Suits.Length)];
                var rank = Card.Ranks[rand.Next(Card.Ranks.Length)];
                newCard = new Card(suit, rank);
            } while (existingCards != null && existingCards.Contains(newCard));
            return newCard;
        }
    }
}

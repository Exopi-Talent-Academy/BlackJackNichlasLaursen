using System;
using System.Collections.Generic;
using Cards;

namespace BlackjackProject.tests
{
    // Deterministic card generator for tests. Returns cards from a queue.
    public class TestCardGenerator : ICardGenerator
    {
        private readonly Queue<Card> _cards;

        public TestCardGenerator(IEnumerable<Card> cards)
        {
            _cards = new Queue<Card>(cards);
        }

        public Card GenerateUniqueCard(Random rand, List<Card> existingCards)
        {
            if (_cards.Count > 0)
            {
                var c = _cards.Dequeue();
                return c;
            }
            // Fallback to random generator if queue is empty
            var generator = new CardGenerator();
            return generator.GenerateUniqueCard(rand, existingCards);
        }
    }
}

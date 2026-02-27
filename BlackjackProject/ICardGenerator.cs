using System;
using System.Collections.Generic;

namespace Cards
{
    public interface ICardGenerator
    {
        Card GenerateUniqueCard(Random rand, List<Card> existingCards);
    }
}

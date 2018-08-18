using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBlackJack.Models
{
    public class Card
    {
        public Card(CardType cardType, CardWorth cardWorth, bool isFaceUp)
        {
            this.CardType = cardType; //spades, diamonds, etc.
            this.CardWorth = cardWorth; //5, 9, Jack, Ace, etc...
            this.IsFaceUp = true;
        }
        public CardType CardType { get; set; }
        public CardWorth CardWorth { get; set; }
        public bool IsFaceUp { get; set; }

    }
}

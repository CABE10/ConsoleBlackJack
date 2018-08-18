using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBlackJack.Models
{
    public class CardWorth
    {
        //The key will be the name: Deuce, Jack, etc...
        //The value will be how we value that card: King = 10, 8 = 8, etc.
        public KeyValuePair<string, int?> CardValue { get; set; }

        public CardWorth(KeyValuePair<string, int?> cardValue)
        {
            this.CardValue = cardValue;
        }
    }
}

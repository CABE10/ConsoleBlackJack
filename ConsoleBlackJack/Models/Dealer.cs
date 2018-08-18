using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBlackJack.Models
{
    public class Dealer : Player
    {
        //The only reason this object is here is to be able to distinguish between a Player and a Dealer.
        //Other than that, they're pretty much identical.
        public Dealer(string name) : base(name)
        {

        }

    }
}

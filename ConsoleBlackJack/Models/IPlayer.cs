using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBlackJack.Models
{
    public interface IPlayer
    { //Required properties for player.
        int CurrentScore { get; set; }
        List<Card> Hand { get; set; }
        bool IsBust { get; set; }
        bool IsStaying { get; set; }
        string Name { get; set; }
        int TotalScore { get; set; }
        int TurnNumber { get; set; }
    }
}

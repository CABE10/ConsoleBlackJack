using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBlackJack.Models
{
    public class Player : IPlayer
    {
        public Player(string name)
        {
            this.Hand = new List<Card>();
            this.TotalScore = 0;
            this.Name = name;
        }
        public int CurrentScore { get; set; } //Score of the hand.
        public List<Card> Hand { get; set; } //Cards in your hand.
        public bool IsBust { get; set; } //Did we go above 21?
        public bool IsStaying { get; set; } //Are we holding?
        public string Name { get; set; } //Player Name.
        public int TotalScore { get; set; } //Total Score of all of my games.
        public int TurnNumber { get; set; } //The turn we are on.
    }
}

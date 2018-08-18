using ConsoleBlackJack.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBlackJack.Services
{
    public static class PlayerService
    {
        /// <summary>
        /// Method that determines the number of players, and gives them a name.
        /// </summary>
        public static List<Player> GetPlayers()
        {
            try
            {
                List<Player> results = new List<Player>();
                int numberOfPlayers = 1; //Rule 5: Support multiple players in the future.
                Int32.TryParse(ConfigurationManager.AppSettings["NumberOfPlayers"], out numberOfPlayers);
                if (numberOfPlayers == 0)
                    throw new ArgumentOutOfRangeException("NumberOfPlayers");
                for (int index = 0; index < numberOfPlayers; index++)
                {
                    results.Add(new Player($"Player {index + 1}"));
                }
                return results;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                Console.ReadLine();
                Environment.Exit(0);
                return null;
            }

        }
        /// <summary>
        /// Resets the immediate scores of the players as to start another game.
        /// </summary>
        public static void ResetPlayer(Player player)
        {
            player.CurrentScore = 0;
            player.Hand.Clear();
            player.TurnNumber = 0;
            player.IsStaying = false;
            player.IsBust = false;
        }

    }
}

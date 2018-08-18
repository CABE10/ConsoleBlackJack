using ConsoleBlackJack.Models;
using ConsoleBlackJack.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBlackJack
{
    class Program
    {
        //Note: Aces can count as low as well as high.
        static void AdjustConsole()
        {
            Console.WindowHeight += 30;
            Console.WindowWidth += 20;
        }
        static void Main(string[] args)
        {
            AdjustConsole();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Welcome to BlackJack!\n");
            Console.ForegroundColor = ConsoleColor.Green;
            Dealer dealer = new Dealer("Dealer"); //Rule 1: There must be a dealer.
            List<Player> players = PlayerService.GetPlayers();

            PlayGame(dealer, players);
        }
        static void PlayGame(Dealer dealer, List<Player> players)
        {
            List<Card> cards = DeckService.GetCards();
            cards = DeckService.ShuffleCards(cards);
            //Dealer is dealt two cards, one is face up.
            Console.ForegroundColor = ConsoleColor.Cyan;
            DeckService.Deal(dealer, cards, 2, false); //Rule 3: Dealer dealt 2, one face down.
            string lineSeparator = "\n____________";
            Console.WriteLine($"{dealer.Name} score is: {DeckService.GetHandScore(dealer)}{lineSeparator}");
            bool isGameOver = false;
            while (!isGameOver)
            {
                int lowestTurnCount = Math.Min(dealer.TurnNumber, players.Min(p => p.TurnNumber));
                if (dealer.TurnNumber == lowestTurnCount && !dealer.IsBust && !dealer.IsStaying)
                {
                    //Dealer's turn
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    DeckService.PlayerTurnChange(dealer, cards, players.Max(p => p.CurrentScore));
                    Console.WriteLine($"{dealer.Name} score is: {DeckService.GetHandScore(dealer)}{lineSeparator}");
                }
                else if (players.Any(p => p.TurnNumber == lowestTurnCount && !p.IsStaying && !p.IsBust))
                {
                    //Player's turn
                    foreach (var player in players.Where(p => p.TurnNumber == lowestTurnCount && !p.IsStaying && !p.IsBust))
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        //One thing I wasn't sure about was the case where the dealer's total is lower but another players total is higher...
                        //I'm not sure if the player has to hit in that case.
                        DeckService.PlayerTurnChange(player, cards, dealer.CurrentScore); //Rule 4.
                        Console.WriteLine($"{player.Name} score is: {DeckService.GetHandScore(player)}{lineSeparator}");
                    }
                }
                else
                {
                    isGameOver = true;
                }
                if (!isGameOver && (dealer.IsBust) || (dealer.IsStaying && players.All(p => p.IsStaying)) || players.All(p => p.IsBust))
                {
                    isGameOver = true;
                }

            }
            TallyScores(dealer, players);
            PlayAgainPrompt(dealer, players);
        }
        static void TallyScores(Dealer dealer, List<Player> players)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Congratulations to the winner(s):");
            if (dealer.CurrentScore != 21 && players.Any(p => p.CurrentScore != 21) &&
                dealer.CurrentScore < 21 && players.Any(p => p.CurrentScore < 21))
            { //When no one reaches 21...
                int maxScore = Math.Max(dealer.CurrentScore, players.Where(p => p.CurrentScore < 21).Max(p => p.CurrentScore));
                bool dealerPlayerTie = dealer.CurrentScore == maxScore && players.Any(p => p.CurrentScore == maxScore); //Rule 8: Tie goes to the dealer. Rule 8
                if (dealer.CurrentScore == maxScore)
                {
                    dealer.TotalScore++;
                    sb.AppendLine($"{dealer.Name} with total running score of {dealer.TotalScore}");
                }
                if (players.Any(p => p.CurrentScore == maxScore) && !dealerPlayerTie)
                {
                    foreach (var player in players.Where(p => p.CurrentScore == maxScore))
                    {
                        player.TotalScore++;
                        sb.AppendLine($"{player.Name} with total running score of {player.TotalScore}");
                    }
                }
            }
            else
            { //When someone reaches 21...
                if (!dealer.IsBust)
                {
                    dealer.TotalScore++;
                    if (dealer.CurrentScore == 21 && dealer.Hand.Count() == 2 && !players.Any(p => p.Hand.Count() == 2 && p.CurrentScore == 21))
                    { //Rule 7, 12c and 12d
                        dealer.TotalScore++;
                    }
                    sb.AppendLine($"{dealer.Name} with total running score of {dealer.TotalScore}");
                }
                foreach (var player in players.Where(p => !p.IsBust))
                {
                    player.TotalScore++;
                    if (player.CurrentScore == 21 && player.Hand.Count() == 2 && !(dealer.CurrentScore == 21 && dealer.Hand.Count() == 2))
                    {
                        player.TotalScore++;
                    }
                    sb.AppendLine($"{player.Name} with total running score of {player.TotalScore}");
                }
            }
            Console.WriteLine(sb.ToString());
        }
        static void PlayAgainPrompt(Dealer dealer, List<Player> players)
        {
            //Prompting the user to play again.
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\nWould you like to play again? y/n");
            string result = Console.ReadLine();
            if (!String.IsNullOrEmpty(result) && result == "y")
            {
                Console.Clear();
                PlayerService.ResetPlayer(dealer);
                foreach (var player in players)
                {
                    PlayerService.ResetPlayer(player);
                }
                PlayGame(dealer, players);
            }
        }
    }
}

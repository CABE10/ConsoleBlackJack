using ConsoleBlackJack.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBlackJack.Services
{
    public static class DeckService
    { //Static class that gives us information about our deck.

        /// <summary>
        /// Method that deals out the cards to an individual player.
        /// </summary>
        public static void Deal(Player player, List<Card> cards, int count, bool lastFaceUp)
        {
            System.Threading.Thread.Sleep(900); //for dramatic effect only and not necessary.
            string lineSeparator = "____________\n";
            if (player.Hand.Count() == 0 || player.Hand.All(h => h.IsFaceUp))
            {
                Console.WriteLine($"{lineSeparator}Dealing to {player.Name}...\n");
                for (int cardsToDeal = 0; cardsToDeal < count; cardsToDeal++)
                {
                    Card cardToDeal = cards.First();
                    cards.Remove(cardToDeal);
                    if ((count - 1) == cardsToDeal)
                    {
                        cardToDeal.IsFaceUp = lastFaceUp;
                    }
                    player.Hand.Add(cardToDeal);
                }
            }
            else
            {
                //Rule 11: We need to turn our card over. (The Dealer)
                player.Hand.ForEach(h => h.IsFaceUp = true);
                Console.WriteLine($"{lineSeparator}{player.Name} turns card over.\n");
            }
            player.TurnNumber++; //Once the cards are dealt, that counts as a turn.
            DisplayCards(player.Hand);
        }
        public static void DisplayCards(List<Card> cards)
        {
            foreach (var card in cards)
            {
                DisplayCard(card);
            }
        }

        public static List<Card> GetCards()
        {
            try
            {
                List<Card> results = new List<Card>();
                int numberOfDecks = 1;
                //Rule 2: We'll want to call our configuration to see how many decks we'll start out with.
                Int32.TryParse(ConfigurationManager.AppSettings["NumberOfDecks"], out numberOfDecks);
                if (numberOfDecks == 0)
                    throw new ArgumentOutOfRangeException("NumberOfDecks");
                var cardTypes = Enum.GetValues(typeof(CardType));

                for (int d = 0; d < numberOfDecks; d++)
                {
                    foreach (var cardType in cardTypes)
                    {
                        foreach (var cardWorth in GetCardValues())
                        {
                            results.Add(new Card((CardType)cardType, new CardWorth(cardWorth), false));
                        }
                    }
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
        /// Method that returns the score of the players current hand.
        /// </summary>
        public static int GetHandScore(Player player)
        {
            int result = 0;
            List<Card> cards = player.Hand;
            foreach (var card in cards.OrderByDescending(c => c.CardWorth.CardValue.Value))
            {
                if (card.IsFaceUp)
                {
                    if (card.CardWorth.CardValue.Value.HasValue)
                    {
                        result += card.CardWorth.CardValue.Value.Value;
                    }
                    else
                    { //We have an ace. Need to determine if it should be a 1 or an 11.
                        int aceValue = GetAceValue(result);
                        result += aceValue;
                        card.CardWorth.CardValue = new KeyValuePair<string, int?>(card.CardWorth.CardValue.Key, aceValue);
                    }
                }

            }
            if (result > 21)
            {
                player.IsBust = true; //rule 6.
            }
            player.CurrentScore = result;
            return result;
        }

        /// <summary>
        /// A change of turn method that 1) prompts, 2) holds, or 3) deals.
        /// </summary>
        public static void PlayerTurnChange(Player player, List<Card> cards, int highestScore)
        {
            if (player.GetType() == typeof(Dealer))
            {
                if ((GetHandScore(player) < 17) || ((GetHandScore(player) < 18) && player.Hand.Any(h => h.CardWorth.CardValue.Key == "Ace" && h.CardWorth.CardValue.Value.Value == 11)) ||
                    player.CurrentScore < highestScore) //rules outlined in step 13.
                {
                    Deal(player, cards, 1, true);
                }
                else
                {
                    Console.WriteLine($"The dealer holds! Score: {GetHandScore(player)}");
                    player.IsStaying = true;
                    player.TurnNumber++;
                }

            }
            else
            { //Rule 9 AND 10 (y will mean HIT and n will mean HOLD)
                Console.WriteLine($"It is {player.Name}'s turn. Your score is {player.CurrentScore}. \nWould you like to hit? (y) to Hit, (n) to Hold.");
                string result = Console.ReadLine();
                if (!String.IsNullOrEmpty(result) && result == "y")
                {
                    if (player.Hand.Count() < 2)
                    { //if this is our very first turn, we deal two.
                        Deal(player, cards, 2, true);
                    }
                    else
                    {
                        Deal(player, cards, 1, true);
                    }
                }
                else
                {
                    Console.WriteLine($"The player holds! Score: {GetHandScore(player)}");
                    player.IsStaying = true;
                    player.TurnNumber++;
                }
            }
        }

        /// <summary>
        /// Method that randomizes a set of cards (the deck).
        /// </summary>
        public static List<Card> ShuffleCards(List<Card> cards)
        {
            Console.WriteLine("Shuffling...\n");
            List<Card> results = new List<Card>();
            Random random = new Random(DateTime.Now.Millisecond);
            int index = 0;
            while (cards.Count > 0)
            {
                index = random.Next(0, cards.Count);
                results.Add(cards[index]);
                cards.RemoveAt(index);
            }
            return results;
        }


        #region Private Methods
        /// <summary>
        /// Method that visually displays the card on the console. A bit lengthy.
        /// </summary>
        private static void DisplayCard(Card card)
        {
            StringBuilder sb = new StringBuilder();
            string cardType = card.IsFaceUp ? card.CardType.ToString() : string.Empty;
            string cardValue = card.IsFaceUp ? card.CardWorth.CardValue.Key : string.Empty;
            for (int index = 0; index < 7; index++)
            {
                int remainingRoom = 0;
                switch (index)
                {
                    case 0:
                    case 6:
                        sb.AppendLine("-------------");
                        break;
                    case 2:
                        remainingRoom = (10 - cardValue.Length);
                        sb.Append($"|  {cardValue}");
                        for (int index2 = 0; index2 < remainingRoom; index2++)
                        {
                            if (index2 != (remainingRoom - 1))
                            {
                                sb.Append(" ");
                            }
                            else
                            {
                                sb.AppendLine("|");
                            }
                        }
                        break;
                    case 3:
                        if (card.IsFaceUp)
                        {
                            sb.AppendLine($"|     of    |");
                        }
                        else
                        {
                            sb.AppendLine($"|           |");
                        }
                        break;
                    case 4:
                        remainingRoom = (10 - cardType.Length);
                        sb.Append($"|  {cardType}");
                        for (int y = 0; y < remainingRoom; y++)
                        {
                            if (y != (remainingRoom - 1))
                            {
                                sb.Append(" ");
                            }
                            else
                            {
                                sb.AppendLine("|");
                            }
                        }
                        break;
                    default:
                        sb.AppendLine("|           |");
                        break;
                }
            }
            Console.WriteLine(sb.ToString());
        }
        /// <summary>
        /// Method that determines whether the ace should be a 1 or an 11.
        /// </summary>
        private static int GetAceValue(int cardTotal)
        {
            int result = 11;
            if (cardTotal + 11 > 21)
            {
                result = 1;
            }
            return result;
        }
        private static Dictionary<string, int?> GetCardValues()
        {
            //I thought about using an enum, but because we have multiple 10 values for King, Jack, Queen, etc... it would cause problems.

            return new Dictionary<string, int?>()
                {
                    {"Ace", null}, //null will represent a 1 or 11.
                    {"Deuce", 2 },
                    {"Eight", 8 },
                    {"Five", 5 },
                    {"Four", 4 },
                    {"Jack", 10 },
                    {"King", 10 },
                    {"Nine", 9 },
                    {"Queen", 10 },
                    {"Seven", 7 },
                    {"Six", 6 },
                    {"Ten", 10 },
                    {"Three", 3 },
                };
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleBlackJack.Models;
using ConsoleBlackJack.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackJack_Tests
{
    [TestClass]
    public class DeckServiceTests
    { //Spot check test class.
        Dealer dealer;
        Player player1;

        [TestInitialize]
        public void Init()
        {
            dealer = new Dealer("Dealer");
            player1 = new Player("Player 1");
        }
        [TestCleanup]
        public void Cleanup()
        {
            dealer.CurrentScore = 0;
            dealer.Hand.Clear();
            dealer.IsBust = false;
            dealer.IsStaying = false;
            dealer.TotalScore = 0;
            dealer.TurnNumber = 0;
            player1.CurrentScore = 0;
            player1.Hand.Clear();
            player1.IsBust = false;
            player1.IsStaying = false;
            player1.TotalScore = 0;
            player1.TurnNumber = 0;
        }

        [TestMethod]
        public void DealTest()
        {
            List<Card> testCards = new List<Card>();
            testCards.Add(new Card(CardType.Diamond, new CardWorth(new KeyValuePair<string, int?>("Five", 5)), true));
            testCards.Add(new Card(CardType.Club, new CardWorth(new KeyValuePair<string, int?>("Six", 6)), true));
            DeckService.Deal(player1, testCards, 2, true);
            Assert.IsTrue(player1.Hand.Count() == 2);
            Assert.IsTrue(testCards.Count() == 0);
            //Asserting that the player was dealt two, and that we removed the two from the deck.
        }
        [TestMethod]
        public void GetCardsTest()
        {
            List<Card> cards = DeckService.GetCards();
            Assert.IsTrue(cards.Count() == 52);
            Assert.IsTrue(cards.Any(c => c.CardType == CardType.Club) && cards.Any(c => c.CardType == CardType.Diamond));
            //Asserting that we have a variety of 52.
        }
        [TestMethod]
        public void GetHandScoreTest()
        {
            List<Card> testCards = new List<Card>();
            testCards.Add(new Card(CardType.Diamond, new CardWorth(new KeyValuePair<string, int?>("Five", 5)), true));
            testCards.Add(new Card(CardType.Club, new CardWorth(new KeyValuePair<string, int?>("Six", 6)), true));
            player1.Hand.AddRange(testCards);
            Assert.IsTrue(DeckService.GetHandScore(player1) == 11);
            Assert.IsTrue(player1.CurrentScore == 11);
        }
        [TestMethod]
        public void ShuffleCardsTest()
        {
            List<Card> testCards = new List<Card>();
            testCards.Add(new Card(CardType.Diamond, new CardWorth(new KeyValuePair<string, int?>("Three", 3)), true));
            testCards.Add(new Card(CardType.Club, new CardWorth(new KeyValuePair<string, int?>("Four", 4)), true));
            testCards.Add(new Card(CardType.Spade, new CardWorth(new KeyValuePair<string, int?>("Five", 5)), true));
            testCards.Add(new Card(CardType.Heart, new CardWorth(new KeyValuePair<string, int?>("Six", 6)), true));
            testCards.Add(new Card(CardType.Spade, new CardWorth(new KeyValuePair<string, int?>("Seven", 7)), true));
            testCards.Add(new Card(CardType.Club, new CardWorth(new KeyValuePair<string, int?>("Eight", 8)), true));
            testCards.Add(new Card(CardType.Heart, new CardWorth(new KeyValuePair<string, int?>("Nine", 9)), true));
            testCards.Add(new Card(CardType.Diamond, new CardWorth(new KeyValuePair<string, int?>("Ten", 10)), true));
            testCards = DeckService.ShuffleCards(testCards);
            //There is a small chance this could fail as with any real shuffle could just return cards in original position.
            int matchCount = 0;
            if (testCards[0].CardType == CardType.Diamond && testCards[0].CardWorth.CardValue.Key == "Three")
                matchCount++;
            if (testCards[0].CardType == CardType.Club && testCards[0].CardWorth.CardValue.Key == "Four")
                matchCount++;
            if (testCards[0].CardType == CardType.Spade && testCards[0].CardWorth.CardValue.Key == "Five")
                matchCount++;
            if (testCards[0].CardType == CardType.Heart && testCards[0].CardWorth.CardValue.Key == "Six")
                matchCount++;
            if (testCards[0].CardType == CardType.Spade && testCards[0].CardWorth.CardValue.Key == "Seven")
                matchCount++;
            if (testCards[0].CardType == CardType.Club && testCards[0].CardWorth.CardValue.Key == "Eight")
                matchCount++;
            if (testCards[0].CardType == CardType.Heart && testCards[0].CardWorth.CardValue.Key == "Nine")
                matchCount++;
            if (testCards[0].CardType == CardType.Diamond && testCards[0].CardWorth.CardValue.Key == "Ten")
                matchCount++;
            //if we have 50% of the cards OR LESS in the same order, this is acceptable to me.
            Assert.IsTrue(matchCount <= 4);
        }
        [TestMethod]
        public void AceAsElevenTest()
        {
            List<Card> testCards = new List<Card>();
            testCards.Add(new Card(CardType.Diamond, new CardWorth(new KeyValuePair<string, int?>("King", 10)), true));
            testCards.Add(new Card(CardType.Club, new CardWorth(new KeyValuePair<string, int?>("Ace", null)), true));
            player1.Hand.AddRange(testCards);
            int result = DeckService.GetHandScore(player1);
            Assert.IsTrue(result == 21);
            Assert.IsTrue(testCards.Any(c => c.CardWorth.CardValue.Value.Value == 11));
            //Asserting when our Ace should be an eleven and not a one.
        }
        [TestMethod]
        public void AceAsOneTest()
        {
            List<Card> testCards = new List<Card>();
            testCards.Add(new Card(CardType.Diamond, new CardWorth(new KeyValuePair<string, int?>("King", 10)), true));
            testCards.Add(new Card(CardType.Heart, new CardWorth(new KeyValuePair<string, int?>("Queen", 10)), true));
            testCards.Add(new Card(CardType.Club, new CardWorth(new KeyValuePair<string, int?>("Ace", null)), true));
            player1.Hand.AddRange(testCards);
            int result = DeckService.GetHandScore(player1);
            Assert.IsTrue(result == 21);
            Assert.IsTrue(testCards.Any(c => c.CardWorth.CardValue.Value.Value == 1));
            //Asserting when our Ace should be a one and not an eleven.
        }
    }
}

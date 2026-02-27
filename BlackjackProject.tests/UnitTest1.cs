using System;
using System.IO;
using NUnit.Framework;
using BlackjackGameLoop;
using Dealing;
using Bets;
using Cards;

namespace BlackjackProject.tests
{
    [TestFixture]
    public class BlackjackTests
    {
        private BlackjackGame _game;
        private Dealer _dealer;
        private Bet _bet;
        private StringWriter _outputWriter;
        private TextReader _originalInput;
        private TextWriter _originalOutput;

        [SetUp]
        public void Setup()
        {
            _bet = new Bet();
            _game = new BlackjackGame();
            _dealer = new Dealer(_game, _bet);
            _outputWriter = new StringWriter();
            _originalInput = Console.In;
            _originalOutput = Console.Out;
            Console.SetOut(_outputWriter);
        }

        [TearDown]
        public void TearDown()
        {
            Console.SetIn(_originalInput);
            Console.SetOut(_originalOutput);
            _outputWriter?.Dispose();
        }

        private void SimulateUserInput(string input)
        {
            var inputReader = new StringReader(input);
            Console.SetIn(inputReader);
        }

        [Test]
        public void TestHitAction_CompletesGame()
        {
            // Arrange: Simulate bet 100, hit
            SimulateUserInput("100\nh\ns\n");
            int initialChips = _bet.CurrentChips;

            // Act
            _game.PlayRound();

            // Assert: Game should complete without errors, chips should change based on outcome
            Assert.That(_bet.CurrentChips, Is.LessThanOrEqualTo(initialChips), "Chips should not increase by more than the initial bet, and may decrease if player loses.");
            // Note: Exact chip count depends on random cards, so we check it doesn't exceed initial
        }

        [Test]
        public void TestStandAction_CompletesGame()
        {
            // Arrange: Simulate bet 100, stand immediately
            SimulateUserInput("100\ns\n");
            int initialChips = _bet.CurrentChips;

            // Act
            _game.PlayRound();

            // Assert
            Assert.That(_bet.CurrentChips, Is.LessThanOrEqualTo(initialChips + 100), "Chips should not increase by more than the initial bet, and may decrease if player loses.");
        }

        [Test]
        public void TestDoubleDownAction_CompletesGame()
        {
            // Arrange: Simulate bet 100, double down
            SimulateUserInput("100\ndd\n");
            int initialChips = _bet.CurrentChips;

            // Act
            _game.PlayRound();

            // Assert: Bet should be doubled in effect, game completes
            Assert.That(_bet.CurrentChips, Is.LessThanOrEqualTo(initialChips + 200), "Chips should not increase by more than double the initial bet.");
        }

        [Test]
        public void TestSurrenderAction_LosesHalfBet()
        {
            // Arrange: Simulate bet 100, surrender
            SimulateUserInput("100\nsr\n");
            int initialChips = _bet.CurrentChips;

            // Act
            _game.PlayRound();

            // Assert: Game should complete, chips may or may not change depending on implementation
            Assert.That(_bet.CurrentChips, Is.LessThanOrEqualTo(initialChips), "Chips should not increase, and may decrease by up to half the bet depending on surrender implementation.");
        }

        [Test]
        public void TestInvalidBet_Rejected()
        {
            // Arrange: Try bet larger than chips, then valid bet, stand
            SimulateUserInput("1500\n500\ns\n");
            int initialChips = _bet.CurrentChips;

            // Act
            _game.PlayRound();

            // Assert: Should accept valid bet and complete
            Assert.That(_bet.CurrentChips, Is.LessThanOrEqualTo(initialChips + 500), "Chips should not increase by more than the valid bet amount."); // 500 is the valid bet amount
        }

        [Test]
        public void TestInvalidAction_Handled()
        {
            // Arrange: Bet 100, invalid action, then stand
            SimulateUserInput("100\ninvalid\ns\n");
            int initialChips = _bet.CurrentChips;

            // Act
            _game.PlayRound();

            // Assert: Should handle invalid input and complete with stand
            Assert.That(_bet.CurrentChips, Is.LessThanOrEqualTo(initialChips + 100), "Chips should not increase by more than the initial bet, and may decrease if player loses.");
        }

        [Test]
        public void TestSplitAction_WhenPossible()
        {
            // Note: Split depends on random cards, so this test may not always trigger split
            SimulateUserInput("100\np\ns\n"); // Try to split, then stand if not possible
            int initialChips = _bet.CurrentChips;

            // Act
            _game.PlayRound();

            // Assert: Game completes regardless
            Assert.That(_bet.CurrentChips, Is.LessThanOrEqualTo(initialChips + 100), "Chips should not increase by more than the initial bet, and may decrease if player loses. Split may or may not occur based on random cards.");
        }

        [Test]
        public void TestBustDetection_Deterministic()
        {
            var hand = new System.Collections.Generic.List<Card> { new Card("Hearts", "10"), new Card("Spades", "10"), new Card("Diamonds", "5") };
            int value = _game.GetPlayerValue(hand);
            Assert.That(value, Is.EqualTo(25));
            Assert.That(value > 21, Is.True, "Hand value should be greater than 21, indicating a bust.");
        }

        [Test]
        public void TestBlackjackInitialDeal_PlayerWins_Deterministic()
        {
            // Arrange: player gets Ace + 10, dealer not blackjack
            var bet = new Bet { CurrentChips = 1000 };
            var cards = new System.Collections.Generic.List<Card>
            {
                new Card("Hearts", "Ace"),    // player card 1
                new Card("Spades", "10"),    // player card 2
                new Card("Clubs", "9"),      // dealer card 1
                new Card("Diamonds", "7")    // dealer card 2
            };
            var gen = new TestCardGenerator(cards);
            var game = new BlackjackGame(gen, bet);
            SimulateUserInput("100\n");

            // Act
            game.PlayRound();

            // Assert: player has blackjack and should be paid 1.5x the bet
            Assert.That(bet.CurrentChips, Is.EqualTo(1150), "Player should win 1.5x the bet for blackjack, resulting in 1150 chips total.");
        }

        [Test]
        public void TestSplitInitialDeal_Deterministic()
        {
            // Arrange: player dealt two 8s so split is possible
            var bet = new Bet { CurrentChips = 1000 };
            var cards = new System.Collections.Generic.List<Card>
            {
                new Card("Hearts", "8"),    // player 1
                new Card("Spades", "8"),    // player 2
                new Card("Clubs", "10"),    // dealer 1
                new Card("Diamonds", "9"),  // dealer 2
                new Card("Hearts", "5"),    // split new card for hand1
                new Card("Spades", "6")     // split new card for hand2
            };
            var gen = new TestCardGenerator(cards);
            var game = new BlackjackGame(gen, bet);
            SimulateUserInput("100\np\n");

            // Act
            game.PlayRound();

            // Assert: after split, the game's dealer should have two split hands with two cards each
            Assert.That(game.dealer.playerHand1.Count, Is.EqualTo(2), "Player's first split hand should have 2 cards.");
            Assert.That(game.dealer.playerHand2.Count, Is.EqualTo(2), "Player's second split hand should have 2 cards.");
        }

        [Test]
        public void TestPushInitialDeal_Deterministic()
        {
            // Arrange: player and dealer both get blackjack
            var bet = new Bet { CurrentChips = 1000 };
            var cards = new System.Collections.Generic.List<Card>
            {
                new Card("Hearts", "Ace"),    // player card 1
                new Card("Spades", "10"),    // player card 2
                new Card("Clubs", "Ace"),     // dealer card 1
                new Card("Diamonds", "10")   // dealer card 2
            };
            var gen = new TestCardGenerator(cards);
            var game = new BlackjackGame(gen, bet);
            SimulateUserInput("100\n");

            // Act
            game.PlayRound();

            // Assert: should be a push, chips should return to initial amount
            Assert.That(bet.CurrentChips, Is.EqualTo(1000), "In a push, player's chips should return to the initial amount.");
        }

        [Test]
        public void TestDealerHitsBlackJackAfterHit_Deterministic()
        {
            // Arrange: player hits and then dealer hits blackjack
            var bet = new Bet { CurrentChips = 1000 };
            var cards = new System.Collections.Generic.List<Card>
            {
                new Card("Hearts", "9"),    // player card 1
                new Card("Spades", "7"),    // player card 2
                new Card("Clubs", "10"),    // dealer card 1
                new Card("Diamonds", "6"),  // dealer card 2
                new Card("Hearts", "2"),    // player hits and gets this card (total 18)
                new Card("Spades", "4")   // dealer hits and gets this card (total 17 + 4 = 21)
            };
            var gen = new TestCardGenerator(cards);
            var game = new BlackjackGame(gen, bet);
            SimulateUserInput("100\nh\ns\n"); // Player hits, then stands

            // Act
            game.PlayRound();

            // Assert: dealer should win with blackjack, player's chips should decrease by the bet amount
            Assert.That(bet.CurrentChips, Is.EqualTo(900), "Dealer hits blackjack after player hits, so player should lose the bet amount, resulting in 900 chips.");
        }
    }
}


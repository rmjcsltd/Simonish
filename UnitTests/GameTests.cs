using System;
using NUnit.Framework;
using Rmjcs.Simonish.Helpers;
using Rmjcs.Simonish.Models;

namespace UnitTests
{
    class GameTests
    {
        // Remember: NUnit uses a single instance for all tests !!!

        [Test]
        public void Constructor_Test()
        {
            _ = new Game();
        }

        [Test]
        public void Phase_Test()
        {
            using (Game game = new Game())
            {
                Assert.AreEqual(GamePhase.Launched, game.Phase);
                game.StartCountdown();
                Assert.AreEqual(GamePhase.Countdown, game.Phase);

                for (int i = 0; i < Constants.CountdownSteps; i++)
                {
                    game.DecrementCountdown();
                    Assert.AreEqual(GamePhase.Countdown, game.Phase);
                }

                game.StartPlay();
                Assert.AreEqual(GamePhase.Playing, game.Phase);
                game.EndPlay();
                Assert.AreEqual(GamePhase.GameOver, game.Phase);

                // Start a 2nd game from GameOver rather than Launched.
                game.StartCountdown();
                Assert.AreEqual(GamePhase.Countdown, game.Phase);
            }
        }

        [Test]
        public void Score_Test()
        {
            using (Game game = new Game())
            {
                game.StartCountdown();
                for (int i = 0; i < Constants.CountdownSteps; i++)
                {
                    game.DecrementCountdown();
                }
                int targetIndex = game.StartPlay();

                (bool Success, int Score, int NewTargetIndex) r = game.RecordHit(targetIndex);
                // Check scores goes up after hitting the right target.
                Assert.AreEqual(1, r.Score);

                targetIndex = r.NewTargetIndex;
                r = game.RecordHit(targetIndex);
                // Check scores goes up after hitting the right target.
                // Assert.AreEqual(2, r.Score);

                targetIndex = r.NewTargetIndex == 0 ? 1 : 0;
                r = game.RecordHit(targetIndex);
                // Check scores goes down after hitting the wrong target.
                Assert.AreEqual(1, r.Score);

                targetIndex = r.NewTargetIndex == 0 ? 1 : 0;
                r = game.RecordHit(targetIndex);
                // Check scores goes down after hitting the wrong target.
                Assert.AreEqual(0, r.Score);

                game.EndPlay();

                Result result = game.GetResult();

                Assert.AreEqual(0, result.Score);
                Assert.AreEqual(2, result.CorrectHits);
                Assert.AreEqual(2, result.IncorrectHits);
            }
        }

        [Test]
        public void TimeLeft_Test()
        {
            using (Game game = new Game())
            {
                // GamePhase.Launched
                Assert.AreEqual(1, game.TimeLeft);

                game.StartCountdown();
                Assert.AreEqual(1, game.TimeLeft);

                for (int i = 0; i < Constants.CountdownSteps; i++)
                {
                    game.DecrementCountdown();
                }

                game.StartPlay();
                Assert.Greater(game.TimeLeft, 0);

                game.EndPlay();
                Assert.AreEqual(0, game.TimeLeft);

                // Start a 2nd game from GameOver rather than Launched.
                game.StartCountdown();
                Assert.AreEqual(1, game.TimeLeft);
            }
        }

        [Test]
        public void DecrementCountdown_Test()
        {
            using (Game game = new Game())
            {
                // GamePhase.Launched
                Assert.Throws<InvalidOperationException>(() => game.DecrementCountdown());

                game.StartCountdown();

                for (int i = 0; i < Constants.CountdownSteps; i++)
                {
                    game.DecrementCountdown();
                }

                // Can't countdown beyond zero.
                Assert.Throws<InvalidOperationException>(() => game.DecrementCountdown());
            }
        }

        [Test]
        public void MethodOrder_Test()
        {
            using (Game game = new Game())
            {
                //Assert.Throws<InvalidOperationException>(() => game.StartCountdown());
                Assert.Throws<InvalidOperationException>(() => game.DecrementCountdown());
                Assert.Throws<InvalidOperationException>(() => game.StartPlay());
                Assert.Throws<InvalidOperationException>(() => game.RecordHit(0));
                Assert.Throws<InvalidOperationException>(() => game.EndPlay());
                Assert.Throws<InvalidOperationException>(() => game.GetResult());

                game.StartCountdown();

                Assert.Throws<InvalidOperationException>(() => game.StartCountdown());
                //Assert.Throws<InvalidOperationException>(() => game.DecrementCountdown());
                Assert.Throws<InvalidOperationException>(() => game.StartPlay());
                Assert.Throws<InvalidOperationException>(() => game.RecordHit(0));
                Assert.Throws<InvalidOperationException>(() => game.EndPlay());
                Assert.Throws<InvalidOperationException>(() => game.GetResult());

                game.DecrementCountdown();

                Assert.Throws<InvalidOperationException>(() => game.StartCountdown());
                //Assert.Throws<InvalidOperationException>(() => game.DecrementCountdown());
                Assert.Throws<InvalidOperationException>(() => game.StartPlay());
                Assert.Throws<InvalidOperationException>(() => game.RecordHit(0));
                Assert.Throws<InvalidOperationException>(() => game.EndPlay());
                Assert.Throws<InvalidOperationException>(() => game.GetResult());

                for (int i = 0; i < Constants.CountdownSteps - 1; i++)
                {
                    game.DecrementCountdown();
                }

                game.StartPlay();

                Assert.Throws<InvalidOperationException>(() => game.StartCountdown());
                Assert.Throws<InvalidOperationException>(() => game.DecrementCountdown());
                Assert.Throws<InvalidOperationException>(() => game.StartPlay());
                //Assert.Throws<InvalidOperationException>(() => game.RecordHit(0));
                //Assert.Throws<InvalidOperationException>(() => game.EndPlay());
                Assert.Throws<InvalidOperationException>(() => game.GetResult());

                game.RecordHit(0);

                Assert.Throws<InvalidOperationException>(() => game.StartCountdown());
                Assert.Throws<InvalidOperationException>(() => game.DecrementCountdown());
                Assert.Throws<InvalidOperationException>(() => game.StartPlay());
                //Assert.Throws<InvalidOperationException>(() => game.RecordHit(0));
                //Assert.Throws<InvalidOperationException>(() => game.EndPlay());
                Assert.Throws<InvalidOperationException>(() => game.GetResult());

                game.EndPlay();

                //Assert.Throws<InvalidOperationException>(() => game.StartCountdown());
                Assert.Throws<InvalidOperationException>(() => game.DecrementCountdown());
                Assert.Throws<InvalidOperationException>(() => game.StartPlay());
                Assert.Throws<InvalidOperationException>(() => game.RecordHit(0));
                Assert.Throws<InvalidOperationException>(() => game.EndPlay());
                //Assert.Throws<InvalidOperationException>(() => game.GetResult());

                game.GetResult();

                //Assert.Throws<InvalidOperationException>(() => game.StartCountdown());
                Assert.Throws<InvalidOperationException>(() => game.DecrementCountdown());
                Assert.Throws<InvalidOperationException>(() => game.StartPlay());
                Assert.Throws<InvalidOperationException>(() => game.RecordHit(0));
                Assert.Throws<InvalidOperationException>(() => game.EndPlay());
                //Assert.Throws<InvalidOperationException>(() => game.GetResult());
            }
        }
    }
}

using System;
using System.Threading;
using NUnit.Framework;
using Rmjcs.Simonish.Helpers;
using Rmjcs.Simonish.Services;
using Rmjcs.Simonish.ViewModels;
using UnitTests.Helpers;

namespace UnitTests
{
    class GameViewModelTests
    {
        // Remember: NUnit uses a single instance for all tests !!!

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            SynchronizationContext.SetSynchronizationContext(new TestSynchronizationContext());
        }

        private static GameViewModel CreateGameViewModel(ManualTimer manualTimer = null)
        {
            IXamarinWrapper xamarinWrapper = new XamarinWrapperStub();
            IFileHelper fileHelper = new FileHelperStub(0);
            manualTimer = manualTimer ?? new ManualTimer();
            ResultsService resultsService = new ResultsService(xamarinWrapper, fileHelper);
            GameService gameService = new GameService(xamarinWrapper, fileHelper, manualTimer, resultsService);

            GameViewModel gameViewModel = new GameViewModel(xamarinWrapper, gameService);

            return gameViewModel;
        }

        [Test]
        public void ConstructorTest()
        {
            using (GameViewModel gameViewModel = CreateGameViewModel())
            {
                Assert.IsNotNull(gameViewModel.StartCommand);
                Assert.IsTrue(gameViewModel.StartCommand.CanExecute(null));
                Assert.IsNotNull(gameViewModel.HitCommand);
                Assert.IsFalse(gameViewModel.HitCommand.CanExecute(null));

                Assert.AreEqual(0, gameViewModel.Score);
                Assert.AreEqual(1, gameViewModel.TimeLeft);
                Assert.IsTrue(gameViewModel.OverlayIsVisible);

                Assert.IsFalse(string.IsNullOrWhiteSpace(gameViewModel.OverlayText)); // We don't really care what the text is but it should not be blank.
                Assert.IsNotNull(gameViewModel.ButtonColours);
                Assert.AreEqual(4, gameViewModel.ButtonColours.Length);
            }
        }

        [Test]
        public void PlayTest()
        {
            ManualTimer manualTimer = new ManualTimer();
            using (GameViewModel gameViewModel = CreateGameViewModel(manualTimer))
            {
                Assert.IsTrue(gameViewModel.StartCommand.CanExecute(null));
                Assert.IsFalse(gameViewModel.HitCommand.CanExecute(null));
                Assert.IsTrue(gameViewModel.OverlayIsVisible);

                // Should not be able to Hit if a game is not in progress.
                Assert.Throws<InvalidOperationException>(() => gameViewModel.HitCommand.Execute(0));

                // Start a game.
                gameViewModel.StartCommand.Execute(null);

                // Can't start or hit during countdown.
                Assert.IsFalse(gameViewModel.StartCommand.CanExecute(null));
                Assert.IsFalse(gameViewModel.HitCommand.CanExecute(null));
                Assert.IsTrue(gameViewModel.OverlayIsVisible);

                // Should not be able to Start if a game is already in progress.
                Assert.Throws<InvalidOperationException>(() => gameViewModel.StartCommand.Execute(null));

                // Do countdown.
                for (int i = 0; i < Constants.CountdownSteps - 1; i++)
                {
                    manualTimer.Fire();
                    Assert.IsFalse(gameViewModel.HitCommand.CanExecute(null));
                }

                manualTimer.Fire();
                Assert.IsTrue(gameViewModel.HitCommand.CanExecute(0));
                Assert.IsFalse(gameViewModel.OverlayIsVisible);

                int score = 0;

                for (int i = 0; i < Constants.TargetCount; i++)
                {
                    gameViewModel.HitCommand.Execute(i);
                    Assert.AreNotEqual(score, gameViewModel.Score);
                    score = gameViewModel.Score;
                }

                Assert.Throws<ArgumentOutOfRangeException>(() => gameViewModel.HitCommand.Execute(Constants.TargetCount));

                double timeLeft = gameViewModel.TimeLeft;

                for (int i = 0; i < Constants.PlaySeconds; i++)
                {
                    Thread.Sleep(1000);
                    manualTimer.Fire();
                    Assert.AreNotEqual(timeLeft, gameViewModel.TimeLeft);
                    timeLeft = gameViewModel.TimeLeft;
                }

                Assert.AreEqual(0, timeLeft);

                Assert.IsTrue(gameViewModel.StartCommand.CanExecute(null));
                Assert.IsFalse(gameViewModel.HitCommand.CanExecute(null));
                Assert.IsTrue(gameViewModel.OverlayIsVisible);
            }
        }
    }
}

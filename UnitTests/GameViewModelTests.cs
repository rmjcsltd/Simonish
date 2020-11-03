using System;
using System.Threading;
using NUnit.Framework;
using Rmjcs.Simonish.Helpers;
using Rmjcs.Simonish.Models;
using Rmjcs.Simonish.Services;
using Rmjcs.Simonish.ViewModels;
using UnitTests.Helpers;

namespace UnitTests
{
    class GameViewModelTests
    {
        // Remember: NUnit uses a single instance for all tests !!!

        private static GameViewModel CreateGameViewModel(ManualTimer manualTimer = null)
        {
            IXamarinWrapper xamarinWrapper = new XamarinWrapperStub();
            manualTimer = manualTimer ?? new ManualTimer();
            GameService gameService = new GameService(xamarinWrapper, manualTimer);

            GameViewModel gameViewModel = new GameViewModel(gameService);

            return gameViewModel;
        }

        [Test]
        public void ConstructorTest()
        {
            GameViewModel gameViewModel = CreateGameViewModel();

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

        [Test]
        public void GetNewResultSource_Test()
        {
            // ToDo: This duplicates CreateResultsViewModel because we need access to _resultsService.

            IXamarinWrapper xamarinWrapper = new XamarinWrapperStub();
            ManualTimer manualTimer = new ManualTimer();
            GameService gameService = new GameService(xamarinWrapper, manualTimer);

            GameViewModel gameViewModel = new GameViewModel(gameService);

            INewResultSource newResultSource = gameViewModel.GetNewResultSource();

            Assert.AreSame(gameService, newResultSource);
        }

        [Test]
        public void PlayTest()
        {
            ManualTimer manualTimer = new ManualTimer();
            GameViewModel gameViewModel = CreateGameViewModel(manualTimer);

            int newResultCount = 0;
            Result newResult = null;
            gameViewModel.GetNewResultSource().NewResult += (sender, args) =>
            {
                newResultCount++;
                newResult = args.Result;
            };

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

            Assert.AreEqual(0, newResultCount);

            double timeLeft = gameViewModel.TimeLeft;

            for (int i = 0; i < Constants.PlaySeconds; i++)
            {
                Thread.Sleep(1000);
                manualTimer.Fire();
                Assert.AreNotEqual(timeLeft, gameViewModel.TimeLeft);
                timeLeft = gameViewModel.TimeLeft;
            }

            Assert.AreEqual(0, timeLeft);
            Assert.AreEqual(1, newResultCount);
            //Assert.NotNull(newResult);
            Assert.AreEqual(Constants.TargetCount, newResult.CorrectHits + newResult.IncorrectHits);

            Assert.IsTrue(gameViewModel.StartCommand.CanExecute(null));
            Assert.IsFalse(gameViewModel.HitCommand.CanExecute(null));
            Assert.IsTrue(gameViewModel.OverlayIsVisible);
        }
    }
}

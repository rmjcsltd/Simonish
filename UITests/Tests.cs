using System.Threading;
using NUnit.Framework;
using Xamarin.UITest;

namespace UITests
{
    [TestFixture(Platform.Android)]
    //[TestFixture(Platform.iOS)]
    public class Tests
    {
        IApp _app;
        readonly Platform _platform;

        public Tests(Platform platform)
        {
            this._platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            _app = AppInitializer.StartApp(_platform);
        }

        [Test]
        [Ignore("Only necessary during test development.")]
        public void JustRepl()
        {
            // Use the tree command to see the view hierarchy.

            _app.Repl();

            Assert.Pass();
        }

        [Test]
        public void PageNavigation()
        {
            // Swipe to Scores
            _app.SwipeRightToLeft("action_bar_root");
            _app.WaitForElement("ScoresScroll");

            // Swipe to About
            _app.SwipeRightToLeft("action_bar_root");
            _app.WaitForElement("AboutScroll");

            // Press Play
            _app.Tap("Play");
            _app.WaitForElement("PlayStack");

            // Press Scores
            _app.Tap("Scores");
            _app.WaitForElement("ScoresScroll");

            // Press About
            _app.Tap("About");
            _app.WaitForElement("AboutScroll");

            // Swipe back to Play
            _app.SwipeLeftToRight("action_bar_root");
            _app.SwipeLeftToRight("action_bar_root");
            _app.WaitForElement("PlayStack");
        }

        [Test]
        public void Play2Games()
        {
            for (int i = 0; i < 2; i++)
            {
                _app.Tap("Start");

                // Wait for the 3 second countdown to finish.
                _app.WaitForNoElement("OverlayLabel");

                string[] buttons = { "Green", "Red", "Yellow", "Blue" };
                int currentButtonIndex = 0;

                while (_app.Query("OverlayLabel").Length == 0)
                {
                    _app.Tap(buttons[currentButtonIndex]);
                    currentButtonIndex = (currentButtonIndex + 1) % buttons.Length;
                }
            }

            // Swipe to Scores
            _app.SwipeRightToLeft("action_bar_root");
        }

        [Test]
        public void PressDisabledButtons()
        {
            // Tap disabled buttons underneath the overlay
            _app.Tap("Green");
            _app.Tap("Red");
            _app.Tap("Yellow");
            _app.Tap("Blue");

            _app.Tap("Start");

            // Wait for the 3 second countdown to finish.
            _app.WaitForNoElement("OverlayLabel");

            // Tap disabled start during a game.
            _app.Tap("Start");

            while (_app.Query("OverlayLabel").Length == 0)
            {
                Thread.Sleep(1000);
            }

        }
    }
}

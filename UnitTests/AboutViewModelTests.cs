using System;
using NUnit.Framework;
using Rmjcs.Simonish.Helpers;
using Rmjcs.Simonish.ViewModels;
using UnitTests.Helpers;

namespace UnitTests
{
    class AboutViewModelTests
    {
        // Remember: NUnit uses a single instance for all tests !!!

        [Test]
        public void ConstructorTest()
        {
            IXamarinWrapper xamarinWrapper = new XamarinWrapperStub();
            _ = new AboutViewModel(xamarinWrapper);
        }

        [Test]
        public void GameDurationTextTest()
        {
            IXamarinWrapper xamarinWrapper = new XamarinWrapperStub();
            AboutViewModel aboutViewModel = new AboutViewModel(xamarinWrapper);

            string s = aboutViewModel.GameDurationText;

            Assert.False(string.IsNullOrWhiteSpace(s));

            bool isInt = int.TryParse(s, out int _);

            Assert.True(isInt);
        }

        [Test]
        public void BuildVersionTextTest()
        {
            IXamarinWrapper xamarinWrapper = new XamarinWrapperStub();
            AboutViewModel aboutViewModel = new AboutViewModel(xamarinWrapper);

            string s = aboutViewModel.BuildVersionText;

            Assert.False(string.IsNullOrWhiteSpace(s));
        }

        [Test]
        public void BuildDateTextTest()
        {
            IXamarinWrapper xamarinWrapper = new XamarinWrapperStub();
            AboutViewModel aboutViewModel = new AboutViewModel(xamarinWrapper);

            string s = aboutViewModel.BuildDateText;

            Assert.False(string.IsNullOrWhiteSpace(s));
        }

        [Test]
        public void ShowWebPageTest()
        {
            IXamarinWrapper xamarinWrapper = new XamarinWrapperStub();
            AboutViewModel aboutViewModel = new AboutViewModel(xamarinWrapper);

            aboutViewModel.ShowWebPageCommand.Execute(null);
            Assert.Throws<ArgumentOutOfRangeException>(() => aboutViewModel.ShowWebPageCommand.Execute(0));
            aboutViewModel.ShowWebPageCommand.Execute(1);
            aboutViewModel.ShowWebPageCommand.Execute(2);
            Assert.Throws<ArgumentOutOfRangeException>(() => aboutViewModel.ShowWebPageCommand.Execute(3));
        }
    }
}

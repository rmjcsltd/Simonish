using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using Rmjcs.Simonish.Helpers;
using Rmjcs.Simonish.Models;
using Rmjcs.Simonish.Services;
using Rmjcs.Simonish.ViewModels;
using UnitTests.Helpers;

namespace UnitTests
{
    class ResultsViewModelTests
    {
        // Remember: NUnit uses a single instance for all tests !!!

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            SynchronizationContext.SetSynchronizationContext(new TestSynchronizationContext());
        }

        private static ResultsViewModel CreateResultsViewModel()
        {
            IXamarinWrapper xamarinWrapper = new XamarinWrapperStub();
            IFileHelper fileHelper = new FileHelperStub();
            ResultsService resultsService = new ResultsService(xamarinWrapper, fileHelper);

            ResultsViewModel resultsViewModel = new ResultsViewModel(xamarinWrapper, resultsService);

            return resultsViewModel;
        }

        [Test]
        public void ConstructorTest()
        {
            ResultsViewModel resultsViewModel = CreateResultsViewModel();

            Assert.NotNull(resultsViewModel.BestResults);
            Assert.NotNull(resultsViewModel.LatestResults);
        }

        [Test]
        public void PropertyTests()
        {
            ResultsViewModel resultsViewModel = CreateResultsViewModel();

            IEnumerable<Result> bestResults = resultsViewModel.BestResults;
            resultsViewModel.BestResults = bestResults;
            resultsViewModel.BestResults = new List<Result>();

            IEnumerable<Result> latestResults = resultsViewModel.LatestResults;
            resultsViewModel.LatestResults = latestResults;
            resultsViewModel.LatestResults = new List<Result>();
        }

        [Test]
        public void NewResultsTests()
        {
            // ToDo: This duplicates CreateResultsViewModel because we need access to _resultsService.

            IXamarinWrapper xamarinWrapper = new XamarinWrapperStub();
            IFileHelper fileHelper = new FileHelperStub();
            ResultsService resultsService = new ResultsService(xamarinWrapper, fileHelper);

            ResultsViewModel resultsViewModel = new ResultsViewModel(xamarinWrapper, resultsService);

            IEnumerable<Result> bestResults = resultsViewModel.BestResults;
            IEnumerable<Result> latestResults = resultsViewModel.LatestResults;

            Result result = new Result(default, 1, 0);
            resultsService.MergeNewGameResult(result);

            Assert.AreNotSame(bestResults, resultsViewModel.BestResults);
            Assert.AreNotSame(latestResults, resultsViewModel.LatestResults);
        }
    }
}

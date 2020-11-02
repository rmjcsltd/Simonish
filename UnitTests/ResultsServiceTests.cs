using NUnit.Framework;
using System;
using System.Collections.Generic;
using Rmjcs.Simonish.Models;
using Rmjcs.Simonish.Services;
using UnitTests.Helpers;
using System.Threading;
using Rmjcs.Simonish.Helpers;

namespace UnitTests
{
    class ResultsServiceTests
    {
        // Remember: NUnit uses a single instance for all tests !!!

        // Pre-calculate the greater of MaxBestResults and MaxLatestResults.
        private const int MaxResults = Constants.MaxBestResults > Constants.MaxLatestResults ? Constants.MaxBestResults : Constants.MaxLatestResults;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            SynchronizationContext.SetSynchronizationContext(new TestSynchronizationContext());
        }

        private static ResultsService CreateScoresService(int countError)
        {
            IXamarinWrapper xamarinWrapper = new XamarinWrapperStub();
            IFileHelper fileHelper = new FileHelperStub(countError);
            ResultsService resultsService = new ResultsService(xamarinWrapper, fileHelper);

            return resultsService;
        }

        [Test]
        public void IsListSorted_Test()
        {
            List<Result> gameResults = new List<Result>();

            DateTime now = DateTime.UtcNow;
            gameResults.Add(new Result(now, 10, 0));

            // 1 item is always sorted.
            Assert.IsTrue(IsListSorted(gameResults));

            // Add the same score.
            gameResults.Add(new Result(now, 10, 0));
            Assert.IsTrue(IsListSorted(gameResults));

            // Add a lower score.
            gameResults.Add(new Result(now, 8, 0));
            Assert.IsTrue(IsListSorted(gameResults));

            // Insert a higher score.
            gameResults.Insert(0, new Result(now, 12, 0));
            Assert.IsTrue(IsListSorted(gameResults));

            // Insert an earlier matching high score.
            gameResults.Insert(0, new Result(now.AddMinutes(-1), 12, 0));
            Assert.IsTrue(IsListSorted(gameResults));

            // Add a later matching low score.
            gameResults.Add(new Result(now.AddMinutes(+1), 8, 0));
            Assert.IsTrue(IsListSorted(gameResults));

            // Failures

            // Insert a later matching high score.
            gameResults.Insert(0, new Result(now.AddMinutes(+1), 12, 0));
            Assert.IsFalse(IsListSorted(gameResults));
            gameResults.RemoveAt(0);

            // Add an earlier matching low score.
            gameResults.Add(new Result(now.AddMinutes(-1), 8, 0));
            Assert.IsFalse(IsListSorted(gameResults));
            gameResults.RemoveAt(gameResults.Count - 1);
        }

        [Test]
        public void Construction_Test()
        {
            _ = CreateScoresService(0);
        }

        [Test]
        public void EventCount_Test()
        {
            int eventCount = 0;
            ResultsService resultsService = CreateScoresService(0);
            resultsService.ResultsChanged += (sender, args) => eventCount++;

            Result result = new Result(default, 1, 0);
            resultsService.MergeNewGameResult(result);
            Assert.AreEqual(1, eventCount);

            resultsService.LoadResults();
            Assert.AreEqual(2, eventCount);

            resultsService.MergeNewGameResult(result);
            Assert.AreEqual(3, eventCount);
        }

        [Test]
        public void MergeNewGameResultNull_Test()
        {
            ResultsService resultsService = CreateScoresService(0);
            Assert.Throws<ArgumentNullException>(() => resultsService.MergeNewGameResult(null));
        }

        [Test]
        public void MergeAscendingNewResults_Test([Range(1, MaxResults + 2)] int n)
        {
            // This tests Score sorting.

            Results lastEventResults = null;
            ResultsService resultsService = CreateScoresService(n);
            resultsService.ResultsChanged += (sender, args) => lastEventResults = args.Results;

            Result result = null;
            for (int i = 1; i <= n; i++) // Ascending.
            {
                result = new Result(new DateTime(2020, 1, i, 0, 0, 0, DateTimeKind.Utc), i, 0);
                resultsService.MergeNewGameResult(result);
            }

            // Check expected number of items in collections.
            Assert.AreEqual(Math.Min(n, Constants.MaxBestResults), ((List<Result>)lastEventResults.BestResults).Count);
            Assert.AreEqual(Math.Min(n, Constants.MaxLatestResults), ((List<Result>)lastEventResults.LatestResults).Count);

            // Check the best results are correctly sorted.
            Assert.True(IsListSorted(lastEventResults.BestResults));

            // Check the latest (last merged ) is the top latest item.
            Assert.AreSame(result, ((List<Result>)lastEventResults.LatestResults)[0]);
        }

        [Test]
        public void MergeDescendingNewResults_Test([Range(1, MaxResults + 2)] int n)
        {
            // This tests Score sorting.

            Results lastEventResults = null;
            ResultsService resultsService = CreateScoresService(n);
            resultsService.ResultsChanged += (sender, args) => lastEventResults = args.Results;

            Result result = null;
            for (int i = n; i >= 1; i--) // Descending.
            {
                result = new Result(new DateTime(2020, 1, i, 0, 0, 0, DateTimeKind.Utc), i, 0);
                resultsService.MergeNewGameResult(result);
            }

            // Check expected number of items in collections.
            Assert.AreEqual(Math.Min(n, Constants.MaxBestResults), ((List<Result>)lastEventResults.BestResults).Count);
            Assert.AreEqual(Math.Min(n, Constants.MaxLatestResults), ((List<Result>)lastEventResults.LatestResults).Count);

            // Check the best results are correctly sorted.
            Assert.True(IsListSorted(lastEventResults.BestResults));

            // Check the latest (last merged ) is the top latest item.
            Assert.AreSame(result, ((List<Result>)lastEventResults.LatestResults)[0]);
        }

        [Test]
        public void MergeAscendingSameNewResults_Test([Range(1, MaxResults + 2)] int n)
        {
            // This tests StartTimeUtc sorting.

            Results lastEventResults = null;
            ResultsService resultsService = CreateScoresService(n);
            resultsService.ResultsChanged += (sender, args) => lastEventResults = args.Results;

            Result result = null;
            for (int i = 1; i <= n; i++) // Ascending.
            {
                result = new Result(new DateTime(2020, 1, i, 0, 0, 0, DateTimeKind.Utc), 10, 0);
                resultsService.MergeNewGameResult(result);
            }

            // Check expected number of items in collections.
            Assert.AreEqual(Math.Min(n, Constants.MaxBestResults), ((List<Result>)lastEventResults.BestResults).Count);
            Assert.AreEqual(Math.Min(n, Constants.MaxLatestResults), ((List<Result>)lastEventResults.LatestResults).Count);

            // Check the best results are correctly sorted.
            Assert.True(IsListSorted(lastEventResults.BestResults));

            // Check the latest (last merged ) is the top latest item.
            Assert.AreSame(result, ((List<Result>)lastEventResults.LatestResults)[0]);
        }

        [Test]
        public void MergeDescendingSameNewResults_Test([Range(1, MaxResults + 2)] int n)
        {
            // This tests StartTimeUtc sorting.

            Results lastEventResults = null;
            ResultsService resultsService = CreateScoresService(n);
            resultsService.ResultsChanged += (sender, args) => lastEventResults = args.Results;

            Result result = null;
            for (int i = n; i >= 1; i--) // Descending.
            {
                result = new Result(new DateTime(2020, 1, i, 0, 0, 0, DateTimeKind.Utc), 10, 0);
                resultsService.MergeNewGameResult(result);
            }

            // Check expected number of items in collections.
            Assert.AreEqual(Math.Min(n, Constants.MaxBestResults), ((List<Result>)lastEventResults.BestResults).Count);
            Assert.AreEqual(Math.Min(n, Constants.MaxLatestResults), ((List<Result>)lastEventResults.LatestResults).Count);

            // Check the best results are correctly sorted.
            Assert.True(IsListSorted(lastEventResults.BestResults));

            // Check the latest (last merged ) is the top latest item.
            Assert.AreSame(result, ((List<Result>)lastEventResults.LatestResults)[0]);
        }

        [Test]
        public void LoadResultsIntoEmpty_Test([Range(-MaxResults, MaxResults + 2)] int n)
        {
            Results lastEventResults = null;
            ResultsService resultsService = CreateScoresService(n);
            resultsService.ResultsChanged += (sender, args) => lastEventResults = args.Results;

            resultsService.LoadResults();

            // When n == -MaxResults LoadResults loads nothing so no ResultsChanged is fired.
            if (n == -MaxResults)
            {
                Assert.IsNull(lastEventResults);
                return;
            }

            int bestLoadedExpectedCount = Math.Clamp(Constants.MaxBestResults + n, 0, Constants.MaxBestResults);
            int latestLoadedExpectedCount = Math.Clamp(Constants.MaxLatestResults + n, 0, Constants.MaxLatestResults);

            // Check expected number of items in collections.
            Assert.AreEqual(bestLoadedExpectedCount, ((List<Result>)lastEventResults.BestResults).Count);
            Assert.AreEqual(latestLoadedExpectedCount, ((List<Result>)lastEventResults.LatestResults).Count);

            // Check the best results are correctly sorted.
            Assert.True(IsListSorted(lastEventResults.BestResults));

            //if (latestLoadedExpectedCount > 0)
            //{
            //    // Check the first loaded item is the first list item.
            //    // Check the last loaded item (up to Max...Results) is the last list item.
            //    Assert.AreSame(fileGameResults.LatestResults[0], lastEventResults.LatestResults[0]);
            //    Assert.AreSame(fileGameResults.LatestResults[latestLoadedExpectedCount - 1], lastEventResults.LatestResults[latestLoadedExpectedCount - 1]);
            //}
        }

        [Test]
        public void LoadResultsIntoHighScore_Test([Range(-MaxResults, MaxResults + 2)] int n)
        {
            Results lastEventResults = null;
            ResultsService resultsService = CreateScoresService(n);
            resultsService.ResultsChanged += (sender, args) => lastEventResults = args.Results;

            Result highResult = new Result(new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc), int.MaxValue, 0);
            resultsService.MergeNewGameResult(highResult);

            resultsService.LoadResults();

            int bestLoadedExpectedCount = Math.Clamp(Constants.MaxBestResults + n, 0, Constants.MaxBestResults - 1);
            int latestLoadedExpectedCount = Math.Clamp(Constants.MaxLatestResults + n, 0, Constants.MaxLatestResults - 1);

            // Check expected number of items in collections.
            Assert.AreEqual(bestLoadedExpectedCount + 1, ((List<Result>)lastEventResults.BestResults).Count);
            Assert.AreEqual(latestLoadedExpectedCount + 1, ((List<Result>)lastEventResults.LatestResults).Count);

            // Check the best results are correctly sorted.
            Assert.True(IsListSorted(lastEventResults.BestResults));

            // Check the initial result is still the top latest item.
            Assert.AreSame(highResult, ((List<Result>)lastEventResults.LatestResults)[0]);
        }

        [Test]
        public void LoadResultsIntoLowScore_Test([Range(-MaxResults, MaxResults + 2)] int n)
        {
            Results lastEventResults = null;
            ResultsService resultsService = CreateScoresService(n);
            resultsService.ResultsChanged += (sender, args) => lastEventResults = args.Results;

            Result lowResult = new Result(new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc), int.MinValue, 0);
            resultsService.MergeNewGameResult(lowResult);

            resultsService.LoadResults();

            int bestLoadedExpectedCount = Math.Clamp(Constants.MaxBestResults + n, 0, Constants.MaxBestResults - 1);
            int latestLoadedExpectedCount = Math.Clamp(Constants.MaxLatestResults + n, 0, Constants.MaxLatestResults - 1);

            // Check expected number of items in collections.
            Assert.AreEqual(bestLoadedExpectedCount + 1, ((List<Result>)lastEventResults.BestResults).Count);
            Assert.AreEqual(latestLoadedExpectedCount + 1, ((List<Result>)lastEventResults.LatestResults).Count);

            // Check the best results are correctly sorted.
            Assert.True(IsListSorted(lastEventResults.BestResults));

            // Check the initial result is still the top latest item.
            Assert.AreSame(lowResult, ((List<Result>)lastEventResults.LatestResults)[0]);
        }

        [Test]
        public void LoadResultsIntoEarlierSameScore_Test([Range(-Constants.MaxBestResults + 1, Constants.MaxBestResults + 2)] int n)
        {
            Results lastEventResults = null;
            ResultsService resultsService = CreateScoresService(n);
            resultsService.ResultsChanged += (sender, args) => lastEventResults = args.Results;

            // Do the load in order to find the best score + date.
            resultsService.LoadResults();
            Result result = new Result(((List<Result>)lastEventResults.BestResults)[0].StartTimeUtc.AddMonths(-1), ((List<Result>)lastEventResults.BestResults)[0].Score, 0);

            // Reset resultsService to new.
            lastEventResults = null;
            resultsService = CreateScoresService(n);
            resultsService.ResultsChanged += (sender, args) => lastEventResults = args.Results;

            resultsService.MergeNewGameResult(result);
            resultsService.LoadResults();

            int bestLoadedExpectedCount = Math.Clamp(Constants.MaxBestResults + n, 0, Constants.MaxBestResults - 1);
            int latestLoadedExpectedCount = Math.Clamp(Constants.MaxLatestResults + n, 0, Constants.MaxLatestResults - 1);

            // Check expected number of items in collections.
            Assert.AreEqual(bestLoadedExpectedCount + 1, ((List<Result>)lastEventResults.BestResults).Count);
            Assert.AreEqual(latestLoadedExpectedCount + 1, ((List<Result>)lastEventResults.LatestResults).Count);

            // Check the best results are correctly sorted.
            Assert.True(IsListSorted(lastEventResults.BestResults));

            // Check the initial result is still the top latest item.
            Assert.AreSame(result, ((List<Result>)lastEventResults.LatestResults)[0]);
        }

        [Test]
        public void LoadResultsIntoLaterSameScore_Test([Range(-Constants.MaxBestResults + 1, Constants.MaxBestResults + 2)] int n)
        {
            Results lastEventResults = null;
            ResultsService resultsService = CreateScoresService(n);
            resultsService.ResultsChanged += (sender, args) => lastEventResults = args.Results;

            // Do the load in order to find the best score + date.
            resultsService.LoadResults();
            Result result = new Result(((List<Result>)lastEventResults.BestResults)[0].StartTimeUtc.AddMonths(+1), ((List<Result>)lastEventResults.BestResults)[0].Score, 0);

            // Reset resultsService to new.
            lastEventResults = null;
            resultsService = CreateScoresService(n);
            resultsService.ResultsChanged += (sender, args) => lastEventResults = args.Results;

            resultsService.MergeNewGameResult(result);
            resultsService.LoadResults();

            int bestLoadedExpectedCount = Math.Clamp(Constants.MaxBestResults + n, 0, Constants.MaxBestResults - 1);
            int latestLoadedExpectedCount = Math.Clamp(Constants.MaxLatestResults + n, 0, Constants.MaxLatestResults - 1);

            // Check expected number of items in collections.
            Assert.AreEqual(bestLoadedExpectedCount + 1, ((List<Result>)lastEventResults.BestResults).Count);
            Assert.AreEqual(latestLoadedExpectedCount + 1, ((List<Result>)lastEventResults.LatestResults).Count);

            // Check the best results are correctly sorted.
            Assert.True(IsListSorted(lastEventResults.BestResults));

            // Check the initial result is still the top latest item.
            Assert.AreSame(result, ((List<Result>)lastEventResults.LatestResults)[0]);
        }

        [Test]
        public void LoadResultsIntoSameSameScore_Test([Range(-Constants.MaxBestResults + 1, Constants.MaxBestResults + 2)] int n)
        {
            Results lastEventResults = null;
            ResultsService resultsService = CreateScoresService(n);
            resultsService.ResultsChanged += (sender, args) => lastEventResults = args.Results;

            // Do the load in order to find the best score + date.
            resultsService.LoadResults();
            Result result = new Result(((List<Result>)lastEventResults.BestResults)[0].StartTimeUtc, ((List<Result>)lastEventResults.BestResults)[0].Score, 0);

            // Reset resultsService to new.
            lastEventResults = null;
            resultsService = CreateScoresService(n);
            resultsService.ResultsChanged += (sender, args) => lastEventResults = args.Results;

            resultsService.MergeNewGameResult(result);
            resultsService.LoadResults();

            int bestLoadedExpectedCount = Math.Clamp(Constants.MaxBestResults + n, 0, Constants.MaxBestResults - 1);
            int latestLoadedExpectedCount = Math.Clamp(Constants.MaxLatestResults + n, 0, Constants.MaxLatestResults - 1);

            // Check expected number of items in collections.
            Assert.AreEqual(bestLoadedExpectedCount + 1, ((List<Result>)lastEventResults.BestResults).Count);
            Assert.AreEqual(latestLoadedExpectedCount + 1, ((List<Result>)lastEventResults.LatestResults).Count);

            // Check the best results are correctly sorted.
            Assert.True(IsListSorted(lastEventResults.BestResults));

            // Check the initial result is still the top latest item.
            Assert.AreSame(result, ((List<Result>)lastEventResults.LatestResults)[0]);
        }

        // ----------------------------------------------------------------------------------------

        private bool IsListSorted(IEnumerable<Result> list)
        {
            return IsListSorted((List<Result>)list);
        }
        private bool IsListSorted(IList<Result> list)
        {
            // Zero or one items in a list can be considered as sorted.
            if (list.Count < 2) return true;

            for (int i = 0; i < list.Count - 1; i++)
            {
                if (list[i].Score < list[i + 1].Score ||
                    list[i].Score == list[i + 1].Score && list[i].StartTimeUtc > list[i + 1].StartTimeUtc)
                {
                    return false;
                }
            }

            return true;
        }
    }
}

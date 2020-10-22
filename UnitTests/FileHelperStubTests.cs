using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using Rmjcs.Simonish.Helpers;
using Rmjcs.Simonish.Models;
using UnitTests.Helpers;

namespace UnitTests
{
    class FileHelperStubTests
    {
        // Remember: NUnit uses a single instance for all tests !!!

        // Pre-calculate the greater of MaxBestResults and MaxLatestResults.
        private const int MaxResults = Constants.MaxBestResults > Constants.MaxLatestResults ? Constants.MaxBestResults : Constants.MaxLatestResults;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            SynchronizationContext.SetSynchronizationContext(new TestSynchronizationContext());
        }

        [Test]
        public void LogExceptionTests()
        {
            FileHelperStub fileHelperStub = new FileHelperStub();

            fileHelperStub.LogException(new Exception());
        }

        [Test]
        public void SaveResultsTests()
        {
            FileHelperStub fileHelperStub = new FileHelperStub();

            Results results = new Results();

            fileHelperStub.SaveResults(results);
        }

        [Test]
        public void LoadResultsTests([Range(1, MaxResults + 2)] int n)
        {
            FileHelperStub fileHelperStub = new FileHelperStub();

            Results results = fileHelperStub.LoadResults();

            int bestLoadedExpectedCount = Math.Clamp(Constants.MaxBestResults + n, 0, Constants.MaxBestResults);
            int latestLoadedExpectedCount = Math.Clamp(Constants.MaxLatestResults + n, 0, Constants.MaxLatestResults);

            // Check expected number of items in collections.
            Assert.AreEqual(bestLoadedExpectedCount, ((List<Result>)results.BestResults).Count);
            Assert.AreEqual(latestLoadedExpectedCount, ((List<Result>)results.LatestResults).Count);
        }
    }
}

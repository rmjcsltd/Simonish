using System;
using System.Collections.Generic;
using Rmjcs.Simonish.Helpers;
using Rmjcs.Simonish.Models;

namespace UnitTests.Helpers
{
    internal class FileHelperStub : IFileHelper
    {
        private readonly Results _results;

        public FileHelperStub() : this(0)
        {
        }

        public FileHelperStub(int countError)
        {
            // countError is the +ve/-ve error in the number of results loaded.

            // Build Results once so if LoadResults is called more than once then the same objects are returned.

            int n = Math.Max(Constants.MaxBestResults + countError, Constants.MaxLatestResults + countError);

            List<Result> bestResults = new List<Result>();
            List<Result> latestResults = new List<Result>();

            for (int i = 1; i <= n; i++)
            {
                // StartTimeUtc is ascending, CorrectHits is descending.
                Result result = new Result(new DateTime(2020, 1, i, 0, 0, 0, DateTimeKind.Utc), ((n - 1) * 10) - ((i + 1) / 2) * 10, 0);

                if (i <= Constants.MaxBestResults + countError)
                    bestResults.Add(result);

                if (i <= Constants.MaxLatestResults + countError)
                    latestResults.Insert(0, result);
            }

            _results = new Results { BestResults = bestResults, LatestResults = latestResults };
        }

        public Results LoadResults()
        {
            return _results;
        }

        public void SaveResults(Results results)
        {
            // This is a stub, no need to do anything here.
        }

        public void LogException(Exception e)
        {
            // This is a stub, no need to do anything here.
        }
    }
}

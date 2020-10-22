﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rmjcs.Simonish.Models;

namespace UnitTests
{
    class ResultsTests
    {
        // Remember: NUnit uses a single instance for all tests !!!

        [Test]
        public void Constructor_NoParams_Test()
        {
            Results results = new Results();

            Assert.AreEqual(0, ((List<Result>)results.BestResults).Count);
            Assert.AreEqual(0, ((List<Result>)results.LatestResults).Count);
        }

        [Test]
        public void Constructor_Params_Test()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new Results((string)null));
            Assert.Throws<ArgumentNullException>(() => _ = new Results((Results)null));

            Result result = new Result(default, 2, 1);
            List<Result> bestResults1 = new List<Result> { result };
            List<Result> latestResults1 = new List<Result> { result };

            Results results1 = new Results { BestResults = bestResults1, LatestResults = latestResults1 };

            Results results2 = new Results(results1);

            List<Result> latestResults2 = (List<Result>)results2.BestResults;
            List<Result> bestResults2 = (List<Result>)results2.LatestResults;

            // The point of this constructor is to create new lists from the supplied lists, not reuse the supplied lists.
            Assert.AreNotSame(bestResults1, bestResults2);
            Assert.AreNotSame(latestResults1, latestResults2);

            // It should re-use the same items though.
            Assert.AreSame(bestResults1[0], bestResults2[0]);
            Assert.AreSame(latestResults1[0], latestResults2[0]);
        }

        [Test]
        public void SerialiseDeserialise_Test()
        {
            string gameResultsText = "B1\t2\t3\rL4\t5\t6\r";
            Results results = new Results(gameResultsText);

            List<Result> bestResults = (List<Result>)results.BestResults;
            List<Result> latestResults = (List<Result>)results.LatestResults;

            Assert.AreEqual(1, bestResults[0].StartTimeUtc.Ticks);
            Assert.AreEqual(2, bestResults[0].CorrectHits);
            Assert.AreEqual(3, bestResults[0].IncorrectHits);

            Assert.AreEqual(4, latestResults[0].StartTimeUtc.Ticks);
            Assert.AreEqual(5, latestResults[0].CorrectHits);
            Assert.AreEqual(6, latestResults[0].IncorrectHits);

            Assert.AreEqual(gameResultsText, results.ToString());
        }

    }
}

using NUnit.Framework;
using Rmjcs.Simonish.Models;

namespace UnitTests
{
    class ResultTests
    {
        // Remember: NUnit uses a single instance for all tests !!!

        [Test]
        public void GameScore_Test()
        {
            Result result = new Result(default, 0, 0);
            Assert.AreEqual(0, result.Score);

            result = new Result(default, 1, 0);
            Assert.AreEqual(1, result.Score);

            result = new Result(default, 1, 1);
            Assert.AreEqual(0, result.Score);

            result = new Result(default, 1, 2);
            Assert.AreEqual(-1, result.Score);

            result = new Result(default, 2, 2);
            Assert.AreEqual(0, result.Score);
        }

        [Test]
        public void SerialiseDeserialise_Test()
        {
            string gameResultText = "1\t2\t3";

            Result result = new Result(gameResultText);

            Assert.AreEqual(1, result.StartTimeUtc.Ticks);
            Assert.AreEqual(2, result.CorrectHits);
            Assert.AreEqual(3, result.IncorrectHits);

            Assert.AreEqual(gameResultText, result.ToString());
        }


        [Test]
        public void GameDateText_Test()
        {
            Result result = new Result(default, 0, 0);

            // This test is more about code coverage than testing a basic getter/setter.

            Assert.IsFalse(string.IsNullOrWhiteSpace(result.StartTimeLocalText));
        }

        [Test]
        public void GameScoreText_Test()
        {
            Result result = new Result(default, 0, 0);

            // This test is more about code coverage than testing a basic getter/setter.

            Assert.IsFalse(string.IsNullOrWhiteSpace(result.ScoreText));
        }

    }
}

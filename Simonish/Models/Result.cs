using System;
using System.Globalization;
using Rmjcs.Simonish.Helpers;

namespace Rmjcs.Simonish.Models
{
    /// <summary>
    /// Represents the immutable result of a single play of the game.
    /// </summary>
    internal class Result
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="Result"/> class with the supplied values.
        /// </summary>
        /// <param name="startTimeUtc"></param>
        /// <param name="correctHits"></param>
        /// <param name="incorrectHits"></param>
        public Result(DateTime startTimeUtc, int correctHits, int incorrectHits)
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            StartTimeUtc = startTimeUtc;
            CorrectHits = correctHits;
            IncorrectHits = incorrectHits;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Result"/> class from the supplied string representation of a <see cref="Result"/>.
        /// </summary>
        /// <param name="gameResultText">A string representation of a <see cref="Result"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="gameResultText"/> is <see langword="null"/></exception>
        /// <exception cref="FormatException">The string representation of a <see cref="Result"/> is not valid.</exception>
        public Result(string gameResultText)
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            if (gameResultText == null)
            {
                throw new ArgumentNullException(nameof(gameResultText));
            }

            string[] values = gameResultText.Split(new[] { '\t' }, StringSplitOptions.None);
            if (values.Length != 3)
            {
                throw new FormatException("The string representation of a Result is not well formed.");
            }

            try
            {
                StartTimeUtc = new DateTime(long.Parse(values[0], CultureInfo.InvariantCulture), DateTimeKind.Utc);
                CorrectHits = int.Parse(values[1], CultureInfo.InvariantCulture);
                IncorrectHits = int.Parse(values[2], CultureInfo.InvariantCulture);
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is FormatException || ex is OverflowException)
            {
                throw new FormatException("One or more values within the string representation of a Result are not well formed.", ex);
            }
        }

        /// <summary>
        /// Gets the game start time in UTC.
        /// </summary>
        public DateTime StartTimeUtc { get; }

        /// <summary>
        /// Gets the number of correct hits made during the game.
        /// </summary>
        public int CorrectHits { get; }

        /// <summary>
        /// Gets the number of incorrect hits made during the game.
        /// </summary>
        public int IncorrectHits { get; }

        /// <summary>
        /// Gets the final score for the game.
        /// </summary>
        public int Score => CorrectHits - IncorrectHits;

        /// <summary>
        /// Converts the value of the current <see cref="Result"/> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of this object that can be used to create a new instance.</returns>
        public override string ToString() => FormattableString.Invariant($"{StartTimeUtc.Ticks}\t{CorrectHits}\t{IncorrectHits}");

        // ToDo: Should not have these formatting properties in the model class.

        /// <remarks>
        /// There's a known issue with date formatting in XAML data bound text, by doing formatting here we can avoid the issue.
        /// See https://github.com/xamarin/Xamarin.Forms/issues/2049
        /// </remarks>
        public string StartTimeLocalText => StartTimeUtc.ToLocalTime().ToString("g", CultureInfo.CurrentCulture);

        public string ScoreText => $"{Score} ({CorrectHits} - {IncorrectHits})";
    }
}

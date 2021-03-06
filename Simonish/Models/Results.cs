﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Rmjcs.Simonish.Helpers;

namespace Rmjcs.Simonish.Models
{
    /// <summary>
    /// Represents 2 immutable collections of <see cref="Result"/>.
    /// </summary>
    /// <remarks>The lists are immutable because they are exposed as IEnumerables.
    /// They _could_ be cast back to Lists then modified but this would not be good practice.</remarks>
    internal class Results
    {
        private IEnumerable<Result> _bestResults;
        private IEnumerable<Result> _latestResults;

        /// <summary>
        /// Initialises a new instance of the <see cref="Results"/> class with 2 empty collections.
        /// </summary>
        public Results()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            // Because the lists are immutable they might as well be initialised with zero capacity.
            BestResults = new List<Result>(0);
            LatestResults = new List<Result>(0);
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Results"/> class with collections copied from the supplied <see cref="Results"/> object.
        /// </summary>
        /// <param name="results"></param>
        /// <exception cref="ArgumentNullException"><paramref name="results"/> is <see langword="null"/></exception>
        public Results(Results results)
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            if (results == null)
            {
                throw new ArgumentNullException(nameof(results));
            }

            BestResults = results.BestResults;
            LatestResults = results.LatestResults;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Results"/> class from the supplied string representation of a <see cref="Results"/>.
        /// </summary>
        /// <param name="gameResultsText">A string representation of a <see cref="Results"/>.</param>
        /// <exception cref="FormatException">The string representation of a <see cref="Results"/> is not valid.</exception>
        public Results(string gameResultsText)
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            List<Result> bestResults = new List<Result>();
            List<Result> latestResults = new List<Result>();

            if (!string.IsNullOrWhiteSpace(gameResultsText))
            {
                string[] lines = gameResultsText.Split(new[] {'\r'}, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];

                    // We need at least 2 characters to have a line type and some line data.
                    if (line.Length < 3)
                    {
                        throw new FormatException($"Line {i} is too short.");
                    }

                    char lineType = line[0];

                    if (lineType == 'B')
                    {
                        bestResults.Add(new Result(line.Substring(1)));
                    }
                    else if (lineType == 'L')
                    {
                        latestResults.Add(new Result(line.Substring(1)));
                    }
                    else
                    {
                        throw new FormatException($"Line {i} does not start with B or L.");
                    }
                }
            }

            BestResults = bestResults;
            LatestResults = latestResults;
        }

        /// <summary>
        /// Gets the collection of best collections of <see cref="Result"/>s.
        /// </summary>
        public IEnumerable<Result> BestResults
        {
            get => _bestResults ?? throw new ArgumentNullException(nameof(_bestResults));
            set => _bestResults = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets the collection of latest collections of <see cref="Result"/>s.
        /// </summary>
        public IEnumerable<Result> LatestResults
        {
            get => _latestResults ?? throw new ArgumentNullException(nameof(_latestResults));
            set => _latestResults = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Converts the value of the current <see cref="Results"/> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of this object that can be used to create a new instance.</returns>
        public override string ToString()
        {
            // The are roughly 25 characters per game result string so we can pre-allocate the space we will need.
            StringBuilder builder = new StringBuilder((Constants.MaxBestResults + Constants.MaxLatestResults) * 25);

            foreach (Result result in BestResults)
            {
                builder.AppendFormat(CultureInfo.InvariantCulture, "B{0}\r", result);
            }
            foreach (Result result in LatestResults)
            {
                builder.AppendFormat(CultureInfo.InvariantCulture, "L{0}\r", result);
            }

            return builder.ToString();
        }
    }
}

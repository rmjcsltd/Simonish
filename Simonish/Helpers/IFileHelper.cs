using System;
using Rmjcs.Simonish.Models;

namespace Rmjcs.Simonish.Helpers
{
    /// <summary>
    /// An interface for a class that handles all file IO.
    /// </summary>
    internal interface IFileHelper
    {
        /// <summary>
        /// Load a set of previously saved best and latest game results.
        /// </summary>
        /// <returns>A Results with zero or more BestResults and zero or more LatestResults.</returns>
        Results LoadResults();

        /// <summary>
        /// Save the supplied best and latest game results.
        /// </summary>
        /// <param name="results">The Results to save.</param>
        void SaveResults(Results results);

        /// <summary>
        /// Write exception information to a log file.
        /// </summary>
        /// <param name="e"></param>
        void LogException(Exception e);
    }
}

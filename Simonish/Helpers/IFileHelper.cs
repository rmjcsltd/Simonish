using System;

namespace Rmjcs.Simonish.Helpers
{
    /// <summary>
    /// An interface for a class that handles all app file IO.
    /// </summary>
    internal interface IFileHelper
    {
        /// <summary>
        /// Read the text in the results file.
        /// </summary>
        /// <returns>The text from the results file.</returns>
        string ReadResultsFile();

        /// <summary>
        /// Write text to the results file.
        /// </summary>
        /// <param name="text">The text to write.</param>
        void WriteResultsFile(string text);

        /// <summary>
        /// Write exception information to a log file.
        /// </summary>
        /// <param name="e"></param>
        void LogException(Exception e);
    }
}

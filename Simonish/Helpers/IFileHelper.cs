using System;

namespace Rmjcs.Simonish.Helpers
{
    /// <summary>
    /// An interface for a class that handles all app file IO.
    /// </summary>
    internal interface IFileHelper
    {
        /// <summary>
        /// Read the text from the results file.
        /// </summary>
        /// <returns>The text from the results file, or null if the results file does not exist.</returns>
        /// <exception cref="FileException">An error occurred whilst trying to read the text from the results file.</exception>
        string ReadResultsFile();

        /// <summary>
        /// Write text to the results file.
        /// </summary>
        /// <param name="text">The text to write.</param>
        /// <exception cref="FileException">An error occurred whilst trying to write the text to the results file.</exception>
        void WriteResultsFile(string text);

        /// <summary>
        /// Write exception information to a log file.
        /// </summary>
        /// <param name="e"></param>
        void LogException(Exception e);
    }
}

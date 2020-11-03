using System;
using System.Diagnostics;
using System.IO;

namespace Rmjcs.Simonish.Helpers
{
    /// <summary>
    /// An IFileHelper for use by the app in normal operation.
    /// </summary>
    /// <remarks>A single instance of this class is created by the ViewModelLocator.</remarks>
    internal class FileHelper : IFileHelper
    {
        private readonly string _resultsFileFullName;

        private readonly object _resultsFileLocker = new object();

        /// <summary>
        /// Initialises a new instance of the <see cref="FileHelper"/> class.
        /// </summary>
        /// <param name="xamarinWrapper">The IXamarinWrapper to use to get the app data directory.</param>
        public FileHelper(IXamarinWrapper xamarinWrapper)
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            _resultsFileFullName = Path.Combine(xamarinWrapper.AppDataDirectory, "Simonish.txt");
        }

        public string ReadResultsFile()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            string text = null;

            lock (_resultsFileLocker)
            {
                if (File.Exists(_resultsFileFullName))
                {
                    try
                    {
                        text = File.ReadAllText(_resultsFileFullName);
                    }
                    catch (Exception e)
                    {
                        throw new FileException("An error occurred whilst trying to read the text from the results file.", e);
                    }
                }
            }

            return text;
        }

        public void WriteResultsFile(string text)
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            lock (_resultsFileLocker)
            {
                try
                {
                    File.WriteAllText(_resultsFileFullName, text);
                }
                catch (Exception e)
                {
                    throw new FileException("An error occurred whilst trying to write the text to the results file.", e);
                }
            }
        }

        public void LogException(Exception e)
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            // Until there is a way to use a log file there is no point creating one.
            Debug.WriteLine(e);
        }
    }
}

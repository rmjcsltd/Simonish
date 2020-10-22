using System;
using System.Diagnostics;
using System.IO;
using Rmjcs.Simonish.Models;

namespace Rmjcs.Simonish.Helpers
{
    /// <summary>
    /// An IFileHelper for use by the app in normal operation.
    /// </summary>
    /// <remarks>A single instance of this class is created by the ViewModelLocator.</remarks>
    internal class FileHelper : IFileHelper
    {
        // ToDo: Merge this into XamarinWrapper because it is Xamarin specific ?

        private readonly string _resultsFileFullName;

        private readonly object _resultsFileLocker = new object();

        public FileHelper(IXamarinWrapper xamarinWrapper)
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod());

            _resultsFileFullName = Path.Combine(xamarinWrapper.AppDataDirectory, "Simonish.txt");
        }

        public Results LoadResults()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod());

            Results results = null;

            lock (_resultsFileLocker)
            {
                if (File.Exists(_resultsFileFullName))
                {
                    string gameResultsText = File.ReadAllText(_resultsFileFullName);
                    results = new Results(gameResultsText);
                }
            }

            return results ?? new Results();
        }

        public void SaveResults(Results results)
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod());

            lock (_resultsFileLocker)
            {
                string gameResultsText = results.ToString();
                File.WriteAllText(_resultsFileFullName, gameResultsText);
            }
        }

        public void LogException(Exception e)
        {
            // Until there is a way to use a log file there is no point creating one.
            Debug.WriteLine(e);
        }
    }
}

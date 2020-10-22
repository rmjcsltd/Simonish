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

        public FileHelper(IXamarinWrapper xamarinWrapper)
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod());

            _resultsFileFullName = Path.Combine(xamarinWrapper.AppDataDirectory, "Simonish.txt");
        }

        public string ReadResultsFile()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod());

            string text = null;

            lock (_resultsFileLocker)
            {
                if (File.Exists(_resultsFileFullName))
                {
                    text = File.ReadAllText(_resultsFileFullName);
                }
            }

            return text;
        }

        public void WriteResultsFile(string text)
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod());

            lock (_resultsFileLocker)
            {
                File.WriteAllText(_resultsFileFullName, text);
            }
        }

        public void LogException(Exception e)
        {
            // Until there is a way to use a log file there is no point creating one.
            Debug.WriteLine(e);
        }
    }
}

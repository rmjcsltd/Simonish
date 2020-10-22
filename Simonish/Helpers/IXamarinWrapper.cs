using System.Threading;

namespace Rmjcs.Simonish.Helpers
{
    /// <summary>
    /// An interface for a class to wrap components that have Xamarin dependencies which complicate unit testing.
    /// </summary>
    internal interface IXamarinWrapper
    {
        /// <summary>
        /// Is the current thread the main/UI thread?
        /// </summary>
        bool IsMainThread { get; }

        /// <summary>
        /// Get the sync context for the main thread.
        /// </summary>
        SynchronizationContext MainSynchronizationContext { get; }

        /// <summary>
        /// Get the file path where the app data can be written/read.
        /// </summary>
        string AppDataDirectory { get; }

        /// <summary>
        /// Show a web page in the default browser.
        /// </summary>
        /// <param name="url">The URL of the web page to show.</param>
        void ShowWebPage(string url);
    }
}
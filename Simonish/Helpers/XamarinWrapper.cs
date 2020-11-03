using System;
using Xamarin.Essentials; // This should be the only class using Xamarin.Essentials.

namespace Rmjcs.Simonish.Helpers
{
    /// <summary>
    /// An IXamarinWrapper for use by the app in normal operation.
    /// </summary>
    /// <remarks>A single instance of this class is created by the ViewModelLocator.</remarks>
    internal class XamarinWrapper : IXamarinWrapper
    {
        public XamarinWrapper()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);
        }


        public bool IsMainThread => MainThread.IsMainThread;

        public string AppDataDirectory => FileSystem.AppDataDirectory;

        public void ShowWebPage(string url)
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            Uri uri = new Uri(url);
            Launcher.OpenAsync(uri).ConfigureAwait(false);
        }
    }
}

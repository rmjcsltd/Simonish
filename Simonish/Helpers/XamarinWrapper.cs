using System;
using System.Diagnostics;
using System.Threading;
using Xamarin.Essentials; // This should be the only class using Xamarin.Essentials.

namespace Rmjcs.Simonish.Helpers
{
    /// <summary>
    /// An IXamarinWrapper for use by the app in normal operation.
    /// </summary>
    /// <remarks>A single instance of this class is created by the ViewModelLocator.</remarks>
    internal class XamarinWrapper : IXamarinWrapper
    {
        private static readonly SynchronizationContext MainThreadSynchronizationContext;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1810:Initialize reference type static fields inline", Justification = "I want to use WriteDebugEntryMessage to track when this is called.")]
        static XamarinWrapper()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), null);

            MainThreadSynchronizationContext = MainThread.GetMainThreadSynchronizationContextAsync().Result;
        }

        public void DebugAssertMainSynchronizationContextIsCorrect()
        {
            Debug.Assert(MainSynchronizationContext == MainThread.GetMainThreadSynchronizationContextAsync().Result);
        }

        public bool IsMainThread => MainThread.IsMainThread;

        public SynchronizationContext MainSynchronizationContext => MainThreadSynchronizationContext;

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

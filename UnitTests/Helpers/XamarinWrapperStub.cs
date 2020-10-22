using System.IO;
using System.Threading;
using Rmjcs.Simonish.Helpers;

namespace UnitTests.Helpers
{
    internal class XamarinWrapperStub : IXamarinWrapper
    {
        public XamarinWrapperStub()
        {
            MainSynchronizationContext = SynchronizationContext.Current;
        }

        public bool IsMainThread => true;

        public SynchronizationContext MainSynchronizationContext { get; }

        public string AppDataDirectory => Path.GetTempPath();
        
        public void ShowWebPage(string url)
        {
            // This is a stub, no need to do anything here.
        }
    }
}

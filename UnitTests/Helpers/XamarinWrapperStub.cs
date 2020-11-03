using System.IO;
using Rmjcs.Simonish.Helpers;

namespace UnitTests.Helpers
{
    internal class XamarinWrapperStub : IXamarinWrapper
    {

        public bool IsMainThread => true;

        public string AppDataDirectory => Path.GetTempPath();
        
        public void ShowWebPage(string url)
        {
            // This is a stub, no need to do anything here.
        }
    }
}

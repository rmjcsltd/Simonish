using NUnit.Framework;
using Rmjcs.Simonish.Helpers;
using Rmjcs.Simonish.Services;
using UnitTests.Helpers;

namespace UnitTests
{
    class GameServiceTests
    {
        // Remember: NUnit uses a single instance for all tests !!!

        [Test]
        public void Constructor_Test()
        {
            IXamarinWrapper xamarinWrapper = new XamarinWrapperStub();
            ManualTimer manualTimer = new ManualTimer();
            _ = new GameService(xamarinWrapper, manualTimer);
        }
    }
}

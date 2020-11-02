using System.Threading;
using NUnit.Framework;
using Rmjcs.Simonish.Helpers;
using Rmjcs.Simonish.Services;
using UnitTests.Helpers;

namespace UnitTests
{
    class GameServiceTests
    {
        // Remember: NUnit uses a single instance for all tests !!!

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            SynchronizationContext.SetSynchronizationContext(new TestSynchronizationContext());
        }

        [Test]
        public void Constructor_Test()
        {
            IXamarinWrapper xamarinWrapper = new XamarinWrapperStub();
            ManualTimer manualTimer = new ManualTimer();
            _ = new GameService(xamarinWrapper, manualTimer);
        }
    }
}

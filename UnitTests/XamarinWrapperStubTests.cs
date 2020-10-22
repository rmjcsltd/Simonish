using System.Threading;
using NUnit.Framework;
using UnitTests.Helpers;

namespace UnitTests
{
    class XamarinWrapperStubTests
    {
        // Remember: NUnit uses a single instance for all tests !!!

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            SynchronizationContext.SetSynchronizationContext(new TestSynchronizationContext());
        }

        [Test]
        public void Tests()
        {
            XamarinWrapperStub xamarinWrapperStub = new XamarinWrapperStub();

            Assert.False(string.IsNullOrWhiteSpace(xamarinWrapperStub.AppDataDirectory));

            Assert.IsTrue(xamarinWrapperStub.IsMainThread);

            Assert.AreSame(SynchronizationContext.Current, xamarinWrapperStub.MainSynchronizationContext);
        }
    }
}

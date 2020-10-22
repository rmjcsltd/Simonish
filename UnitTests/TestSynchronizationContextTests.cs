using System.Threading;
using NUnit.Framework;
using Rmjcs.Simonish.Helpers;
using UnitTests.Helpers;

namespace UnitTests
{
    class TestSynchronizationContextTests
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
            IXamarinWrapper xamarinWrapper = new XamarinWrapperStub();

            int thisThreadId = Thread.CurrentThread.ManagedThreadId;
            int counter = 0;
            bool sameThread = false;

            xamarinWrapper.MainSynchronizationContext.Post(o =>
            {
                counter++;
                sameThread = (thisThreadId == Thread.CurrentThread.ManagedThreadId);
            }, null);

            Assert.AreEqual(1, counter);
            Assert.IsTrue(sameThread);

            xamarinWrapper.MainSynchronizationContext.Send(o =>
            {
                counter++;
                sameThread = (thisThreadId == Thread.CurrentThread.ManagedThreadId);
            }, null);

            Assert.AreEqual(2, counter);
            Assert.IsTrue(sameThread);
        }
    }
}

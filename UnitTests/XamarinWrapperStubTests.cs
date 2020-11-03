using NUnit.Framework;
using UnitTests.Helpers;

namespace UnitTests
{
    class XamarinWrapperStubTests
    {
        // Remember: NUnit uses a single instance for all tests !!!

        [Test]
        public void Tests()
        {
            XamarinWrapperStub xamarinWrapperStub = new XamarinWrapperStub();

            Assert.False(string.IsNullOrWhiteSpace(xamarinWrapperStub.AppDataDirectory));
        }
    }
}

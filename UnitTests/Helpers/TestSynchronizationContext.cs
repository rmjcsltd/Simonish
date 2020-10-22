using System.Threading;

namespace UnitTests.Helpers
{
    internal class TestSynchronizationContext : SynchronizationContext
    {
        // ToDo: Need to research this approach further.
        // https://www.dotnetcodegeeks.com/2015/10/using-a-synchronizationcontext-in-unit-tests.html

        public override void Post(SendOrPostCallback d, object state)
        {
            d(state);
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            d(state);
        }
    }
}

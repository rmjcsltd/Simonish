using System;
using NUnit.Framework;
using UnitTests.Helpers;

namespace UnitTests
{
    class ManualTimerTests
    {
        // Remember: NUnit uses a single instance for all tests !!!

        [Test]
        public void Tests()
        {
            ManualTimer manualTimer = new ManualTimer();

            int fireCount = 0;

            // Can't start without an action.
            Assert.Throws<InvalidOperationException>(() => manualTimer.Start());

            // Can't fire without an action.
            Assert.Throws<InvalidOperationException>(() => manualTimer.Fire());

            // Can't set an empty action.
            Assert.Throws<ArgumentNullException>(() => manualTimer.SetAction(null));

            manualTimer.SetAction(() => fireCount++);

            // Can't set action twice.
            Assert.Throws<InvalidOperationException>(() => manualTimer.SetAction(() => fireCount++));

            // Can't fire before starting.
            Assert.Throws<InvalidOperationException>(() => manualTimer.Fire());

            manualTimer.Start();

            Assert.AreEqual(0, fireCount);
            manualTimer.Fire();
            Assert.AreEqual(1, fireCount);

            // Can't fire again before re-starting.
            Assert.Throws<InvalidOperationException>(() => manualTimer.Fire());

            manualTimer.Start();

            Assert.AreEqual(1, fireCount);
            manualTimer.Fire();
            Assert.AreEqual(2, fireCount);
        }
    }
}

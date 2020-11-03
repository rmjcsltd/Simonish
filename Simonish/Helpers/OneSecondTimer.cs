using System;
using System.Threading;
using Xamarin.Forms;

namespace Rmjcs.Simonish.Helpers
{
    /// <summary>
    /// An ITimer that fires once after one second.
    /// </summary>
    /// <remarks>Note that the action will be invoked on a non-UI thread.</remarks>
    internal class OneSecondTimer : ITimer
    {
        private readonly TimeSpan _oneSecond = new TimeSpan(0, 0, 1);
        private long _isRunningFlag;
        private Action _action;

        public OneSecondTimer()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);
        }

        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/></exception>
        /// <exception cref="InvalidOperationException">Action has already been set.</exception>
        public void SetAction(Action action)
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            // Although there's no reason why we couldn't change the action it's not an appropriate thing to do in this app.
            if (_action != null)
            {
                throw new InvalidOperationException("Action has already been set.");
            }

            // This is not a threadsafe check because _isRunningFlag could be set after the Interlocked.Read
            // however it is still useful to prevent a single thread calling Start and SetAction in the wrong order
            // which is the more likely programming error in this app.
            if (Interlocked.Read(ref _isRunningFlag) == 1)
            {
                throw new InvalidOperationException("Action can not be set if the timer is already running.");
            }

            _action = action ?? throw new ArgumentNullException(nameof(action));
        }

        private bool TimerElapsed()
        {
            // Clear _isRunningFlag.
            // This must be done before calling _action because _action will re-start the timer if required.
            if (Interlocked.CompareExchange(ref _isRunningFlag, 0, 1) == 0)
            {
                throw new InvalidOperationException("The timer should not fire if it is not running.");
            }

            _action.Invoke();

            return false; // don't automatically run again.
        }

        /// <exception cref="InvalidOperationException">The action must be set before the timer is started.</exception>
        public void Start()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            if (_action == null)
            {
                throw new InvalidOperationException("The action must be set before the timer is started.");
            }

            // Set _isRunningFlag, unless it is already set.
            if (Interlocked.CompareExchange(ref _isRunningFlag, 1, 0) == 1)
            {
                throw new InvalidOperationException("Timer can not be started if the timer is already running.");
            }

            Device.StartTimer(_oneSecond, TimerElapsed);
        }
    }
}

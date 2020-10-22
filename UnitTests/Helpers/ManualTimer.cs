using System;
using Rmjcs.Simonish.Helpers;

namespace UnitTests.Helpers
{
    internal class ManualTimer : ITimer
    {
        private Action _action;
        private bool _enabled = false;

        public void SetAction(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            // Although there's no reason why we couldn't change the action it's not an appropriate thing to do in this app.
            if (_action != null)
            {
                throw new InvalidOperationException("Action has already been set.");
            }

            _action = action;
        }

        public void Start()
        {
            if (_action == null)
            {
                throw new InvalidOperationException("The action must be set before the timer is started.");
            }

            _enabled = true;
        }

        public void Fire()
        {
            if (_action == null)
            {
                throw new InvalidOperationException("The action must be set before the timer is fired.");
            }

            if (!_enabled)
            {
                throw new InvalidOperationException("Start must be called before the timer is fired.");
            }

            // We're emulating an AutoReset = False timer.
            _enabled = false;

            // Unlike a real System.Timers.Timer the action is invoked synchronously on the current thread.
            _action.Invoke();
        }

        #region IDispose

        public void Dispose()
        {
            // Nothing to dispose but ITimer requires this.
        }

        #endregion
    }
}

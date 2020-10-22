using System;
using System.Timers;

namespace Rmjcs.Simonish.Helpers
{
    /// <summary>
    /// An ITimer that fires once after one second.
    /// </summary>
    /// <remarks>Note that the action will be invoked on a ThreadPool thread.</remarks>
    internal class OneSecondTimer : ITimer
    {
        private readonly Timer _timer;
        private Action _action;

        public OneSecondTimer()
        {
            _timer = new Timer { Enabled = false, AutoReset = false, Interval = 1000 };
            _timer.Elapsed += TimerOnElapsed;
        }

        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/></exception>
        /// <exception cref="InvalidOperationException">Action has already been set.</exception>
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

        /// <exception cref="InvalidOperationException">The action must be set before the timer fires.</exception>
        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            // This method will be called on a ThreadPool thread.

            if (_action == null)
            {
                throw new InvalidOperationException("The action must be set before the timer fires.");
            }

            _action.Invoke();
        }

        /// <exception cref="InvalidOperationException">The action must be set before the timer is started.</exception>
        public void Start()
        {
            if (_action == null)
            {
                throw new InvalidOperationException("The action must be set before the timer is started.");
            }

            _timer.Start();
        }

        #region IDisposable

        /// <summary>
        /// Releases all resources used by the current <see cref="OneSecondTimer"/>.
        /// </summary>
        public void Dispose()
        {
            _timer?.Dispose();
        }

        #endregion
    }
}

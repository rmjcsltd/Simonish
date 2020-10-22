using System;

namespace Rmjcs.Simonish.Helpers
{
    /// <summary>
    /// An interface for a class to provide timer functionality.
    /// </summary>
    interface ITimer : IDisposable
    {
        /// <summary>
        /// Set the Action to execute when the timer fires.
        /// </summary>
        /// <param name="action">The Action to execute.</param>
        void SetAction(Action action);

        /// <summary>
        /// Start the timer timing.
        /// </summary>
        void Start();
    }
}

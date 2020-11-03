using System;

namespace Rmjcs.Simonish.Helpers
{
    /// <summary>
    /// Represents an exception that occurred during a file read/write operation.
    /// </summary>
    public class FileException : Exception
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="FileException"/> class.
        /// </summary>
        public FileException()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="FileException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public FileException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="FileException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public FileException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}

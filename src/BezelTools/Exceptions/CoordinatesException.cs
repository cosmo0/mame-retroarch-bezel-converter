using System;

namespace BezelTools.Exceptions
{
    /// <summary>
    /// Exception thrown when there is a problem with coordinates
    /// </summary>
    [Serializable]
    public class CoordinatesException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:CoordinatesException"/> class
        /// </summary>
        public CoordinatesException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CoordinatesException"/> class
        /// </summary>
        /// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
        public CoordinatesException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CoordinatesException"/> class
        /// </summary>
        /// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
        /// <param name="inner">The exception that is the cause of the current exception. </param>
        public CoordinatesException(string message, System.Exception inner) : base(message, inner)
        {
        }
    }
}

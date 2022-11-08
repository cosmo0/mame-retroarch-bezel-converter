using System;

namespace BezelTools.Exceptions
{
    /// <summary>
    /// Thrown when there is a problem with the source LAY file
    /// </summary>
    [Serializable]
    public class LayFileException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:LayFileException"/> class
        /// </summary>
        public LayFileException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LayFileException"/> class
        /// </summary>
        /// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
        public LayFileException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LayFileException"/> class
        /// </summary>
        /// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
        /// <param name="inner">The exception that is the cause of the current exception. </param>
        public LayFileException(string message, System.Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LayFileException"/> class
        /// </summary>
        /// <param name="context">The contextual information about the source or destination.</param>
        /// <param name="info">The object that holds the serialized object data.</param>
        protected LayFileException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
}

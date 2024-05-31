using System;

namespace Omu.BlazorAwesome.Core
{
    /// <summary>
    /// awesome exception
    /// </summary>
    public class AwesomeException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public AwesomeException()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public AwesomeException(string message) : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public AwesomeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
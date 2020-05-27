using System;

namespace SlackLib
{
    /// <summary>
    /// Thrown when something goes wrong with Slack API
    /// </summary>
    public class SlackLibException : Exception
    {
        public SlackLibException() { }
        public SlackLibException(string message) : base(message) { }
        public SlackLibException(string message, Exception inner) : base(message, inner) { }
    }
}
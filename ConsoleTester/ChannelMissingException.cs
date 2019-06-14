namespace ConsoleTester
{
    /// <summary>
    /// Thrown when channel doesn't have a configured webhook
    /// </summary>
    [System.Serializable]
    public class ChannelWebHookMissingException : System.Exception
    {
        public ChannelWebHookMissingException() { }
        public ChannelWebHookMissingException(string message) : base(message) { }
        public ChannelWebHookMissingException(string message, System.Exception inner) : base(message, inner) { }
        protected ChannelWebHookMissingException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
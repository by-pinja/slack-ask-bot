namespace SlackLib.Payloads
{
    public class Channel
    {
        public string Name { get; set; }
    }

    /// <summary>
    /// For some very nice reason this is actually different type than the user received by dialog submission.
    /// </summary>
    public class User
    {
        public string Username { get; set; }
    }
}
namespace SlackLib.Payloads
{
    public class Channel
    {
        public string Name { get; set; }
    }

    // For some very nice reason this is actually different type than the user received by dialog submission.
    public class User
    {
        public string Username { get; set; }
    }

    public class WithText
    {
        public string Text { get; set; }
    }
}
using System.Collections.Generic;

namespace SlackLib.Payloads
{
    public class ViewSubmission : PayloadBase
    {
        public View View { get; set; }
    }

    public class View
    {
        public State State { get; set; }
    }

    public class State
    {
        public Dictionary<string, Dictionary<string, Data>> values { get; set; }
    }

    public class Data
    {
        public string value { get; set; }
    }
}
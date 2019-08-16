using System;
using Newtonsoft.Json;

namespace SlackLib
{
    public class SlackResponseParser
    {
        public SlackResponse Parse(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("message", nameof(content));
            }

            return JsonConvert.DeserializeObject<SlackResponse>(content);
        }
    }
}
using System;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AzureFunctions
{
    public class PayloadParser
    {
        public AnswerContext Parse(string content)
        {
            var escaped = HttpUtility.ParseQueryString(content);
            var payload = escaped["payload"];
            if (payload == null)
            {
                throw new ArgumentException("No payload-element found in content");
            }

            JObject json = JsonConvert.DeserializeObject<JObject>(payload);
            
            return new AnswerContext(
                json.RequireString(x => x.trigger_id),
                json.RequireString(x => x.message.blocks[0].block_id),
                json.RequireString(x => x.channel.name),
                json.RequireString(x => x.user.username),
                json.RequireString(x => x.actions[0].text.text));
        }
    }
}
using Newtonsoft.Json;

namespace Server.Proxy
{
    public class Conversation
    {
        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}

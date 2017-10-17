using Newtonsoft.Json;

namespace Server.Proxy
{
    public class User
    {
        [JsonIgnore]
        public string ConnectionId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}

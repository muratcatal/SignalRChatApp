using System.Collections.Generic;
using Newtonsoft.Json;

namespace Server.Proxy
{
    public class UserBannedResult
    {
        //Here we didnt use JSonProperty name thus, in client side our object will have 
        // properties as here
        public List<string> Users { get; set; }
        public string BannedUser { get; set; }
    }
}

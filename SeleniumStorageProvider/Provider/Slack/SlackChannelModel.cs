using System.Collections.Generic;

namespace SeleniumStorageProvider.Provider.Slack
{
    public class SlackChannelModel
    {
        public bool ok { get; set; }
        public List<Channel> channels { get; set; }
    }

    public class Topic
    {
        public string value { get; set; }
        public string creator { get; set; }
        public int last_set { get; set; }
    }

    public class Purpose
    {
        public string value { get; set; }
        public string creator { get; set; }
        public int last_set { get; set; }
    }

    public class Channel
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool is_channel { get; set; }
        public int created { get; set; }
        public string creator { get; set; }
        public bool is_archived { get; set; }
        public bool is_general { get; set; }
        public bool is_member { get; set; }
        public List<string> members { get; set; }
        public Topic topic { get; set; }
        public Purpose purpose { get; set; }
        public int num_members { get; set; }
    }

        
    
}

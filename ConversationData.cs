using System.Collections.Generic;

namespace ChatGptBot
{
    public class ChatMessageData{
        public string role {get; set;}
        public string content {get; set;}

    }

    public class ConversationData
    {
        public List<ChatMessageData> messageHistory { get; set; } =new List<ChatMessageData>();
    }
}

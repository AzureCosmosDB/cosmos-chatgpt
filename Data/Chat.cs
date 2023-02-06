using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CosmosDB_ChatGPT.Data
{

    public class ChatSession
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string Type { get; set; }

        public string ChatSessionId { get; set; } //partition key

        public string ChatSessionName { get; set; }

        public List<ChatMessage> Messages { get; set; }

		public int Index { get; set; }

		public DateTime LastUpdated { get; set; }

		public ChatSession()
        {
            this.Id= Guid.NewGuid().ToString();
            this.Type = "ChatSession";
            this.ChatSessionId = this.Id;
            this.ChatSessionName = "New Chat";
			this.LastUpdated = DateTime.Now;
			this.Messages = new List<ChatMessage>();
        }

        public ChatSession(string chatSessionId, string chatSessionName,DateTime lastUpdated, int index) {

            this.Id = chatSessionId;
            this.Type = "ChatSession";
            this.ChatSessionId = chatSessionId;
            this.ChatSessionName = chatSessionName;
            this.LastUpdated = lastUpdated;
            this.Index = index;
            this.Messages = new List<ChatMessage>();
        }

        public void AddMessage(ChatMessage message) {
        
            Messages.Add(message);
        }
    }

    public class ChatMessage
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string Type { get; set; }

        public string ChatSessionId { get; set; } //partition key

        public DateTime TimeStamp { get; set; }
        
        public string Sender { get; set; }
        
        public string Text { get; set; }

        public ChatMessage(string ChatSessionId, string Sender, string Text)
        {

            this.Id = Guid.NewGuid().ToString();
            this.Type = "ChatMessage";
            this.ChatSessionId = ChatSessionId; //partition key
            this.Sender = Sender;
            this.TimeStamp = DateTime.UtcNow; //need to make ISO 8601
            this.Text = Text;

        }
    }
}

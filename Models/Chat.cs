using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CosmosDB_ChatGPT.Models
{
    public class Chat
    {
        [Required]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Prompt { get; set; }

        [Required]
        public string Response { get; set; }

        [Required]
        public string DateTime { get; set; }

        public int ExtendedResponseOrdinal { get; set; }
        

        public Chat (string Id, string UserName, string Prompt, string Response, string DateTime, int? ExtendedResponseOrdinal)
        {
            this.Id = Id;
            this.UserName = UserName;
            this.Prompt = Prompt;
            this.Response = Response;
            this.DateTime = DateTime;
            if(ExtendedResponseOrdinal != null)
                this.ExtendedResponseOrdinal = ExtendedResponseOrdinal.Value;
        }

    }
}

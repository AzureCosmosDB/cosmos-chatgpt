using CosmosDB_ChatGPT.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Collections;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System;

namespace CosmosDB_ChatGPT.Services
{
    public class CosmosService
    {
        
        private readonly CosmosClient cosmosClient;
        private readonly Container chatContainer;

        public CosmosService(IConfiguration configuration)
        {
            /*
            string uri = configuration["CosmosUri"];
            string key = configuration["CosmosKey"];
            string database = configuration["CosmosDatabase"];
            string container = configuration["CosmosContainer"];

            cosmosClient = new CosmosClient(key, uri);

            chatContainer = CreateContainerIfNotExistsAsync(database, container).Result;
            */
        }

        
        // First call is made to this when chat page is loaded for left-hand nav.
        // Only retrieve the chat sessions, not chat messages
        public async Task<List<ChatSession>> GetChatSessionsListAsync()
        {
            List<ChatSession> chatSessions = new();

            try { 
                //Get documents that are the chat sessions without the chat message documents.
                QueryDefinition query = new QueryDefinition("SELECT DISTINCT c.id, c.Type, c.ChatSessionId, c.ChatSessionName FROM c WHERE c.Type = @Type")
                    .WithParameter("@Type", "ChatSession");

                FeedIterator<ChatSession> results = chatContainer.GetItemQueryIterator<ChatSession>(query);

                while (results.HasMoreResults)
                {
                    int i = 0;
                    FeedResponse<ChatSession> response = await results.ReadNextAsync();
                    foreach (ChatSession chatSession in response)
                    {
						chatSession.Index= ++i;
						chatSessions.Add(chatSession);
                    }
                
                }
            }
            catch(CosmosException ce)
            {
                //if 404, first run, create a new default chat session document.
                if (ce.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ChatSession chatSession = new ChatSession();
                    await InsertChatSessionAsync(chatSession);
                    chatSessions.Add(chatSession);
                }

            }

            return chatSessions;

        }

        public async Task<ChatSession> InsertChatSessionAsync(ChatSession chatSession)
        {

            return await chatContainer.CreateItemAsync<ChatSession>(chatSession, new PartitionKey(chatSession.ChatSessionId));

        }

        public async Task<ChatSession> UpdateChatSessionAsync(ChatSession chatSession)
        {

            return await chatContainer.ReplaceItemAsync(item: chatSession, id: chatSession.Id, partitionKey: new PartitionKey(chatSession.ChatSessionId));

        }

        public async Task DeleteChatSessionAsync(string chatSessionId)
        {
            //Retrieve the chat session and all the chat message items for a chat session
            QueryDefinition query = new QueryDefinition("SELECT c.id, c.ChatSessionId FROM c WHERE c.id = @Type")
                    .WithParameter("@Type", "ChatSession");

            FeedIterator<dynamic> results = chatContainer.GetItemQueryIterator<dynamic>(query);

            while (results.HasMoreResults)
            {
                FeedResponse<dynamic> response = await results.ReadNextAsync();
                foreach (var item in response)
                {
                    await chatContainer.DeleteItemAsync(id: item.id, partitionKey: new PartitionKey(item.ChatSessionId));
                }

            }

        }

        public async Task<ChatMessage> InsertChatMessageAsync(ChatMessage chatMessage)
        {

            return await chatContainer.CreateItemAsync<ChatMessage>(chatMessage, new PartitionKey(chatMessage.ChatSessionId));
            
        }

        public async Task<List<ChatMessage>> GetChatSessionMessagesAsync(string chatSessionId)
        {

            //Get the chat messages for a chat session
            QueryDefinition query = new QueryDefinition("SELECT * FROM c WHERE c.ChatSessionId = @ChatSessionId AND c.Type = @Type")
                .WithParameter("@ChatSessionId", chatSessionId)
                .WithParameter("@ChatSession", "ChatSession");

            FeedIterator<ChatMessage> results = chatContainer.GetItemQueryIterator<ChatMessage>(query);

            List<ChatMessage> chatMessages= new List<ChatMessage>();
            
            while (results.HasMoreResults)
            {
                FeedResponse<ChatMessage> response = await results.ReadNextAsync();
                foreach (ChatMessage chatMessage in response)
                {
                    chatMessages.Add(chatMessage);
                }
            }

            return chatMessages;

        }


        public async Task<Container> CreateContainerIfNotExistsAsync(string databaseId, string containerId)
        {

            Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);

            ContainerProperties properties = new ContainerProperties();

            properties.PartitionKeyDefinitionVersion = PartitionKeyDefinitionVersion.V2;
            properties.PartitionKeyPath = "/ChatSessionId";

            properties.IndexingPolicy.Automatic = true;
            properties.IndexingPolicy.IndexingMode = IndexingMode.Consistent;
            properties.IndexingPolicy.IncludedPaths.Add(new IncludedPath { Path = "/*" });
            properties.IndexingPolicy.ExcludedPaths.Add(new ExcludedPath { Path = "/Response/?" });
            properties.IndexingPolicy.CompositeIndexes.Add(
                new Collection<CompositePath> { 
                    new CompositePath() { 
                        Path = "/ChatName", Order = CompositePathSortOrder.Ascending,}, 
                    new CompositePath() { 
                        Path = "/DateTime", Order = CompositePathSortOrder.Ascending 
                    } 
                });

            ThroughputProperties throughput = ThroughputProperties.CreateAutoscaleThroughput(1000);

            return await database.CreateContainerIfNotExistsAsync(properties, throughput);

            
        }
    }

}

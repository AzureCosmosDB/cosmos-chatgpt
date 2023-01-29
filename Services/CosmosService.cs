using CosmosDB_ChatGPT.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Collections;
using System.Collections.ObjectModel;

namespace CosmosDB_ChatGPT.Services
{
    public class CosmosService
    {
        
        private readonly CosmosClient cosmosClient;
        private readonly Container chatContainer;
    

        public CosmosService(IConfiguration configuration)
        {
            
            string uri = configuration["CosmosUri"];
            string key = configuration["CosmosKey"];
            string database = configuration["CosmosDatabase"];
            string container = configuration["CosmosContainer"];

            cosmosClient = new CosmosClient(key, uri);

            chatContainer = cosmosClient.GetDatabase(database).GetContainer(container);
            //chatContainer = CreateContainerIfNotExistsAsync(database, container).Wait();
        }


        public async Task<Chat> InsertChatAsync(Chat chat)
        {
            return await chatContainer.CreateItemAsync(chat, new PartitionKey(chat.UserName));
        }
        
        public async Task<Chat> ReadChatAsync(string UserName, string id)
        {
            return await chatContainer.ReadItemAsync<Chat>(partitionKey: new PartitionKey(UserName), id: id);
        }

        public async Task<Chat> ReplaceChatAsync(Chat chat)
        {
            return await chatContainer.ReplaceItemAsync<Chat>(partitionKey: new PartitionKey(chat.UserName) , id: chat.Id, item: chat);
        }

        public async Task DeleteChatAsync(string UserName, string id)
        {
            await chatContainer.DeleteItemAsync<Chat>(partitionKey: new PartitionKey(UserName), id: id);
        }

        public async Task<List<Chat>> GetChatsForSession(string UserName)
        {
            QueryDefinition query = new QueryDefinition(
                "SELECT * FROM c WHERE c.UserName = @UserName")
                .WithParameter("@UserName", UserName);

            FeedIterator<Chat> results = chatContainer.GetItemQueryIterator<Chat>(
                query,
                requestOptions: new QueryRequestOptions()
                {
                    PartitionKey = new PartitionKey(UserName)
                });

            List<Chat> sessionChats = new List<Chat>();
            while (results.HasMoreResults)
            {
                FeedResponse<Chat> response = await results.ReadNextAsync();
                sessionChats.Add(response.First());
            }

            return sessionChats;
        }

        public async Task<List<Chat>> GetAllChats()
        {
            QueryDefinition query = new QueryDefinition("SELECT * FROM c");

            FeedIterator<Chat> results = chatContainer.GetItemQueryIterator<Chat>(query);

            List<Chat> sessionChats = new List<Chat>();
            while (results.HasMoreResults)
            {
                FeedResponse<Chat> response = await results.ReadNextAsync();
                sessionChats.Add(response.First());
            }

            return sessionChats;
        }

        public async Task<Container> CreateContainerIfNotExistsAsync(string databaseId, string containerId)
        {

            Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);

            //ContainerProperties properties = new ContainerProperties();
            //properties.PartitionKeyDefinitionVersion = PartitionKeyDefinitionVersion.V2;
            //properties.PartitionKeyPath = "/userName";
            //properties.IndexingPolicy.Automatic = true;
            //properties.IndexingPolicy.IndexingMode = IndexingMode.Consistent;
            //properties.IndexingPolicy.IncludedPaths.Add(new IncludedPath { Path = "/*" });
            //properties.IndexingPolicy.ExcludedPaths.Add(new ExcludedPath { Path = "Response/?" });

            //ThroughputProperties t = new ThroughputProperties();


            //Container container = database.CreateContainerIfNotExistsAsync(properties, new ThroughputProperties(400))

            //ContainerProperties properties = new ContainerProperties()
            //{
            //    Id = containerId,
            //    PartitionKeyPath = "/userName",
            //    PartitionKeyDefinitionVersion = PartitionKeyDefinitionVersion.V2,
            //    IndexingPolicy = new IndexingPolicy()
            //    {
            //        Automatic = true,
            //        IndexingMode = IndexingMode.Consistent,
            //        IncludedPaths = new Collection<IncludedPath>
            //        {
            //            new IncludedPath { Path = "/*" }
            //        },
            //        ExcludedPaths = new Collection<ExcludedPath>
            //        {
            //            new ExcludedPath { Path = "/Response/?"}
            //        }
            //    }
            //};

            return await database.CreateContainerIfNotExistsAsync(containerId, "/UserName", 400);

            
        }
    }
}

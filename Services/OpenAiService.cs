using CosmosDB_ChatGPT.Models;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CosmosDB_ChatGPT.Services
{
    public class OpenAiService
    {
        
        private readonly HttpClient httpClient;

        public OpenAiService(IConfiguration configuration) 
        {
            string uri = configuration["OpenAiUri"];
            string key = configuration["OpenAiKey"];
            string deployment = configuration["OpenAiDeployment"];
            string serviceUri = uri + "/openai/deployments/" + deployment + "/completions?api-version=2022-12-01";

            httpClient = new()
            {
                BaseAddress = new Uri(serviceUri)
            };

            httpClient.DefaultRequestHeaders.Add("api-key", key); //auth key
            //httpClient.DefaultRequestHeaders.Add("Content-Type", "application-json");
        }

        public async Task<string> PostAsync(string Prompt)
        {

            return FakeResponse();

            using StringContent jsonContent = new(
                JsonSerializer.Serialize(new
                {
                    model = "text-davinci-003",
                    prompt = Prompt,
                    temperature = 0     //sampling temperature
                }),
                Encoding.UTF8,
                "application/json");


            using HttpResponseMessage response = await httpClient.PostAsync(
                "Cosmos-ChatGPT",
                jsonContent);

            response.EnsureSuccessStatusCode();

            string jsonString = await response.Content.ReadAsStringAsync();

            return jsonString;  //temporary until have api-key

            //Max response size is 16 KB
            //OpenAiResponse? openAiResponse = JsonSerializer.Deserialize<OpenAiResponse>(jsonString);
            //string? chatResponse = openAiResponse.ojbect;
            //return chatResponse;

        }

        private static string FakeResponse()
        {
            dynamic json = new
            {
                id = "cmpl-GERzeJQ4lvqPk8SkZu4XMIuR",
                @object = "text_completion",
                created = 1586839808,
                model = "text-davinci:003",
                choices = new[]
                {
                    new
                    {
                        text = "\n\nThis is indeed a test",
                        index = 0,
                        logprobs = "",
                        finish_reason = "length"
                    }
                },
                usage = new
                    {
                        prompt_tokens = 5,
                        completion_tokens = 7,
                        total_tokens = 12
                    }
            };

            return json;

        }

    }

    

    public class OpenAiResponse
    {
        public string? id;
        public string? ojbect;
        public int created;
        public string? model;
        public List<OpenAiResponseChoices>? choices;
    }

    public class OpenAiResponseChoices
    {
        string? text;
        string? index;
        string? logprobs;
        string? finish_reason;
    }
}

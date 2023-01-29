using CosmosDB_ChatGPT.Models;
using System;
using System.Configuration;
using System.Xml.Linq;

namespace CosmosDB_ChatGPT.Services
{
    public class ChatService
    {

        private List<Chat> chatList;

        private readonly CosmosService cosmos;
        private readonly OpenAiService openAi;

        public ChatService(IConfiguration configuration) 
        { 

            chatList= new List<Chat>();

            cosmos = new CosmosService(configuration);
            openAi = new OpenAiService(configuration);

        }

        public async Task<List<Chat>> GetChats()
        {
            chatList = await cosmos.GetAllChats();
            return chatList;
        }

        public async Task<Chat> GetChat(string UserName, string Id)
        {
            var index = chatList.FindIndex(p => p.UserName == UserName && p.Id == Id);
            if (chatList[index].Response is null)
                chatList[index] = await cosmos.ReadChatAsync(UserName, Id);

            return chatList[index];
        }

        public async Task<Chat> AddNewChat(Chat chat)
        {
            Chat newChat = await cosmos.InsertChatAsync(chat);

            chatList.Add(newChat);

            return newChat;
        }

        public async Task Delete(string UserName, string Id)
        {
            var chat = await GetChat(UserName, Id);
            if (chat is null)
                return;

            await cosmos.DeleteChatAsync(UserName, Id);

            chatList.Remove(chat);
        }
        public async Task<Chat> Update(Chat chat)
        {
            var index = chatList.FindIndex(p => p.UserName == chat.UserName && p.Id == chat.Id);

            //if (index == -1)

            Chat updatedChat = await cosmos.ReplaceChatAsync(chat);

            chatList[index] = updatedChat;

            return updatedChat;
        }
    }
}

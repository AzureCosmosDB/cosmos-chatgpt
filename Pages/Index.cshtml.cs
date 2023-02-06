using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CosmosDB_ChatGPT.Services;
using CosmosDB_ChatGPT.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CosmosDB_ChatGPT.Pages
{
    public class IndexModel : PageModel
    {

        private readonly ChatService _chatService;
        
        public List<ChatSession> ChatSessions { get; set; } = default!;
        public List<ChatMessage> ChatMessages { get; set; }

        public List<ChatSession> ChatSessionList { get; set; }


        [BindProperty(SupportsGet = true)]
        public string Prompt { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ChatSessionId { get; set; }

        public SelectList ChatSessionNames { get; set; }

        public IndexModel(ChatService chatService)
        {
            _chatService = chatService;

            ChatSessions = _chatService.GetAllChatSessionsAsync().Result;

            ChatSessionList = new List<ChatSession>();

            foreach(var item in ChatSessions)
            {
                ChatSessionList.Add(new ChatSession(item.ChatSessionId, item.ChatSessionName));
            }

            
        }

       

         

        public async Task<IActionResult> OnChatSessionSelect(string chatSessionId)
        {
            

            string csid = chatSessionId;

            return Page();
        }

        
        public async Task<IActionResult> OnPostAsync()
        {

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if(!string.IsNullOrEmpty(Prompt))
            {
                await _chatService.AskOpenAi(ChatSessionId, Prompt);
            }

            return RedirectToPage("./Index");

        }

        


    }
}
using CosmosDB_ChatGPT.Models;
using CosmosDB_ChatGPT.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CosmosDB_ChatGPT.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly ChatService _chatService;

		public HomeController(ILogger<HomeController> logger, ChatService chatService)
		{
			_logger = logger;
			_chatService = chatService;


		}

		public async Task<IActionResult> Index()
		{
			return View(await _chatService.GetAllChatSessionsAsync());
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
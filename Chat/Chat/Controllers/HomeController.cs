using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Chat.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Chat.Controllers;

public class HomeController(ILogger<HomeController> logger) : Controller
{
    private readonly ILogger<HomeController> _logger = logger;

    [HttpGet]
    public IActionResult Index()
    {
        var model = new ChatModel(systemMessage: "You are a friendly AI chatbot that helps users answers questions. Always format response using markdown");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Chat([FromForm] ChatModel model, [FromServices] Kernel kernel)
    {

        if (ModelState.IsValid)
        {
            var chatService = kernel.GetRequiredService<IChatCompletionService>();
            model.ChatHistory.AddUserMessage(model.Prompt!);
            var response = await chatService.GetChatMessageContentsAsync(model.ChatHistory);
            model.ChatHistory.AddRange(response);
            //reset prompt
            model.Prompt = string.Empty;
            return PartialView("ChatHistoryPartialView", model);
        }

        return BadRequest(ModelState);
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

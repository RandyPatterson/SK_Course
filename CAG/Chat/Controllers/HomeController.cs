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
    public async Task<IActionResult> Chat(
        [FromForm]      ChatModel model,
        [FromServices]  Kernel kernel,
        [FromServices]  PromptExecutionSettings promptSettings)
    {

        if (ModelState.IsValid)
        {
            var chatService = kernel.GetRequiredService<IChatCompletionService>();
            model.ChatHistory.AddUserMessage(model.Prompt!);
            var history = new ChatHistory(model.ChatHistory);
            var response = await chatService.GetChatMessageContentAsync(history,promptSettings,kernel);
            model.ChatHistory.Add(response);
            //reset prompt
            model.Prompt = string.Empty;
            return PartialView("ChatHistoryPartialView", model);
        }

        return BadRequest(ModelState);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

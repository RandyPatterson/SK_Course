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
        // Add CSS to head
        var css = @"
            .no-drop-indicator {
                position: fixed;
                pointer-events: none;
                z-index: 9999;
                width: 32px;
                height: 32px;
                border-radius: 50%;
                border: 2px solid red;
                display: none;
            }
            .no-drop-indicator::after {
                content: '';
                position: absolute;
                top: 50%;
                left: -2px;
                right: -2px;
                height: 2px;
                background-color: red;
                transform: rotate(45deg);
            }";
        
        ViewData["CustomCSS"] = css;
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

    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return Json(new { success = false, message = "No file uploaded" });

        try
        {
            // Create unique filename
            var fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(file.FileName)}";
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            
            // Create directory if it doesn't exist
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var filePath = Path.Combine(uploadsPath, fileName);

            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Json(new { success = true, fileName = fileName });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file");
            return Json(new { success = false, message = "Error uploading file" });
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

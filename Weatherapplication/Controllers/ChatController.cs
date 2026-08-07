using Google.GenAI;
using Microsoft.AspNetCore.Mvc;

namespace Weatherapplication.Controllers
{
    public class ChatController : Controller
    {
        private readonly IConfiguration _configuration;

        public ChatController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
        //{
        //    //try
        //    //{
        //    //    var apiKey = _configuration["Gemini:ApiKey"]?.Trim();

        //    //    var client = new Client(apiKey);

        //    //    var response = await client.Models.GenerateContentAsync(
        //    //        model: "gemini-2.5-flash",
        //    //        contents: request.Message);

        //    //    return Json(new
        //    //    {
        //    //        reply = response.Text
        //    //    });
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    return Json(new
        //    //    {
        //    //        reply = ex.Message
        //    //    });
        //    //}
        //}
    }

    public class ChatRequest
    {
        public string Message { get; set; } = "";
    }
}
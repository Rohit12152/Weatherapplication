namespace Weatherapplication.Models
{
    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
    }

    // Gemini se jo response aayega uske liye Model
    public class ChatResponse
    {
        public string Reply { get; set; } = string.Empty;
        public bool IsSuccess { get; set; } = true;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}

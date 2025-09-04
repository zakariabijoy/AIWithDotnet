using Microsoft.Extensions.AI;
using OllamaSharp;
using OllamaSharp.Models.Chat;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;


GitHubModelsTextOnly();
//GitHubModelsTextAndModel(); 
//await OllamaModelsTextAndImageModel();
//LmStudioModelsTextAndImage();





#region Github Models text only
void GitHubModelsTextOnly()
{
    var apiKey = Environment.GetEnvironmentVariable("GITHUB_MODEL_API_KEY");
    if (string.IsNullOrWhiteSpace(apiKey))
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Missing API key. Set environment variable GITHUB_MODEL_API_KEY.");
        Console.ResetColor();
        return;
    }

    var endpoint = new Uri("https://models.github.ai/inference");
    var model = "openai/gpt-4.1";

    var clientOptions = new OpenAIClientOptions
    {
        Endpoint = endpoint
    };

    var client = new ChatClient(model, new ApiKeyCredential(apiKey), clientOptions);

    // Persistent conversation history
    var messages = new List<OpenAI.Chat.ChatMessage>
{
    new SystemChatMessage("You are a helpful assistant.")
};

    Console.WriteLine("Continuous chat started. Type /exit to quit, /reset to clear history (keeps system prompt).");
    Console.WriteLine();

    while (true)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("You: ");
        Console.ResetColor();
        var input = Console.ReadLine();

        if (input is null) continue;
        if (input.Equals("/exit", StringComparison.OrdinalIgnoreCase)) break;

        if (input.Equals("/reset", StringComparison.OrdinalIgnoreCase))
        {
            // Replace this line:
            // messages.RemoveAll(m => m.Role != ChatRole.System);

            // With this code, which preserves only the system message(s) using type checking:
            messages.RemoveAll(m => m.GetType().Name != "SystemChatMessage");
            Console.WriteLine("[History cleared]");
            continue;
        }

        if (string.IsNullOrWhiteSpace(input)) continue;

        // Append user message
        messages.Add(new UserChatMessage(input));

        try
        {
            // Options can be adjusted as needed
            var requestOptions = new ChatCompletionOptions
            {
                // MaxOutputTokens = 512,
                // Temperature = 0.7f
            };

            var response = client.CompleteChat(messages, requestOptions);

            var assistantText = response.Value.Content.Count > 0
                ? response.Value.Content[0].Text
                : "(No content returned)";

            // Append assistant reply to history
            messages.Add(new AssistantChatMessage(assistantText));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Assistant: " + assistantText);
            Console.ResetColor();

            // (Optional) token usage if exposed by SDK (pseudo — uncomment if available)
            // Console.WriteLine($"Usage: prompt={response.Value.Usage.PromptTokens}, completion={response.Value.Usage.CompletionTokens}, total={response.Value.Usage.TotalTokens}");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: " + ex.Message);
            Console.ResetColor();
        }
    }

    Console.WriteLine("Session ended.");
}
#endregion



#region  Github Models text and image
void GitHubModelsTextAndModel()
{
    var apiKey = Environment.GetEnvironmentVariable("GITHUB_MODEL_API_KEY");
    var endpoint = new Uri("https://models.github.ai/inference");
    var model = "openai/gpt-5";

    var clientOptions = new OpenAIClientOptions
    {
        Endpoint = endpoint
    };

    var client = new ChatClient(model, new ApiKeyCredential(apiKey), clientOptions);

    var systemMessage = "You are an expert handwriting analysis assistant. Your task is to compare handwritten text in multiple images and determine if they match the same handwriting style. Analyze the following characteristics: letter formation, stroke patterns, spacing, slant, and overall writing style. Calculate a matching percentage based on these factors. If the matching percentage is 80% or higher, respond with 'MATCHED - [percentage]%'. If below 80%, respond with 'NOT MATCHED - [percentage]%'. Always include the specific percentage in your response.";

    var imageUrl = @"D:\Work\Personal\Practice_Learning\AI_ML\DotNet\AIWithDotnet\LLMModlesWithDotnet\nz hand.jpg";
    var imageUrl2 = @"D:\Work\Personal\Practice_Learning\AI_ML\DotNet\AIWithDotnet\LLMModlesWithDotnet\nz.jpg";

    var messages = new List<OpenAI.Chat.ChatMessage>
    {
        new SystemChatMessage(systemMessage),
        new UserChatMessage(new List<ChatMessageContentPart>
        {
            //ChatMessageContentPart.CreateImagePart(new Uri(imageUrl)),
            //ChatMessageContentPart.CreateImagePart(new Uri(imageUrl2))
            ChatMessageContentPart.CreateImagePart(new BinaryData(File.ReadAllBytes(imageUrl)),"image/jpg"),
            ChatMessageContentPart.CreateImagePart(new BinaryData(File.ReadAllBytes(imageUrl2)),"image/jpg"),
        })
    };

    try
    {
        var response = client.CompleteChat(messages);
        var assistantText = response.Value.Content.Count > 0
            ? response.Value.Content[0].Text
            : "(No content returned)";

        Console.WriteLine("Assistant: " + assistantText);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }
}
#endregion

#region OllamaModelsTextAndModelOnly

async Task OllamaModelsTextAndImageModel()
{
    IChatClient client = new OllamaApiClient(
        new Uri("http://localhost:11434/"),
        "gemma3:4b" 
    );

    var systemMessage =
        "You are an expert handwriting analysis assistant. " +
        "Compare handwritten text in multiple images and determine if they match the same handwriting style. " +
        "Analyze: letter formation, stroke patterns, spacing, slant, and overall writing style. " +
        "Calculate a matching percentage. " +
        "If >= 80%: respond with 'MATCHED - [percentage]%'. " +
        "If < 80%: respond with 'NOT MATCHED - [percentage]%'. " +
        "Always include the percentage.";

    var imagePath1 = @"D:\Work\Personal\Practice_Learning\AI_ML\DotNet\AIWithDotnet\LLMModlesWithDotnet\nz hand.jpg";
    var imagePath2 = @"D:\Work\Personal\Practice_Learning\AI_ML\DotNet\AIWithDotnet\LLMModlesWithDotnet\nz.jpg";

    var messages = new Microsoft.Extensions.AI.ChatMessage(Microsoft.Extensions.AI.ChatRole.System, systemMessage);
    messages.Contents.Add(new DataContent(File.ReadAllBytes(imagePath1), "image/jpg"));
    messages.Contents.Add(new DataContent(File.ReadAllBytes(imagePath2), "image/jpg"));


    try
    {
        var response = await client.GetResponseAsync(messages);
        Console.WriteLine(response.Text);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }
}
#endregion


#region  LmStudio text and image
void LmStudioModelsTextAndImage()
{
    var apiKey = "LmStudiot";
    var endpoint = new Uri("http://localhost:1234/v1");
    var model = "gemma-3-4b";

    var clientOptions = new OpenAIClientOptions
    {
        Endpoint = endpoint,
        NetworkTimeout = TimeSpan.FromMinutes(10),
    };

    var client = new ChatClient(model, new ApiKeyCredential(apiKey), clientOptions);

    var systemMessage = "You are an expert handwriting analysis assistant. Your task is to compare handwritten text in multiple images and determine if they match the same handwriting style. Analyze the following characteristics: letter formation, stroke patterns, spacing, slant, and overall writing style. Calculate a matching percentage based on these factors. If the matching percentage is 80% or higher, respond with 'MATCHED - [percentage]%'. If below 80%, respond with 'NOT MATCHED - [percentage]%'. Always include the specific percentage in your response.";

    var imageUrl = @"D:\Work\Personal\Practice_Learning\AI_ML\DotNet\AIWithDotnet\LLMModlesWithDotnet\nz hand.jpg";
    var imageUrl2 = @"D:\Work\Personal\Practice_Learning\AI_ML\DotNet\AIWithDotnet\LLMModlesWithDotnet\nz.jpg";

    var messages = new List<OpenAI.Chat.ChatMessage>
    {
        new SystemChatMessage(systemMessage),
        new UserChatMessage(new List<ChatMessageContentPart>
        {
            ChatMessageContentPart.CreateImagePart(new BinaryData(File.ReadAllBytes(imageUrl)),"image/jpg"),
            ChatMessageContentPart.CreateImagePart(new BinaryData(File.ReadAllBytes(imageUrl2)),"image/jpg"),
        })
    };

    try
    {
        var response = client.CompleteChat(messages);
        var assistantText = response.Value.Content.Count > 0
            ? response.Value.Content[0].Text
            : "(No content returned)";

        Console.WriteLine("Assistant: " + assistantText);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }
}
#endregion
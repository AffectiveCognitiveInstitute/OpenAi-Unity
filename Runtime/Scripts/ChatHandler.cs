public class ChatHandler : OpenAIHandler
{
    public string SystemMessage;
    public string Model = "gpt-4o-mini";
    //private ChatClient _chatEndpoint => OpenAIClient.GetChatClient();

    //private bool first = true;
    //private Task _task;
    //private void Update()
    //{
    //    if (first)
    //    {
    //        first = false; /*new AudioClient("tts-1", ApiKey).GenerateSpeechAsync("", GeneratedSpeechVoice.)*/
    //        _task = Task.Run(async () =>
    //        {
    //            try
    //            {
    //                var res = await GetResponseAsync("Write a haiku about recursion in programming.");
    //            }
    //            catch (Exception e)
    //            {
    //                Debug.LogException(e);
    //            }
    //        });
    //    }
    //}

    public ChatContext StartConversation() => new ChatContext(OpenAIClient, Model, SystemMessage);
    public ChatContext StartConversation(string systemMessage) => new ChatContext(OpenAIClient, Model, systemMessage ?? SystemMessage);

    //public async Task<string> GetResponseAsync(string userInput, CancellationToken cancellationToken = default)
    //    => await GetResponseAsync(SystemMessage, userInput, cancellationToken);

    //public async Task<string> GetResponseAsync(string systemMessage, string userInput, CancellationToken cancellationToken = default)
    //{
    //    var messages = new List<Message>
    //    {
    //        new Message(Role.System, systemMessage),
    //        new Message(Role.User, userInput)
    //    };

    //    var request = new ChatRequest(messages, Model);
    //    var response = await _chatEndpoint.GetCompletionAsync(request, cancellationToken);

    //    return response.FirstChoice.Message;
    //}

    //public string GetResponse(string userInput)
    //    => GetResponse(SystemMessage, userInput);

    //public string GetResponse(string systemMessage, string userInput)
    //{
    //    var messages = new List<Message>
    //    {
    //        new Message(Role.System, systemMessage),
    //        new Message(Role.User, userInput)
    //    };

    //    var request = new ChatRequest(messages, Model);
    //    var response = _chatEndpoint.GetCompletionAsync(request)
    //        .ConfigureAwait(false)
    //        .GetAwaiter()
    //        .GetResult();

    //    return response.FirstChoice.Message;
    //}
}
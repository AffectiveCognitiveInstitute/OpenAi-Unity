using OpenAI;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

public class ChatContext
{
    private readonly ChatClient _chatClient;
    private readonly OpenAIClient _openAIClient;
    private readonly string _model;
    private readonly List<Message> _history = new List<Message>();
    private readonly MainThreadDispatcher _threadDispatcher;
    private bool _isRunning = false;
    private List<Task> _tasks = new List<Task>();

    public ChatContext(OpenAIClient openAIClient, string model, string systemMessage = null)
    {
        _openAIClient = openAIClient;
        _chatClient = openAIClient.GetChatClient(model);
        _model = model;
        _threadDispatcher = MainThreadDispatcher.Instance;
        if (!string.IsNullOrWhiteSpace(systemMessage))
        {
            _history.Add(new Message(ChatMessageRole.System, systemMessage));
        }
    }

    public void EnqueueCompletion(string userInput, Action<string> action)
    {
        Debug.LogWarning($"EnqueueCompletion: {userInput}");
        var task = Task.Run(async () =>
        {
            Debug.LogWarning($"Run completion...");
            try
            {
                var result = await GetCompletionAsync(userInput);
                Debug.LogWarning($"Finished completion! {result}");
                _threadDispatcher.Enqueue(() =>
                {
                    Debug.LogWarning($"Run callback");
                    action?.Invoke(result);
                });
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        });
        _tasks.Add(task);
    }

    public async Task<string> GetCompletionAsync(string userInput, CancellationToken cancellationToken = default)
    {
        Assert.IsFalse(string.IsNullOrWhiteSpace(userInput), $"The parameter {nameof(userInput)} must not be null or whitespace!");
        Assert.IsFalse(_isRunning, "Only single thread input is allowed");

        _isRunning = true;
        _history.Add(new Message(ChatMessageRole.User, userInput));
        var messages = _getMessages();
        var response = await _chatClient.CompleteChatAsync(messages, null, cancellationToken);
        var result = response.Value.Content[0].Text;
        _history.Add(new Message(ChatMessageRole.Assistant, response.Value.Content[0].Text));
        _isRunning = false;

        return result;
    }

    private List<ChatMessage> _getMessages()
    {
        var result = new List<ChatMessage>();
        foreach (var item in _history)
        {
            switch (item.Role)
            {
                case ChatMessageRole.Assistant:
                    result.Add(ChatMessage.CreateAssistantMessage(item.Content));
                    break;
                case ChatMessageRole.User:
                    result.Add(ChatMessage.CreateUserMessage(item.Content));
                    break;
                case ChatMessageRole.System:
                    result.Add(ChatMessage.CreateSystemMessage(item.Content));
                    break;
                case ChatMessageRole.Tool:
                case ChatMessageRole.Function:
                default:
                    break;
            }
        }
        return result;
    }
}

public class Message
{
    public ChatMessageRole Role { get; set; }
    public string Content { get; set; }
    public Message(ChatMessageRole role, string content)
    {
        Role = role;
        Content = content;
    }
}
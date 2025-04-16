using OpenAI;
using UnityEngine;
using UnityEngine.Assertions;

public class OpenAIHandler : MonoBehaviour
{
    public string ApiKey;

    /// <summary>
    /// zb. https://llm-proxy.imla.hs-offenburg.de
    /// </summary>
    public string Endpoint;

    protected OpenAIClient OpenAIClient;
    protected MainThreadDispatcher MainThreadDispatcher;

    void OnEnable()
    {
        Debug.LogWarning("Initialize OpenAI Handler.");
        MainThreadDispatcher = MainThreadDispatcher.Instance;

        Assert.IsNotNull(ApiKey, $"{nameof(ApiKey)} is null!");
        var apiKey = new System.ClientModel.ApiKeyCredential(ApiKey);
        OpenAIClientOptions options = null;
        if (!string.IsNullOrWhiteSpace(Endpoint))
        {
            options = new OpenAIClientOptions()
            {
                Endpoint = new System.Uri(Endpoint)
            };
        }
        OpenAIClient = new OpenAIClient(apiKey, options);
    }
}

#if UNITY_EDITOR
#endif

namespace OpenAI.Settings
{
    public static class OpenAISettingsRuntime
    {
        public static string GetAPIKey()
        {
#if UNITY_EDITOR
            return OpenAIProjectSettings.GetOrCreateSettings().apiKey;
#else
            // Optional fallback: Env variable, cloud setting, etc.
            return System.Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "";
#endif
        }
    }
}
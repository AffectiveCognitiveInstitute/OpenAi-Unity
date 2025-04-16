#if UNITY_EDITOR
using UnityEditor;

static class OpenAISettingsProvider
{
    [SettingsProvider]
    public static SettingsProvider CreateOpenAISettingsProvider()
    {
        var provider = new SettingsProvider("Project/OpenAI", SettingsScope.Project)
        {
            label = "OpenAI",
            guiHandler = (searchContext) =>
            {
                var settings = OpenAIProjectSettings.GetOrCreateSettings();
                EditorGUI.BeginChangeCheck();
                settings.apiKey = EditorGUILayout.PasswordField("API Key", settings.apiKey);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(settings);
                }
            },

            keywords = new[] { "OpenAI", "API", "Key", "ChatGPT", "Whisper" }
        };

        return provider;
    }
}
#endif

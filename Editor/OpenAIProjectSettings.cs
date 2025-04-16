using UnityEngine;

public class OpenAIProjectSettings : ScriptableObject
{
    public string apiKey;

    private const string k_AssetPath = "ProjectSettings/OpenAIProjectSettings.asset";

    public static OpenAIProjectSettings GetOrCreateSettings()
    {
        var settings = Resources.Load<OpenAIProjectSettings>(k_AssetPath);
        if (settings == null)
        {
            settings = CreateInstance<OpenAIProjectSettings>();
            System.IO.Directory.CreateDirectory("ProjectSettings");
            UnityEditor.AssetDatabase.CreateAsset(settings, k_AssetPath);
            UnityEditor.AssetDatabase.SaveAssets();
        }
        return settings;
    }
}

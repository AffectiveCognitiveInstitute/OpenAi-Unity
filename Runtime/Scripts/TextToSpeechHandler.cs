using OpenAI.Audio;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class TextToSpeechHandler : OpenAIHandler
{
    public GeneratedSpeechVoice? SpeechVoice;
    public float? SpeedRatio;
    public string Model = "tts-1";
    private List<Task> _tasks = new List<Task>();
    private MainThreadDispatcher _threadDispatcher;

    private void Start()
    {
        _threadDispatcher = MainThreadDispatcher.Instance;
    }

    public void EnqueueSpeech(string userInput, GeneratedSpeechVoice? speechVoice, Action<AudioClip> action, AudioTranscriptionOptions options = null, CancellationToken cancellationToken = default)
    {
        Debug.LogWarning($"EnqueueSpeech");
        var task = Task.Run(async () =>
        {
            Debug.LogWarning($"Run speech...");
            try
            {
                var result = await GetSpeechAsync(userInput, speechVoice, cancellationToken);
                Debug.LogWarning($"Finished speech! {result}");
                var audioData = PCM16ToFloatArray(result.Span);

                _threadDispatcher.Enqueue(() =>
                {
                    Debug.LogWarning($"Create AudioClip");
                    var audioClip = CreateAudioClip(audioData, 24000, 1);
                    Debug.LogWarning($"Run callback");
                    action?.Invoke(audioClip);
                });
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        });
        _tasks.Add(task);
    }

    public async Task<ReadOnlyMemory<byte>> GetSpeechAsync(string userInput, GeneratedSpeechVoice? speechVoice, CancellationToken cancellationToken = default)
    {
        var result = await OpenAIClient.GetAudioClient(Model).GenerateSpeechAsync(userInput, speechVoice ?? SpeechVoice ?? GeneratedSpeechVoice.Alloy, new SpeechGenerationOptions() { ResponseFormat = GeneratedSpeechFormat.Pcm }, cancellationToken);
        return result.Value.ToMemory();
    }

    private float[] PCM16ToFloatArray(ReadOnlySpan<byte> pcmData)
    {
        int sampleCount = pcmData.Length / 2;
        float[] floatData = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            short sample = BitConverter.ToInt16(pcmData.Slice(i * 2, 2));
            floatData[i] = sample / 32768f; // normalisieren auf [-1.0, 1.0]
        }

        return floatData;
    }

    public static AudioClip CreateAudioClip(float[] samples, int sampleRate, int channels, string name = "PCMClip")
    {
        int frameCount = samples.Length / channels;
        var clip = AudioClip.Create(name, frameCount, channels, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}

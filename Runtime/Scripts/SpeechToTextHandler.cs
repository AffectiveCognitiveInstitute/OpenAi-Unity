using OpenAI.Audio;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class SpeechToTextHandler : OpenAIHandler
{
    public string Model = "whisper-1";
    public string Language;
    public string Prompt;
    public float? Temperature;
    public AudioTimestampGranularities TimestampGranularities = AudioTimestampGranularities.Default;
    private List<Task> _tasks = new List<Task>();
    private MainThreadDispatcher _threadDispatcher;

    private void Start()
    {
        _threadDispatcher = MainThreadDispatcher.Instance;
    }

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

    public void EnqueueTranscription(AudioClip audioClip, Action<string> action, AudioTranscriptionOptions options = null, CancellationToken cancellationToken = default)
    {
        Debug.LogWarning($"EnqueueTranscription");
        var audioBytes = WavHelper.GetWav(audioClip, out var _, true);
        var task = Task.Run(async () =>
        {
            Debug.LogWarning($"Run transcription...");
            try
            {
                var result = await TranscribeAsync(audioBytes, options, cancellationToken);
                Debug.LogWarning($"Finished transcription! {result}");
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

    public async Task<string> TranscribeAsync(AudioClip audioClip, AudioTranscriptionOptions options = null, CancellationToken cancellationToken = default)
    {
        var audioBytes = WavHelper.GetWav(audioClip, out var _, true);
        return await TranscribeAsync(audioBytes, options, cancellationToken);
    }

    //public async Task<string> TranscribeAsync(byte[] audioBytes, AudioTranscriptionOptions options = null, CancellationToken cancellationToken = default)
    //{
    //    using (var audioStream = new MemoryStream(audioBytes))
    //    {
    //        if (options == null)
    //        {
    //            options = new AudioTranscriptionOptions()
    //            {
    //                Language = Language,
    //                Prompt = Prompt,
    //                ResponseFormat = AudioTranscriptionFormat.Simple,
    //                Temperature = Temperature ?? 0.2f,
    //                TimestampGranularities = AudioTimestampGranularities.Default
    //            };
    //        }
    //        audioStream.Seek(0, SeekOrigin.Begin);
    //        Debug.Log($"Audio Stream: {audioStream.Length}");
    //        var result = await OpenAIClient.GetAudioClient(Model).TranscribeAudioAsync(audioStream, "audio.pcm", options, cancellationToken);
    //        return result.Value.Text;
    //    }
    //}

    public async Task<string> TranscribeAsync(byte[] audioBytes, AudioTranscriptionOptions options = null, CancellationToken cancellationToken = default)
    {
        if (options == null)
        {
            options = new AudioTranscriptionOptions()
            {
                Language = Language,
                Prompt = Prompt,
                ResponseFormat = AudioTranscriptionFormat.Simple,
                Temperature = Temperature ?? 0.2f,
                TimestampGranularities = AudioTimestampGranularities.Default
            };
        }

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);

        using var form = new MultipartFormDataContent();
        form.Add(new StringContent(Model), "model");
        form.Add(new ByteArrayContent(audioBytes), "file", "audio.wav");

        if (options.Language != null)
        {
            form.Add(new StringContent(options.Language), "language");
        }

        if (options.Prompt != null)
        {
            form.Add(new StringContent(options.Prompt), "prompt");
        }

        if (options.ResponseFormat.HasValue)
        {
            form.Add(new StringContent(options.ResponseFormat.ToString()), "response_format");
        }

        if (options.Temperature.HasValue)
        {
            form.Add(new StringContent(options.Temperature.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)), "temperature");
        }

        if (TimestampGranularities.HasFlag(AudioTimestampGranularities.Word))
        {
            form.Add(new StringContent("word"), "timestamp_granularities[]");
        }

        if (TimestampGranularities.HasFlag(AudioTimestampGranularities.Segment))
        {
            form.Add(new StringContent("segment"), "timestamp_granularities[]");
        }

        var response = await httpClient.PostAsync("https://api.openai.com/v1/audio/transcriptions", form);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Debug.LogError($"Whisper failed: {response.StatusCode} - {content}");
            return null;
        }
        var result = JsonSerializer.Deserialize<STTResponse>(content);
        return result.Text;
    }

    internal class STTResponse
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
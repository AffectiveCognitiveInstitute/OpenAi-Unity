using System.IO;
using UnityEngine;

public class MicrophoneHandler : MonoBehaviour
{
    private string _device;
    private AudioClip _audioClip;
    private int _sampleWindow = 128;
    private bool _isRecording = false;

    void OnEnable()
    {
        if (_device == null)
        {
            _device = Microphone.devices[0];
        }
    }

    /// <summary>
    /// Aktueller Peak aus dem Window
    /// </summary>
    public float LevelMax
    {
        get
        {
            var levelMax = 0f;
            var waveData = new float[_sampleWindow];
            var micPosition = Microphone.GetPosition(null) - (_sampleWindow + 1);
            if (micPosition < 0)
            {
                return 0;
            }
            _audioClip.GetData(waveData, micPosition);

            for (var i = 0; i < _sampleWindow; i++)
            {
                var wavePeak = waveData[i] * waveData[i];
                if (levelMax < wavePeak)
                {
                    levelMax = wavePeak;
                }
            }
            return levelMax;
        }
    }

    public bool IsRecording => _isRecording;

    public void StartRecording(int seconds)
    {
        if (!_isRecording)
        {
            _isRecording = true;
            _audioClip = Microphone.Start(_device, true, seconds, 44100);
        }
    }

    public AudioClip StopRecording()
    {
        if (_isRecording)
        {
            Microphone.End(_device);
            _isRecording = false;
            return _audioClip;
        }
        return null;
    }

    public Stream ToWavStream(AudioClip audioClip) => WavHelper.ToStream(audioClip, true);

}

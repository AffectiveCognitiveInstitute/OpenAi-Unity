using System.Threading.Tasks;
using UnityEngine;

public class AudioScaler : MonoBehaviour
{
    public MicrophoneHandler MicrophoneHandler;
    //public SpeechToTextHandler SpeechToTextHandler;
    //public TextToSpeechHandler TextToSpeechHandler;
    public AudioSource AudioSource;

    void Start()
    {

    }

    // Update is called once per frame
    private Task task;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 500f) && hit.transform == this.transform)
            {
                //var bytes = TextToSpeechHandler.GetResponse("Bitte mach mir eine MP3 aus dieser Eingabe", GeneratedSpeechVoice.Onyx, GeneratedSpeechFormat.Mp3, 1.0f);
                //var tmp = Path.GetTempFileName().Replace(".tmp", ".mp3");
                //File.WriteAllBytes(tmp, bytes.ToArray());
                //try
                //{
                //    var _webRequest = UnityWebRequestMultimedia.GetAudioClip(tmp, AudioType.MPEG);
                //    var _downloadHandler = (DownloadHandlerAudioClip)_webRequest.downloadHandler;
                //    _downloadHandler.streamAudio = true;
                //    _webRequest.SendWebRequest();
                //    while (!_webRequest.isDone)
                //    {

                //    }
                //    var audioClip = DownloadHandlerAudioClip.GetContent(_webRequest);
                //    audioClip.LoadAudioData();
                //    AudioSource.clip = audioClip;
                //    AudioSource.Play();
                //}
                //catch (Exception ex)
                //{
                //    Debug.LogException(ex);
                //}

                //if (!MicrophoneHandler.IsRecording)
                //{
                //    MicrophoneHandler.StartRecording(5);
                //}
                //else
                //{
                //    var recording = MicrophoneHandler.StopRecording();

                //    try
                //    {
                //        var task = SpeechToTextHandler.GetTextAsync(recording);
                //        while (!task.IsCompleted)
                //        {

                //        }
                //        var transcription = task.Result;
                //    }
                //    catch (Exception ex)
                //    {
                //        Debug.LogException(ex);
                //    }

                //    //var wavStream = MicrophoneHandler.ToWavStream(recording);
                //}
            }
        }

        var lm = Mathf.Max(0.1f, MicrophoneHandler.LevelMax);
        this.transform.localScale = Vector3.Slerp(this.transform.localScale, new Vector3(lm, 0.5f, lm) * 5, 0.05f);

        if (MicrophoneHandler.IsRecording)
        {
            GetComponent<Renderer>().material.color = new Color(0, 255, 0);
        }
        else
        {
            GetComponent<Renderer>().material.color = new Color(255, 255, 255);
        }
    }
}

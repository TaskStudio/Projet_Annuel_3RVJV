using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoTransition : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public AudioSource audioSource;
    public string nextSceneName;

    private void Start()
    {
        // Configure the VideoPlayer to use the AudioSource
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.SetTargetAudioSource(0, audioSource);

        // Play the video
        videoPlayer.Play();

        // Register callback for when the video ends
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        FindObjectOfType<FadeOutTransition>().StartFadeOut();
    }
}
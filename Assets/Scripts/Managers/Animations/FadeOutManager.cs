using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public AudioSource audioSource;
    public string nextSceneName;
    public Image fadeImage;
    public float fadeDuration = 1.0f;

    private void Start()
    {
        // Set the fade image to black and fully opaque
        fadeImage.color = Color.black;
        fadeImage.canvasRenderer.SetAlpha(1.0f);

        // Start the fade-out effect
        FadeOut();

        // Configure the VideoPlayer to use the AudioSource
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.SetTargetAudioSource(0, audioSource);

        // Play the video
        videoPlayer.Play();

        // Register callback for when the video ends
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    private void FadeOut()
    {
        fadeImage.CrossFadeAlpha(0, fadeDuration, false);
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        StartCoroutine(FadeAndLoadScene());
    }

    private IEnumerator FadeAndLoadScene()
    {
        // Fade to black
        fadeImage.CrossFadeAlpha(1, fadeDuration, false);
        yield return new WaitForSeconds(fadeDuration);

        // Load the next scene
        SceneManager.LoadScene(nextSceneName);
    }
}
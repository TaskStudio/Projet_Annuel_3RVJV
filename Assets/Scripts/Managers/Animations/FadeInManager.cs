using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeInManager : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1.0f;

    private void Start()
    {
        if (fadeImage != null)
        {
            // Set the fade image to black and fully opaque
            fadeImage.color = Color.black;
            fadeImage.canvasRenderer.SetAlpha(1.0f);

            // Start the fade-in effect
            StartCoroutine(FadeIn());
        }
    }

    private IEnumerator FadeIn()
    {
        fadeImage.CrossFadeAlpha(0, fadeDuration, false);
        yield return new WaitForSeconds(fadeDuration);

        // Optionally, you can disable the fade image after the fade-in completes
        fadeImage.gameObject.SetActive(false);
    }
}
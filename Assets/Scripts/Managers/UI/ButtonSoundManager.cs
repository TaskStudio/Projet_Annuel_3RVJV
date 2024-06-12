using UnityEngine;
using UnityEngine.UIElements;

public class ButtonSoundManager : MonoBehaviour
{
    public AudioSource hoverAudioSource;
    public AudioSource clickAudioSource;
    public AudioSource downClickAudioSource;

    private void OnEnable()
    {
        // Get the root VisualElement
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Find all buttons in the document
        var buttons = root.Query<Button>().ToList();

        // Add event listeners to each button
        foreach (var button in buttons)
        {
            button.RegisterCallback<MouseEnterEvent>(ev => OnButtonHover());
            button.RegisterCallback<ClickEvent>(ev => OnButtonClick());
            button.RegisterCallback<MouseDownEvent>(ev => OnButtonDown());
        }
    }

    private void OnButtonHover()
    {
        if (hoverAudioSource != null)
        {
            hoverAudioSource.Play();
        }
    }

    private void OnButtonClick()
    {
        if (clickAudioSource != null)
        {
            clickAudioSource.Play();
        }
    }
    
    private void OnButtonDown()
    {
        if (clickAudioSource != null)
        {
            downClickAudioSource.Play();
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main; // Get the main camera
    }

    private void Update()
    {
        // Make the progress bar face the camera
        if (mainCamera)
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                mainCamera.transform.rotation * Vector3.up);
        }
    }

    public void SetMaxValue(int maxValue)
    {
        slider.maxValue = maxValue;
        slider.value = maxValue;
        fill.color = gradient.Evaluate(1f);
    }

    public void SetValue(int value)
    {
        slider.value = value;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
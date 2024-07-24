using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        gameObject.SetActive(false); // Disable HealthBar by default
    }

    private void Update()
    {
        // Face the camera
        if (mainCamera)
            transform.LookAt(
                transform.position + mainCamera.transform.rotation * Vector3.forward,
                mainCamera.transform.rotation * Vector3.up
            );
    }

    public void Initialize(int hp)
    {
        SetMaxValue(hp);
        SetValue(hp);
    }

    private void SetMaxValue(int maxValue)
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
    
    public int GetCurrentHealth()
    {
        return (int)slider.value;
    }

    public int GetMaxHealth()
    {
        return (int)slider.maxValue;
    }


    public void SetVisibility(bool visible)
    {
        gameObject.SetActive(visible);
    }
}
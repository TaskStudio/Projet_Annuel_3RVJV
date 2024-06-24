using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    private Camera mainCamera;
    private IDamageable damageableEntity;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // Face the camera
        if (mainCamera)
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                mainCamera.transform.rotation * Vector3.up);
        }
        
        if (damageableEntity != null)
        {
            SetValue(damageableEntity.GetHealthPoints());
        }
    }

    public void Initialize(IDamageable damageable)
    {
        damageableEntity = damageable;
        SetMaxValue(damageable.GetMaxHealthPoints());
        SetValue(damageable.GetHealthPoints()); 
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
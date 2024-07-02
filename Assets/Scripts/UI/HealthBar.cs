using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    private Camera mainCamera;
    public Entity damageableEntity;

    private void Start()
    {
        mainCamera = Camera.main;
        Initialize(damageableEntity);
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
            // Assuming GetHealthPoints() is an extension method or part of some system
            SetValue(damageableEntity.GetHealthPoints());
        }
    }

    public void Initialize(Entity myEntity)
    {
        damageableEntity = myEntity;
        SetMaxValue(damageableEntity.GetMaxHealthPoints());
        SetValue(damageableEntity.GetHealthPoints()); 
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
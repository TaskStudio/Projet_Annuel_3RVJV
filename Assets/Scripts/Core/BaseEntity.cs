using UnityEngine;

public class BaseEntity : MonoBehaviour, ISelectable
{
    public EntityProfile profile;  // Assign the profile in the inspector
    private EntityVisuals visuals;

    private ProgressBar progressBarInstance;
    private int currentHealth;

    public bool IsSelected { get; set; }

    private void Start()
    {
        InitializeProgress();
        FindProgressBar();
    }

    private void InitializeProgress()
    {
        if (profile != null)
        {
            profile.Initialize();
            currentHealth = profile.CurrentValue;
        }
    }

    private void FindProgressBar()
    {
        // Find the ProgressBar component in the child GameObject
        progressBarInstance = GetComponentInChildren<ProgressBar>();
        if (progressBarInstance != null)
        {
            progressBarInstance.SetMaxValue(profile.MaxValue);
        }
        else
        {
            Debug.LogWarning("ProgressBar component not found in children.");
        }
    }

    public void Select()
    {
        IsSelected = true;
        UpdateVisuals();
    }

    public void Deselect()
    {
        IsSelected = false;
        UpdateVisuals();
    }

    public IProfile GetProfile()
    {
        return profile;
    }

    public void UpdateVisuals()
    {
        if (visuals)
        {
            visuals.UpdateVisuals(IsSelected);
        }
    }

    public int Health => currentHealth;

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        if (progressBarInstance)
        {
            progressBarInstance.SetValue(currentHealth);
        }
    }

    public int MaxValue => profile.MaxValue;

    public int CurrentValue
    {
        get => currentHealth;
        set
        {
            currentHealth = value;
            if (progressBarInstance != null)
            {
                progressBarInstance.SetValue(currentHealth);
            }
        }
    }

    public void UpdateProgress(int value)
    {
        CurrentValue = value;
    }
}

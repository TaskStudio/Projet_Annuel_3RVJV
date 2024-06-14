using UnityEngine;

public class BaseEntity : MonoBehaviour, IDamageable
{
    public Profile profile; // Assign the profile in the inspector

    private ProgressBar progressBarInstance;

    public int CurrentValue
    {
        get => Health;
        set
        {
            Health = value;
            if (progressBarInstance != null) progressBarInstance.SetValue(Health);
        }
    }


    private void Start()
    {
        InitializeProgress();
        FindProgressBar();
    }

    public int Health { get; set; }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health < 0) Health = 0;
        if (progressBarInstance) progressBarInstance.SetValue(Health);
    }

    public Profile GetProfile()
    {
        return profile;
    }

    private void InitializeProgress()
    {
        if (profile != null)
        {
            profile.Initialize();
            Health = profile.CurrentValue;
        }
    }

    private void FindProgressBar()
    {
        // Find the ProgressBar component in the child GameObject
        progressBarInstance = GetComponentInChildren<ProgressBar>();
        if (progressBarInstance != null)
            progressBarInstance.SetMaxValue(profile.MaxValue);
        else
            Debug.LogWarning("ProgressBar component not found in children.");
    }

    public void UpdateProgress(int value)
    {
        CurrentValue = value;
    }
}
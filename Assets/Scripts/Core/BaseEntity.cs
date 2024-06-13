using UnityEngine;

public class BaseEntity : MonoBehaviour, ISelectable
{
    public EntityProfile profile; // Assign the profile in the inspector
    private EntityVisuals visuals;

    public bool IsSelected { get; set; }

    public void Select()
    {
        UpdateVisuals();
    }

    public void Deselect()
    {
        UpdateVisuals();
    }

    public IProfile GetProfile()
    {
        return profile;
    }

    public void UpdateVisuals()
    {
        if (visuals) visuals.UpdateVisuals(IsSelected);
    }
}
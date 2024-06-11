using UnityEngine;

public class BaseEntity : MonoBehaviour, ISelectable
{
    public EntityProfile profile;
    private EntityVisuals visuals;

    public bool IsSelected { get; set; }

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
}


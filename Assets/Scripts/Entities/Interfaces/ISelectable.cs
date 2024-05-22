public interface ISelectable
{
    bool IsSelected { get; set; }
    void Select();
    void Deselect();
    
    void UpdateVisuals();
}
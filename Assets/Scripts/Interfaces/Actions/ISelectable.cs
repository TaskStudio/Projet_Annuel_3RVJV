public interface ISelectable
{
    bool IsSelected { get; set; }
    void Select();
    void Deselect();
    Profile GetProfile(); // Renvoie un profil générique
}
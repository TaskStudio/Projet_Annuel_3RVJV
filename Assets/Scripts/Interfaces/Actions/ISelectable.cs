public interface ISelectable
{
    bool IsSelected { get; set; }
    void Select();
    void Deselect();
    IProfile GetProfile(); // Renvoie un profil générique
}
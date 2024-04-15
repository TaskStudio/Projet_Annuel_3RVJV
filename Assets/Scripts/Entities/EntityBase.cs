using UnityEngine;

public class EntityBase : MonoBehaviour
{
    private int hp = 1000;
    public GameObject losePanel;  // Assign this in the inspector to your LosePanel

    void Start()
    {
        if (losePanel != null)
            losePanel.SetActive(false);
        else
            Debug.LogError("LosePanel not set in the inspector.");
        
        Debug.Log("EntityBase Start method called. LosePanel active: " + losePanel.activeSelf);
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("EntityBase taking damage.");
        hp -= damage;
        if (hp <= 0)
        {
            Debug.Log("EntityBase destroyed. Activating LosePanel.");
            if (losePanel != null)
            {
                losePanel.SetActive(true);  // This will show the LosePanel with the message
            }
            else
            {
                Debug.LogError("LosePanel not set in the inspector.");
            }
        }
    }
}

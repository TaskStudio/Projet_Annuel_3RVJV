using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    private int hp = 1000;
    public GameObject winPanel;  // Assign this in the inspector to your WinPanel

    void Start()
    {
        if (winPanel != null)
            winPanel.SetActive(false);
        else
            Debug.LogError("WinPanel not set in the inspector.");

        Debug.Log("EnemyBase Start method called. WinPanel active: " + winPanel.activeSelf);
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("EnemyBase taking damage.");
        hp -= damage;
        if (hp <= 0)
        {
            Debug.Log("EnemyBase destroyed. Activating WinPanel.");
            if (winPanel != null)
            {
                winPanel.SetActive(true);  // This will show the WinPanel with the message
            }
            else
            {
                Debug.LogError("WinPanel not set in the inspector.");
            }
        }
    }
}
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    private int hp = 1000;
    public GameObject winPanel; 

    void Start()
    {
        winPanel.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        { 
            Time.timeScale = 0f;
            winPanel.SetActive(true); 
        }
    }
}
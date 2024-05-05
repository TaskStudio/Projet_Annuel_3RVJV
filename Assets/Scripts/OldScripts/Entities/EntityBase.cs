using UnityEngine;

public class EntityBase : MonoBehaviour
{
    private int hp = 1000;
    public GameObject losePanel; 

    void Start()
    {
        losePanel.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Time.timeScale = 0f;
            losePanel.SetActive(true);
        }
    }
}

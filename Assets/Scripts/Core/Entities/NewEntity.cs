using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NewEntity : BaseObject
{
    [Space(10)] [Header("Actions")]
    public List<UnityEvent> actionList;

    public int hp { get; set; }
    public int maxHp { get; }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp < 0)
        {
            hp = 0;
            gameObject.SetActive(false);
        }
    }
}
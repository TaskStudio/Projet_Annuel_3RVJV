using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
///     Class <c>Entity</c> represents a Base Object that can be destroyed/killed
/// </summary>
public class Entity : BaseObject
{
    [Space(10)] [Header("Actions")]
    public List<UnityEvent> actionList;

    public int hp;
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
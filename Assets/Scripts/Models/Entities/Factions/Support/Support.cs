using System.Linq;
using UnityEngine;

public class Support : Fighter, IAlly
{
    public int healAmount = 10;
    private bool currentTargetIsInRange;

    private float healTimer;
    private float manaRegenTimer;

    protected new void Start()
    {
        base.Start();
        healTimer = data.attackCooldown;
    }

    protected new void Update()
    {
        base.Update();

        if (currentTarget != null
            && Vector3.Distance(transform.position, currentTarget.transform.position) > data.attackRange)
        {
            currentTargetIsInRange = false;
            targetPosition = currentTarget.transform.position;
        }
        else
        {
            currentTargetIsInRange = true;
            Stop();
        }

        healTimer -= Time.deltaTime;
        if (targetsInRange.Count > 0 && healTimer <= 0f)
        {
            HealNearbyEntities();
            healTimer = data.attackCooldown;
        }

        if (currentMana < data.maxManaPoints)
        {
            manaRegenTimer -= Time.deltaTime;
            if (manaRegenTimer <= 0f)
            {
                currentMana++;
                manaRegenTimer = 1f;
            }
        }
    }

    public override void SetTarget(IBaseObject target)
    {
        if (target == null) return;
        if (target is IAlly) currentTarget = target as IEntity;
    }

    public override void AddTargetInRange(IEntity target)
    {
        if (target is IAlly)
        {
            targetsInRange.Add(target);
            target.AddTargetedBy(this);
        }
    }

    public override void RemoveTargetInRange(IEntity target)
    {
        if (target is IAlly)
        {
            targetsInRange.Remove(target);
            target.RemoveTargetedBy(this);
        }
    }

    private void HealNearbyEntities()
    {
        if (currentTarget == null || currentTarget.GetHealthPoints() == currentTarget.GetMaxHealthPoints())
        {
            var sortedAlliesPerMissingHealth = targetsInRange.FindAll(
                    ally => ally.GetHealthPoints() < ally.GetMaxHealthPoints()
                )
                .OrderBy(ally => ally.GetMissingHealthPercentage());

            currentTarget = sortedAlliesPerMissingHealth.FirstOrDefault();
        }

        if (currentTarget != null && currentTargetIsInRange)
        {
            currentTarget.SetHealthPoints(currentTarget.GetHealthPoints() + healAmount);
            currentMana -= healAmount;
        }
    }
}
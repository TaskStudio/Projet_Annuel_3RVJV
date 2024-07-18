using System.Linq;
using UnityEngine;

public class Support : Fighter, IAlly
{
    [Space(10)] [Header("Heal")] [SerializeField]
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

        targetsInRange = Physics.OverlapSphere(transform.position, data.detectionRange)
            .Select(c => c.GetComponent<IEntity>())
            .Where(e => e is IAlly)
            .ToList();

        if (currentTarget != null)
            if (Vector3.Distance(transform.position, currentTarget.transform.position) > data.attackRange)
            {
                currentTargetIsInRange = false;
                targetPosition = currentTarget.transform.position;
            }
            else
            {
                currentTargetIsInRange = true;
                Stop();
            }
        else targetPosition = heldPosition;

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
                SetManaPoints(currentMana + 1);
                manaRegenTimer = 1f;
            }
        }
    }

    public override void SetTarget(IBaseObject target)
    {
        if (target == null) return;
        if (target is IAlly) currentTarget = target as IEntity;
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

        if (currentMana >= healAmount && currentTarget != null && currentTargetIsInRange)
        {
            currentTarget.SetHealthPoints(currentTarget.GetHealthPoints() + healAmount);
            SetManaPoints(currentMana - healAmount);
        }
    }
}
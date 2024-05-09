using UnityEngine;

public class NonEnemy : MonoBehaviour, IMovable, IShootable, ISelectable
{
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public GameObject selectionIndicatorPrefab;
    public float moveSpeed = 5f;
    public float stoppingDistance = 0.5f;
    public LayerMask Entity; // LayerMask for other entities to avoid collisions
    public float collisionRadius = 0.5f;
    public float avoidanceStrength = 0.1f;
    public bool IsSelected { get; set; }

    private EntityVisuals visuals;
    private Vector3 targetPosition;
    private Collider entityCollider;

    void Start()
    {
        EntitiesManager.Instance.RegisterMovableEntity(this);
        visuals = GetComponent<EntityVisuals>();
        entityCollider = GetComponent<Collider>();
        if (visuals == null)
        {
            visuals = gameObject.AddComponent<EntityVisuals>();
        }
        visuals.selectionIndicatorPrefab = selectionIndicatorPrefab;
        targetPosition = transform.position;
    }

    void Update()
    {
        HandleInput();
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            Vector3 adjustedPosition = AvoidCollisions();
            MoveTowardsTarget(adjustedPosition);
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Z) && IsSelected)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            Shoot(mousePosition);
        }
    }

    private Vector3 AvoidCollisions()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, collisionRadius, Entity);
        Vector3 avoidanceVector = Vector3.zero;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider != entityCollider)
            {
                Vector3 collisionDirection = transform.position - hitCollider.transform.position;
                avoidanceVector += collisionDirection.normalized;
            }
        }

        if (avoidanceVector != Vector3.zero)
        {
            avoidanceVector = avoidanceVector.normalized * avoidanceStrength;
        }

        return targetPosition + avoidanceVector; // Calculate a new temporary target position that includes avoidance
    }

    public void Move(Vector3 newPosition)
    {
        targetPosition = newPosition;
    }

    private void MoveTowardsTarget(Vector3 adjustedPosition)
    {
        Vector3 moveDirection = (adjustedPosition - transform.position).normalized;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, 1, transform.position.z);
    }

    public void Shoot(Vector3 target)
    {
        if (projectilePrefab && projectileSpawnPoint)
        {
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
            projectile.GetComponent<Rigidbody>().velocity = (target - transform.position).normalized * 20f;
        }
    }

    public void Select()
    {
        IsSelected = true;
        UpdateVisuals();
    }

    public void Deselect()
    {
        IsSelected = false;
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        if (visuals != null)
        {
            visuals.UpdateVisuals(IsSelected);
        }
    }
}

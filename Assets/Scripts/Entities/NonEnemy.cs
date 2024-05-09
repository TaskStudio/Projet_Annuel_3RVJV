using UnityEngine;
using System.Collections;

public class NonEnemy : MonoBehaviour, IMovable, IShootable, ISelectable
{
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public GameObject selectionIndicatorPrefab; 
    public float moveSpeed = 5f;
    public float stoppingDistance = 0.5f;
    public bool IsSelected { get; set; }
    private EntityVisuals visuals; 
    private Vector3 targetPosition;

    void Start()
    {
        EntitiesManager.Instance.RegisterMovableEntity(this);
        visuals = GetComponent<EntityVisuals>();
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
        MoveTowardsTarget();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Z) && IsSelected)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            Shoot(mousePosition);
        }
    }

    public void Move(Vector3 newPosition)
    {
        targetPosition = newPosition; 
    }

    private void MoveTowardsTarget()
    {
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            Vector3 movePosition = new Vector3(targetPosition.x, 1, targetPosition.z);
            
            Vector3 moveDirection = (movePosition - transform.position).normalized;
            
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
            
            transform.position = new Vector3(transform.position.x, 1, transform.position.z);
        }
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

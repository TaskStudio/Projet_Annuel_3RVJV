using UnityEngine;

public class NonEnemy : MonoBehaviour, IMovable, IShootable, ISelectable
{
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public GameObject selectionIndicatorPrefab; 
    public float moveSpeed = 5f;
    public bool IsSelected { get; set; }
    private EntityVisuals visuals; 

    void Start()
    {
        visuals = GetComponent<EntityVisuals>();
        if (visuals == null)
        {
            visuals = gameObject.AddComponent<EntityVisuals>();
        }
        visuals.selectionIndicatorPrefab = selectionIndicatorPrefab; 
    }

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Z) && IsSelected)
        {
            Vector3 targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            Shoot(targetPosition);
        }

        if (Input.GetKey(KeyCode.UpArrow) && IsSelected)
        {
            Move(transform.position + transform.forward * moveSpeed * Time.deltaTime);
        }
    }

    public void Move(Vector3 targetPosition)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
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
        Debug.Log($"Selected: {gameObject.name}");
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

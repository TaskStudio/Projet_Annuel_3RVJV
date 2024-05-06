using UnityEngine;

public class NonEnemy : Entity, IMovable, IShootable
{
    public GameObject projectilePrefab; 
    public Transform projectileSpawnPoint;
    public float moveSpeed = 5f;  // Default move speed


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && IsSelected())
        {
            Shoot(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    public void Shoot(Vector3 target)
    {
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        Vector3 direction = (target - projectileSpawnPoint.position).normalized;
        projectile.GetComponent<Rigidbody>().velocity = direction * 20f; // Set speed of projectile
    }

    public void Move(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private bool IsSelected()
    {
        return SelectionManager.Instance.IsEntitySelected(this.gameObject);
    }

}
using UnityEngine;

public class Entityy : MonoBehaviour
{
    public int hp = 100;
    public GameObject projectilePrefab; 
    public Transform projectileSpawnPoint;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && IsEntitySelected())
        {
            ShootProjectile();
        }
    }

    private bool IsEntitySelected()
    {
        // Vous avez besoin d'accéder au script EntitiesController pour vérifier la sélection
        EntitiesController entitiesController = FindObjectOfType<EntitiesController>();
        if (entitiesController != null)
        {
            return entitiesController.IsEntitySelected(gameObject);
        }
        return false;
    }

    
    private void ShootProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity); 
        // Diriger le projectile vers la position de la souris
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f))
        {
            Vector3 targetPoint = hit.point;
            targetPoint.y = projectileSpawnPoint.position.y; 
            Vector3 direction = targetPoint - projectile.transform.position;
            projectile.transform.forward = direction.normalized; 
            projectile.GetComponent<Projectile>().damage = 100; 
        }
    }


    protected virtual void Awake()
    {
        if (EntityManager.Instance != null)
        {
            EntityManager.Instance.RegisterEntity(this);
        }
    }


    protected virtual void OnDestroy()
    {
        EntitiesController controller = FindObjectOfType<EntitiesController>();
        if (controller != null)
        {
            controller.DeregisterEntity(this.gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }

}

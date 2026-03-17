using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class ProjectileController : MonoBehaviour
{
    public ProjectileScriptableObject projectileData;
    
    [Header("Runtime Variables")]
    public Transform targetPosition;
    public float timer;
    public bool isAllyProjectile;
    private string tagToCompare => isAllyProjectile ? "Enemy" : "Ally";

    private IObjectPool<ProjectileController> pool;
    void Start()
    {
        Initialize(projectileData);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= projectileData.lifeTime) ReturnProjectile();
        if (projectileData.isTurretProjectile) MoveForward();
        else MoveToTarget();
    }

    public void Initialize(ProjectileScriptableObject data)
    {
        projectileData = data;
    }

    void MoveForward()
    {
        transform.position += transform.right * projectileData.projectileSpeed * Time.deltaTime;
    }

    void MoveToTarget()
    {
        if (targetPosition == null || !targetPosition.gameObject.activeInHierarchy)
        {
            // If the target is gone, just fly straight or disappear
            MoveForward();
            return;
        }
        
        transform.position = Vector2.MoveTowards(transform.position, targetPosition.position, projectileData.projectileSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(tagToCompare))
        {
            other.GetComponent<UnitController>().TakeDamage(projectileData.projectileDamage);
            ReturnProjectile();
        }
    }

    public void ResetProjectile(Vector2 position, Quaternion direction, bool fromAlly, Transform targetPosition)
    {
        timer = 0f;
        transform.position = position;
        transform.rotation = direction;
        isAllyProjectile = fromAlly;
        this.targetPosition = targetPosition;
    }

    public void ResetProjectile(Vector2 position, Quaternion direction, bool fromAlly)
    {
        timer = 0f;
        transform.position = position;
        transform.rotation = direction;
        isAllyProjectile = fromAlly;
    }

    public void SetPool(IObjectPool<ProjectileController> pool)
    {
        this.pool = pool;
    }

    void ReturnProjectile()
    {
        pool.Release(this);
    }
}

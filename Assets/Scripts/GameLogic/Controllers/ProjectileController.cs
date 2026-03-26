using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Controller class for projectile behavior.
/// Contains methods for moving the projectile and returning it to the pool.
/// </summary>
public class ProjectileController : MonoBehaviour
{
    public ProjectileScriptableObject projectileData;
    
    public GameObject _target;
    private Vector2 _direction;
    private bool _isAllyProjectile;
    private IObjectPool<ProjectileController> _pool;
    private float _timer;

    /// <summary>
    /// Projectiles can either be initialized with a target or a direction.
    ///  
    /// If the projectile is initialized with a target, it will move towards the target.
    /// If the target initialized is no longer active, it changes to the closest active enemy.
    /// If there is none, it returns to the pool.
    /// 
    /// If the projectile is initialized with a direction, it will move in that direction.
    /// </summary>
    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= projectileData.lifeTime) ReturnProjectile();

        if (_target != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, _target.transform.position, projectileData.projectileSpeed * Time.deltaTime);
            Vector2 dir = (_target.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            if (_target.activeSelf == false)
            {
                if (!DetectClosestEnemy())
                {
                    _target = null;
                    ReturnProjectile();
                    return;
                }
            }
        }
        else
        {
            transform.Translate(_direction * projectileData.projectileSpeed * Time.deltaTime, Space.World);
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    bool DetectClosestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 3f, LayerMask.GetMask("Units"));
        string targetTag = _isAllyProjectile ? "Enemy" : "Ally";

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject.CompareTag(targetTag))
            {
                _target = hit.transform.gameObject;
                return true;
            }
        }
        return false;
    }

    public void Initialize(GameObject target, Vector2 direction, bool fromAlly, ProjectileScriptableObject data)
    {
        _timer = 0f;
        _target = target;
        _direction = direction;
        _isAllyProjectile = fromAlly;
        projectileData = data;
    }

    /// <summary>
    /// Triggers when the projectile collides with something.
    /// 
    /// Calls ApplyDamage, which checks the projectileData if the projectile is an AOE.
    /// Switches logic based on that value. 
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        string targetTag = _isAllyProjectile ? "Enemy" : "Ally";
        if (other.CompareTag(targetTag))
        {
            ApplyDamage(other.gameObject);
        }
        
        if (other.CompareTag("Floor")) ReturnProjectile();
    }

    void ApplyDamage(GameObject hitObject)
    {
        if (projectileData.isAOE)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, projectileData.aoeRadius, LayerMask.GetMask("Units"));
            foreach (Collider2D h in hits)
            {
                if (h.CompareTag(_isAllyProjectile ? "Enemy" : "Ally"))
                {
                    h.GetComponent<IDamageable>()?.TakeDamage(projectileData.projectileDamage);
                }
            }
        }
        else
        {
            hitObject.GetComponent<IDamageable>()?.TakeDamage(projectileData.projectileDamage);
        }

        ReturnProjectile();
    }

    public void SetPool(IObjectPool<ProjectileController> pool) => _pool = pool;
    private void ReturnProjectile()
    {
        if(gameObject.activeSelf) _pool.Release(this);
    }
}
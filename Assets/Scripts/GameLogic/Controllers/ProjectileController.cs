using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class ProjectileController : MonoBehaviour
{
    public ProjectileScriptableObject projectileData;
    
    public GameObject _target;
    private Vector2 _direction;
    private bool _isAllyProjectile;
    private IObjectPool<ProjectileController> _pool;
    private float _timer;

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= projectileData.lifeTime) ReturnProjectile();

        if (_target != null)
        {
            // HOMING LOGIC: Move toward the transform
            transform.position = Vector2.MoveTowards(transform.position, _target.transform.position, projectileData.projectileSpeed * Time.deltaTime);
            
            // Rotate to face the moving target
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
            // DOWNWARD LOGIC: Move straight down
            transform.Translate(_direction * projectileData.projectileSpeed * Time.deltaTime, Space.World);
            
            // Rotate to face the moving direction
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

    void OnTriggerEnter2D(Collider2D other)
    {
        string targetTag = _isAllyProjectile ? "Enemy" : "Ally";
        if (other.CompareTag(targetTag))
        {
            ApplyDirectDamage(other.gameObject);
        }
        
        if (other.CompareTag("Floor")) ReturnProjectile();
    }

    void ApplyDirectDamage(GameObject hitObject)
    {
        if (projectileData.isAOE)
        {
            // AOE Logic
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
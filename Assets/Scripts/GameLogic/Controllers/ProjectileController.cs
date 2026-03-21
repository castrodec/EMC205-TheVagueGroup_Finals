using UnityEngine;
using UnityEngine.Pool;

public class ProjectileController : MonoBehaviour
{
    public ProjectileScriptableObject projectileData;
    
    private Transform _target;
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
            transform.position = Vector2.MoveTowards(transform.position, _target.position, projectileData.projectileSpeed * Time.deltaTime);
            
            // Rotate to face the moving target
            Vector2 dir = (_target.position - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            if (_target.gameObject.activeSelf == false) ReturnProjectile();
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

    public void Initialize(Transform target, Vector2 direction, bool fromAlly, ProjectileScriptableObject data)
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
            DamageEnemy(projectileData.isAOE);
        }
        
        if (other.CompareTag("Floor")) ReturnProjectile();
    }

    void DamageEnemy(bool isAOE)
    {
        if (!isAOE && _target != null)
        {
            _target.GetComponent<IDamageable>()?.TakeDamage(projectileData.projectileDamage);
            ReturnProjectile();
        } 
        else
        {
            foreach (Collider2D hit in Physics2D.OverlapCircleAll(transform.position, projectileData.aoeRadius, projectileData.targetLayer))
            {
                hit.GetComponent<IDamageable>()?.TakeDamage(projectileData.projectileDamage);
            }
            ReturnProjectile();
        }
    }

    public void SetPool(IObjectPool<ProjectileController> pool) => _pool = pool;
    private void ReturnProjectile() => _pool?.Release(this);
}
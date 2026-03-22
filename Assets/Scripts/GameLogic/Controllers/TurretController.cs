using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class TurretController : MonoBehaviour, IDamageable
{
    public TurretScriptableObject turretData;
    [SerializeField] private Transform firePoint;
    public int currentHealth;
    public GameObject target;
    private IObjectPool<TurretController> _pool;
    [HideInInspector] public IState idleState, shootingState;
    private IState currentState;

    void Awake()
    {
        idleState = new IdleState(this);
        shootingState = new ShootingState(this);
    }

    void Start() => ChangeState(idleState);

    void Update() => currentState?.Tick();

    public void ChangeState(IState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    public bool DetectTarget()
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(firePoint.position, turretData.detectionRange, turretData.targetLayer);

        foreach (Collider2D h in hit)
        {
            if (h.gameObject.CompareTag("Enemy"))
            {
                target = h.gameObject;
                return true;
            }
        }

        return false;
    }

    public void Explode()
    {
        foreach (Collider2D hit in Physics2D.OverlapCircleAll(transform.position, turretData.aoeRadius, turretData.targetLayer))
        {
            hit.GetComponent<IDamageable>()?.TakeDamage(turretData.damage);
            Die();
        }
    }

    public void ResetTurret(Vector2 position, TurretScriptableObject data)
    {
        currentHealth = data.maxHealth;
        turretData = data;
        transform.position = position;
        ChangeState(idleState);
    }

    public void ShootDirection(Vector2 direction) => ObjectPooler.Instance.SpawnProjectile(turretData.projectileData, firePoint.position, null, direction, true);

    public void ShootTarget(GameObject target) => ObjectPooler.Instance.SpawnProjectile(turretData.projectileData, firePoint.position, target, Vector2.zero, true);

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;
        currentHealth -= damage;
        if (currentHealth <= 0) Die();
    }

    public void Die()
    {
        _pool?.Release(this);
    }

    public void SetPool(IObjectPool<TurretController> pool) => _pool = pool;

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, turretData.detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, turretData.aoeRadius);
        Gizmos.color = Color.red;
        if (target == null) return;
        Gizmos.DrawLine(transform.position, target.transform.position);
    }
}

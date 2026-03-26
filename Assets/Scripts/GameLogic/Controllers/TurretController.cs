using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Controller class for turret behavior.
/// Contains methods for detecting targets and firing projectiles, as well as the state machine that handles
/// the turret's current state.
/// </summary>
public class TurretController : MonoBehaviour, IDamageable
{
    [Header("Data & References")]
    public TurretScriptableObject turretData;
    [SerializeField] private Transform firePoint;
    
    [Header("Status")]
    public int currentHealth;
    public GameObject target;
    
    private IObjectPool<TurretController> _pool;
    private IState currentState;
    
    [HideInInspector] public IState idleState, shootingState;

    private void Awake()
    {
        idleState = new IdleState(this);
        shootingState = new ShootingState(this);
    }

    private void Start() => ChangeState(idleState);

    private void Update() => currentState?.Tick();

    public void ChangeState(IState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    /// <summary>
    /// Detects a target within the turret's detection range, declared within the turretData.
    /// </summary>
    /// <returns> Returns true when there is an enemy in range, false otherwise. </returns>
    public bool DetectTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(firePoint.position, turretData.detectionRange, turretData.targetLayer);

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject.CompareTag("Enemy"))
            {
                target = hit.gameObject;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Explodes the turret, dealing damage to all units in range.
    /// </summary>
    public void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, turretData.aoeRadius, turretData.targetLayer);
        foreach (Collider2D hit in hits)
        {
            hit.GetComponent<IDamageable>()?.TakeDamage(turretData.damage);
        }
        Die();
    }


    public void ShootDirection(Vector2 direction) => 
        ObjectPooler.Instance.SpawnProjectile(turretData.projectileData, firePoint.position, null, direction, true);

    public void ShootTarget(GameObject target) => 
        ObjectPooler.Instance.SpawnProjectile(turretData.projectileData, firePoint.position, target, Vector2.zero, true);

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;
        currentHealth -= damage;
        if (currentHealth <= 0) Die();
    }

    public void Die() => _pool?.Release(this);

    public void SetPool(IObjectPool<TurretController> pool) => _pool = pool;

    public void ResetTurret(Vector2 position, TurretScriptableObject data)
    {
        currentHealth = data.maxHealth;
        turretData = data;
        transform.position = position;
        ChangeState(idleState);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, turretData.detectionRange);
    }
}
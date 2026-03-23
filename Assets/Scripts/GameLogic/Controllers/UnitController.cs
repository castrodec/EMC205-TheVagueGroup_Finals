using UnityEngine;
using UnityEngine.Pool;

public class UnitController : MonoBehaviour, IDamageable
{
    public UnitScriptableObject unitData;
    [HideInInspector] public IState currentState, marchingState, attackingState, flyingState, reloadingState;

    public int currentHealth, currentAmmo;
    public GameObject target, allyInFront;
    public LayerMask unitLayer, turretLayer;
    public bool isAlly;
    private IObjectPool<UnitController> _pool;

    private void Awake() // Use Awake for state initialization
    {
        marchingState = new MarchingState(this);
        attackingState = new AttackingState(this);
        flyingState = new FlyingState(this);
        reloadingState = new ReloadingState(this);
    }

    private void Update() => currentState?.Tick();

    public void ChangeState(IState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    // --- CORE LOGIC METHODS ---

    public void HandleBasicMovement()
    {
        transform.Translate(Vector2.right * unitData.moveSpeed * Time.deltaTime);
    }

    public bool DetectEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, unitData.attackRange, unitLayer);
        string targetTag = isAlly ? "Enemy" : "Ally";

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject.CompareTag(targetTag))
            {
                target = hit.transform.gameObject;
                return true;
            }
        }

        return false;
    }

    public bool DetectTurret()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, unitData.attackRange, turretLayer);

        if (!isAlly)
        {
            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject.GetComponent<TurretController>().turretData.turretType != TurretScriptableObject.TurretType.Trap)
                {
                    target = hit.transform.gameObject;
                    return true;
                }
            }
        }
        
        return false;
    }

    public bool DetectAllyInFront()
    {
        Vector2 direction = isAlly ? Vector2.right : Vector2.left;
        float distance = 1f;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, unitLayer);

        if (hit && hit.collider.gameObject.GetComponent<UnitController>().unitData.attackType == unitData.attackType
        && hit.collider.CompareTag(isAlly ? "Ally" : "Enemy"))
        {
            allyInFront = hit.transform.gameObject;
            return true;
        }

        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAlly) return;
        if (collision.gameObject.CompareTag("Base"))
        {
            DamagePlayer(1);
            Die();
        }
    }

    public void DamagePlayer(int damage)
    {
        if (isAlly) return;
        GameManager.Instance.TakeDamage(damage);
        Die();
    }

    public void ShootDirection(Vector2 direction) => ObjectPooler.Instance.SpawnProjectile(unitData.projectileData, transform.position, null, direction, isAlly);

    public void ShootTarget(GameObject target) => ObjectPooler.Instance.SpawnProjectile(unitData.projectileData, transform.position, target, Vector2.zero, isAlly);

    public void TakeDamage(int damage) 
    {
        if (currentHealth <= 0) return;
        currentHealth -= damage;
        if (currentHealth <= 0) Die();
    }
    public void Die()
    {
        _pool.Release(this);
        if (!isAlly) {
            ResourceManager.Instance.AddCoins(Mathf.RoundToInt(unitData.unitCost * 1.5f));
            WaveManager.Instance.EnemyDied();
        }
    }

    public void ResetUnit(Vector2 pos, Quaternion rot, bool ally, UnitScriptableObject data)
    {
        target = null;
        currentHealth = unitData.maxHealth;
        currentAmmo = unitData.ammoCapacity;
        transform.position = pos;
        transform.rotation = rot;
        isAlly = ally;
        gameObject.tag = isAlly ? "Ally" : "Enemy";
        unitData = data;
        //Color color = ally ? Color.green : Color.red;
        //GetComponent<SpriteRenderer>().color = color;
        ChangeState(data.attackType.Equals(UnitScriptableObject.AttackType.Flying) ? flyingState : marchingState);
    }

    public void SetPool(IObjectPool<UnitController> pool) => _pool = pool;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, unitData.attackRange);

        if (target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, target.transform.position);
        }
    }
}
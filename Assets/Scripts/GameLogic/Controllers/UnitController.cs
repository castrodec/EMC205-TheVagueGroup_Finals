using System;
using UnityEngine;
using UnityEngine.Pool;

public class UnitController : MonoBehaviour, IDamageable
{
    public UnitScriptableObject unitData;
    [HideInInspector] public IState currentState, marchingState, attackingState, flyingState, reloadingState;

    public int currentHealth, currentAmmo;
    public GameObject target;
    public bool isAlly;
    private IObjectPool<UnitController> _pool;

    private void Start()
    {
        marchingState = new MarchingState(this);
        attackingState = new AttackingState(this);
        flyingState = new FlyingState(this);
        reloadingState = new ReloadingState(this);

        ResetUnit(transform.position, transform.rotation, isAlly,  unitData);
        ChangeState(unitData.attackType == UnitScriptableObject.AttackType.Flying ? flyingState : marchingState);
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
        // Use All to ensure we don't just stop at the first collider hit (which might be the unit itself)
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, unitData.attackRange, unitData.targetLayer);
        string targetTag = isAlly ? "Enemy" : "Ally";

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            if (hit.CompareTag(targetTag))
            {
                target = hit.gameObject;
                return true;
            }
        }
        return false;
    }

    public bool DetectAllyInFront()
    {
        Vector2 direction = isAlly ? Vector2.right : Vector2.left;
        float distance = 1f;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, unitData.targetLayer);

        if (hit && hit.collider.gameObject.GetComponent<UnitController>().unitData.attackType == unitData.attackType
        && hit.collider.CompareTag(isAlly ? "Ally" : "Enemy"))
        {
            target = hit.transform.gameObject;
            return true;
        }

        return false;
    }

    public void ShootDirection(Vector2 direction) => ObjectPooler.Instance.SpawnProjectile(unitData.projectileData, transform.position, null, direction, isAlly);

    public void ShootTarget(Transform target) => ObjectPooler.Instance.SpawnProjectile(unitData.projectileData, transform.position, target, Vector2.zero, isAlly);

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
            ResourceManager.Instance.AddCoins(Mathf.RoundToInt(unitData.unitCost * 1.2f));
            WaveManager.Instance.EnemyDied();
        }
    }

    public void ResetUnit(Vector2 pos, Quaternion rot, bool ally, UnitScriptableObject data)
    {
        currentHealth = unitData.maxHealth;
        currentAmmo = unitData.ammoCapacity;
        transform.position = pos;
        transform.rotation = rot;
        isAlly = ally;
        gameObject.tag = isAlly ? "Ally" : "Enemy";
        unitData = data;
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
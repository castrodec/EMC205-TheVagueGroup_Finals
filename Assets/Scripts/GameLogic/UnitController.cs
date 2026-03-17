using UnityEngine;
using UnityEngine.Pool;

public class UnitController : MonoBehaviour, IDamageable
{
    public UnitScriptableObject unitData;
    [HideInInspector] public IState currentState;
    [HideInInspector] public IState marchingState, attackingState, flyingState, reloadingState;

    [Header("Runtime Attributes")]
    public int currentHealth;
    public int currentAmmo;
    public GameObject target;
    public bool isAlly;
    private IObjectPool<UnitController> pool;

    public void Initialize(UnitScriptableObject data)
    {
        unitData = data;
        ChangeState(marchingState);
    }

    void Awake()
    {
        marchingState = new MarchingState(this);
        attackingState = new AttackingState(this);
        flyingState = new FlyingState(this);
        reloadingState = new ReloadingState(this);
    }

    void Start()
    {
        currentHealth = unitData.maxHealth;
        currentAmmo = unitData.ammoCapacity;
        ChangeState(marchingState);
    }

    void Update()
    {
        currentState?.Tick();
    }

    public void ChangeState(IState newState)
    {
        if (currentState != null)
            currentState?.Exit();

        currentState = newState;

        if (currentState != null)
            currentState?.Enter();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(name + " Got hit!");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void ResetUnit(Vector2 position, Quaternion direction, bool isAlly)
    {
        currentHealth = unitData.maxHealth;
        currentAmmo = unitData.ammoCapacity;
        transform.position = position;
        transform.rotation = direction;
        this.isAlly = isAlly;
    }

    public void Die()
    {
        pool.Release(this);
    }

    public void SetPool(IObjectPool<UnitController> pool)
    {
        this.pool = pool;
    }

    public void ShootBullet(Transform targetPosition)
    {
        ObjectPooler.Instance.SpawnProjectile(unitData.projectileData, transform.position, Quaternion.identity, isAlly, targetPosition);
        currentAmmo--;
    }
}

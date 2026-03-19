using UnityEngine;
using UnityEngine.Pool;

public class TurretController : MonoBehaviour, IDamageable
{
    public TurretScriptableObject data;
    public IState currentState, idleState, firingState;
    public GameObject target;
    public int currentHealth;
    public IObjectPool<TurretController> pool;

    private void Awake()
    {
        idleState = new IdleState(this);
        firingState = new FiringState(this);
    }

    public void ResetTurret(Vector2 pos, TurretScriptableObject newData)
    {
        transform.position = pos;
        data = newData;
        currentHealth = data.maxHealth;
        ChangeState(idleState);
    }

    public void ChangeState(IState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    void Update() => currentState?.Tick();

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0) Die();
    }

    public void Die() => pool?.Release(this);

    public void SetPool(IObjectPool<TurretController> pool) => this.pool = pool;
}

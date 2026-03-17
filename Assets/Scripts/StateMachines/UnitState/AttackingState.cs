using UnityEngine;

public class AttackingState : IState
{
    private UnitController _unit;
    private float _lastAttackTime;

    public AttackingState(UnitController unit) => _unit = unit;

    public void Enter() => Debug.Log(_unit.name + " started attacking!");

    public void Tick()
    {
        // 1. Check if target still exists and is in range
        if (_unit.target == null || Vector2.Distance(_unit.transform.position, _unit.target.transform.position) > _unit.unitData.attackRange)
        {
            _unit.target = null;
            _unit.ChangeState(_unit.marchingState);
            return;
        }

        // 2. Attack based on interval
        if (Time.time >= _lastAttackTime + _unit.unitData.attackInterval)
        {
            PerformAttack();
            _lastAttackTime = Time.time;
        }
    }

    private void PerformAttack()
{
    var enemy = _unit.target.GetComponent<UnitController>();
    
    if (_unit.unitData.isRanged) 
    {
        if (_unit.currentAmmo <= 0)
            {
                _unit.ChangeState(_unit.reloadingState);
            }
        
        // Instead of hardcoding indices, we use the data from the SO
        ObjectPooler.Instance.SpawnProjectile(
            _unit.unitData.projectileData, // The data asset
            _unit.transform.position, 
            Quaternion.identity, 
            _unit.isAlly, 
            enemy.transform
        );
    } 
    else 
    {
        enemy.TakeDamage(_unit.unitData.attackDamage);
    }
}

    public void Exit() { }
}
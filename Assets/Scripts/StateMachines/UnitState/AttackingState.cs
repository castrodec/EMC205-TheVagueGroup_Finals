using UnityEngine;

public class AttackingState : IState
{
    private UnitController _unit;
    private float _attackTimer;

    public AttackingState(UnitController unit) => _unit = unit;

    public void Enter() => _attackTimer = _unit.unitData.attackInterval;

    public void Tick()
    {
        // Safety check: if target died or was disabled by something else
        if (_unit.target == null || !_unit.target.activeSelf)
        {
            _unit.ChangeState(_unit.marchingState);
            return;
        }

        float distance = Vector2.Distance(_unit.transform.position, _unit.target.transform.position);
        
        if (distance > _unit.unitData.attackRange)
        {
            _unit.ChangeState(_unit.marchingState);
            return;
        }

        _attackTimer += Time.deltaTime;
        if (_attackTimer >= _unit.unitData.attackInterval)
        {
            if (_unit.unitData.attackType == UnitScriptableObject.AttackType.Ranged)
            {
                if (_unit.currentAmmo <= 0) { _unit.ChangeState(_unit.reloadingState); return; }
                
                _unit.ShootTarget(_unit.target);
                _unit.currentAmmo--;
            }
            else // Melee
            {
                _unit.target.GetComponent<IDamageable>()?.TakeDamage(_unit.unitData.attackDamage);
            }
            _attackTimer = 0;
        }
    }

    public void Exit()
    {
        _unit.target = null;
    }
}
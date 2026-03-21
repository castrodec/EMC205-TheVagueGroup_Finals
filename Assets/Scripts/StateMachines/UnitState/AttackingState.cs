using UnityEngine;

public class AttackingState : IState
{
    private UnitController _unit;
    private float _attackTimer;

    public AttackingState(UnitController unit) => _unit = unit;

    public void Enter() => _attackTimer = _unit.unitData.attackInterval;

    public void Tick()
    {
        if (_unit.target == null || _unit.target.gameObject.activeSelf == false)
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
                
                _unit.ShootTarget(_unit.target.transform);
                _unit.currentAmmo--;
            }
            else // Melee
            {
                _unit.target.GetComponent<IDamageable>()?.TakeDamage(_unit.unitData.attackDamage);
            }
            _attackTimer = 0;
        }
    }

    public void Exit() { }
}
using System.Threading;
using UnityEngine;

public class ShootingState : IState
{
    private TurretController _turret;
    private float _timer;
    public ShootingState(TurretController turretController)
    {
        this._turret = turretController;
    }

    public void Enter() => _timer = _turret.turretData.fireRate;

    public void Tick()
    {
        // Check if target exists and is active
        if (_turret.target == null || !_turret.target.activeSelf)
        {
            _turret.ChangeState(_turret.idleState);
            return;
        }

        // NEW: Check if target is out of range
        float distance = Vector2.Distance(_turret.transform.position, _turret.target.transform.position);
        if (distance > _turret.turretData.detectionRange)
        {
            _turret.ChangeState(_turret.idleState);
            return;
        }

        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            if (_turret.turretData.turretType == TurretScriptableObject.TurretType.Bolt) _turret.ShootDirection(Vector2.right);
            else _turret.ShootTarget(_turret.target);
            _timer = _turret.turretData.fireRate;
        }
    }

    public void Exit() => _turret.target = null;
    
}

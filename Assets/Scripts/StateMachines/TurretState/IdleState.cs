using UnityEngine;

public class IdleState : IState
{
    private TurretController _turret;
    public IdleState(TurretController turretController) => this._turret = turretController;
    public void Enter() { }
    public void Tick()
    {
        if (_turret.DetectTarget())
        {
            if (_turret.turretData.turretType == TurretScriptableObject.TurretType.Trap) _turret.Explode();
            else _turret.ChangeState(_turret.shootingState);
        }
    }
    public void Exit() { }
}

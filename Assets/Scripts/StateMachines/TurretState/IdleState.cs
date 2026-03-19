using UnityEngine;

public class IdleState : IState
{
    private TurretController _turret;
    public IdleState(TurretController turret)
    {
        _turret = turret;
    }
    public void Enter() { }
    public void Tick() { }
    public void Exit() { }
}

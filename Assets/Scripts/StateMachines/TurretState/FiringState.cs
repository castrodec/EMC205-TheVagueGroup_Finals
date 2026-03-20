using UnityEngine;

public class FiringState : IState
{
    private TurretController _turret;
    public FiringState(TurretController turret)
    {
        _turret = turret;
    }
    public void Enter() { }
    public void Tick() { }
    public void Exit() { }
}

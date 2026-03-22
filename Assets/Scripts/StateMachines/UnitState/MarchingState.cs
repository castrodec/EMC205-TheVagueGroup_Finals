using UnityEngine;

public class MarchingState : IState
{
    private UnitController _unit;

    public MarchingState(UnitController unit) => _unit = unit;

    public void Enter() { }

    // MarchingState.cs
    public void Tick()
    {
        // Use the Controller's detection logic to see if we should fight
        if (_unit.DetectEnemy() || _unit.DetectTurret())
        {
            _unit.ChangeState(_unit.attackingState);
        }

        if (_unit.DetectAllyInFront())
        {
            return;
        }

        _unit.HandleBasicMovement();
    }

    public void Exit() { }
}
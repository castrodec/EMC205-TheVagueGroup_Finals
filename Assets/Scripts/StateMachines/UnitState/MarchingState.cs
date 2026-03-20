using UnityEngine;

public class MarchingState : IState
{
    private UnitController _unit;

    public MarchingState(UnitController unit) => _unit = unit;

    public void Enter() { }

    // MarchingState.cs
    public void Tick()
    {
        // 1. Call the consolidated movement logic in the Controller
        _unit.HandleBasicMovement();

        // 2. Use the Controller's detection logic to see if we should fight
        if (_unit.DetectEnemy())
        {
            _unit.ChangeState(_unit.attackingState);
        }
    }

    public void Exit() { }
}
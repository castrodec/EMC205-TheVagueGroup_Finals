using UnityEngine;

public class FlyingState : IState
{
    private UnitController _unit;

    public FlyingState(UnitController unit)
    {
        _unit = unit;
    }

    public void Enter()
    {
        
    }

    public void Tick()
    {
        // Implement attack logic here
        
    }

    public void Exit()
    {
        
    }
}

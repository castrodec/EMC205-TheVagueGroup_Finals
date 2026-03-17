using UnityEngine;

public class ReloadingState : IState
{
    private UnitController _unit;

    public ReloadingState(UnitController unit)
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

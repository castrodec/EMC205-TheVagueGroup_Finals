using UnityEngine;

public class ReloadingState : IState
{
    private UnitController _unit;
    private float _timer;

    public ReloadingState(UnitController unit) => _unit = unit;

    public void Enter() => _timer = 0f;

    public void Tick()
    {

        //Reload Timer logic
        _timer += Time.deltaTime;
        if (_timer >= _unit.unitData.reloadTime)
        {
            _unit.currentAmmo = _unit.unitData.ammoCapacity;
            _unit.ChangeState(_unit.marchingState);
        }
    }

    public void Exit() { }
}
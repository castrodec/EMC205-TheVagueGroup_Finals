using UnityEngine;

public class FlyingState : IState
{
    private UnitController _unit;
    private float _shootTimer;

    public FlyingState(UnitController unit) => _unit = unit;

    public void Enter() => _shootTimer = 0;

    public void Tick()
    {
        // 1. Move Forward (Using UnitController method)
        _unit.HandleBasicMovement();

        // 2. Maintain Hover height
        float hover = Mathf.Sin(Time.time * _unit.unitData.hoverSpeed) * _unit.unitData.hoverAmount;
        _unit.transform.position = new Vector2(_unit.transform.position.x, _unit.unitData.flightHeight + hover);

        // 3. Continuous Downward Shooting
        _shootTimer += Time.deltaTime;
        if (_shootTimer >= _unit.unitData.attackInterval)
        {
            _unit.ShootDirection(Vector2.down);
            _shootTimer = 0;
        }
    }

    public void Exit() { }
}
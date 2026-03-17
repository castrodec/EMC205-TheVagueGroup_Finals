using UnityEngine;

public class MarchingState : IState
{
    private UnitController _unit;
    private string _tag;

    public MarchingState(UnitController unit)
    {
        _unit = unit;
    }

    public void Enter()
    {
        Debug.Log("Entering Marching State for unit" + _unit.name);
        _tag = _unit.isAlly ? "Enemy" : "Ally";
    }

    public void Tick()
    {
        if (_unit.isAlly) _unit.transform.Translate(Vector2.right * _unit.unitData.moveSpeed * Time.deltaTime);
        else _unit.transform.Translate(Vector2.left * _unit.unitData.moveSpeed * Time.deltaTime);
        if (DetectEnemyInRange())
        {
            _unit.ChangeState(_unit.attackingState);
        }
    }

    public void Exit()
    {
        Debug.Log("Exiting Marching State for unit" + _unit.name);
    }

    public bool DetectEnemyInRange()
    {
        float detectionRange = _unit.unitData.attackRange;
        Collider2D[] hits = Physics2D.OverlapCircleAll(_unit.transform.position, detectionRange);

        if (hits.Length > 0)
        {
            GameObject closestTarget = null;
            float closestDistance = Mathf.Infinity;

            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag(_tag))
                {
                    float distance = Vector2.Distance(_unit.transform.position, hit.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestTarget = hit.gameObject;
                    }
                }
            }

            if (closestTarget != null)
            {
                _unit.target = closestTarget;
                return true;
            }
        }

        return false; // fallback if no enemies are detected
    }
}

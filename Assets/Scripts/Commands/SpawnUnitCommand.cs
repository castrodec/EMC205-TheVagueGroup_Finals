using UnityEngine;

public class SpawnUnitCommand : ICommand
{
    private UnitScriptableObject _unitData;
    private Vector2 _spawnPosition;
    private bool _isAlly;

    public SpawnUnitCommand(UnitScriptableObject data, Vector2 position, bool isAlly)
    {
        _unitData = data;
        _spawnPosition = position;
        _isAlly = isAlly;
    }

    public void Execute()
    {
        ObjectPooler.Instance.SpawnUnit(_unitData, _spawnPosition, _isAlly);
    }

    public void Undo() 
    { }
}
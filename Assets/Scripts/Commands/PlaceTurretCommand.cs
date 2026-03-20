using UnityEngine;

public class PlaceTurretCommand : ICommand
{
    private TurretScriptableObject _data;
    private Vector2 _position;

    public PlaceTurretCommand(TurretScriptableObject data, Vector2 position)
    {
        _data = data;
        _position = position;
    }

    public void Execute()
    {
        ObjectPooler.Instance.SpawnTurret(_data, _position);
    }
    public void Undo() { /* Logic to return to pool */ }
}
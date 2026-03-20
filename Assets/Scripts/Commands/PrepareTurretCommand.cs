public class PrepareTurretCommand : ICommand
{
    private TurretBuilder _builder;
    private TurretScriptableObject _data;

    public PrepareTurretCommand(TurretBuilder builder, TurretScriptableObject data)
    {
        _builder = builder;
        _data = data;
    }

    public void Execute() => _builder.EnterBuildMode(_data);
    public void Undo() => _builder.ExitBuildMode();
}
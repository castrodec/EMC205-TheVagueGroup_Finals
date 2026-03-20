public class ScrollCommand : ICommand
{
    private CameraController _receiver;
    private float _distance;

    public ScrollCommand(CameraController receiver, float distance)
    {
        _receiver = receiver;
        _distance = distance;
    }

    public void Execute()
    {
        _receiver.Move(_distance);
    }

    public void Undo() => _receiver.Move(-_distance);
}
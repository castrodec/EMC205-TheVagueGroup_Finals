public interface ICommand
{
    void Execute();
    void Queue();
    void Undo();
}

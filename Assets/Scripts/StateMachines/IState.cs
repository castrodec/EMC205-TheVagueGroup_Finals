using Unity.VisualScripting;

public interface IState
{
    void Enter();
    void Tick();
    void Exit();
}

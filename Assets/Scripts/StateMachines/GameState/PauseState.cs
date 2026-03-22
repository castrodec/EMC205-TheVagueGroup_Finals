using UnityEngine;

public class PauseState : IState
{
    private GameManager playerManager;
    public PauseState(GameManager playerManager) => this.playerManager = playerManager;

    public void Enter()
    {
        Time.timeScale = 0f;
        UIManager.Instance.TogglePauseMeu();
    }
    public void Tick() { }
    public void Exit() { }
}

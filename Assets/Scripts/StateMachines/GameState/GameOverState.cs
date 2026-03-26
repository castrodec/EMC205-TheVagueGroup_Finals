using UnityEngine;

public class GameOverState : IState
{
    private GameManager playerManager;
    public GameOverState(GameManager playerManager) => this.playerManager = playerManager;

    public void Enter()
    {
        Time.timeScale = 0f;
        UIManager.Instance.ToggleGameOverMenu();
    }
    public void Tick() { }
    public void Exit() { }
}

using UnityEngine;

public class PlayState : IState
{
    private GameManager playerManager;
    public PlayState(GameManager playerManager) => this.playerManager = playerManager;

    public void Enter() 
    { 
        Time.timeScale = 1f; 
    }
    public void Tick() { }
    public void Exit() { }
}

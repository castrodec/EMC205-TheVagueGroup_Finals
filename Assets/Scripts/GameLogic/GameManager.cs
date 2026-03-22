using UnityEngine;

public class GameManager : MonoBehaviour, IDamageable
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private int Health = 5;
    public bool playerHasWon = false;
    public GameObject playerBase;
    private IState playState, pauseState, gameOverState;
    private IState currentState;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
    private void Start()
    {
        playState = new PlayState(this);
        pauseState = new PauseState(this);
        gameOverState = new GameOverState(this);
        UIManager.Instance.UpdateHealthDisplay(Health);
        ChangeState(playState);
    }

    public void ChangeState(IState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    public void TogglePause()
    {
        if (currentState == playState) ChangeState(pauseState);
        else ChangeState(playState);
    }

    public void Update() => currentState?.Tick();

    public void TakeDamage(int damage)
    {
        Health -= damage;
        UIManager.Instance.UpdateHealthDisplay(Health);
        if (Health <= 0) Die();
    }

    public void Die()
    {
        ChangeState(gameOverState);
    }
}

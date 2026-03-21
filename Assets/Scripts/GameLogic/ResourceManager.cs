using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;
    [SerializeField] private int coins = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    void Start()
    {
        UIManager.Instance.UpdateCoinsDisplay(coins);
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        UIManager.Instance.UpdateCoinsDisplay(coins);
    }

    public bool RemoveCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            UIManager.Instance.UpdateCoinsDisplay(coins);
            return true;
        }
        return false;
    }
}

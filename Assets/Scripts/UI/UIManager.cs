using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI References")] 
    public TextMeshProUGUI coinsDisplay;
    public TextMeshProUGUI healthDisplay;
    public TextMeshProUGUI waveCountDisplay;
    public GameObject startWaveButton;
    public static UIManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    public void UpdateCoinsDisplay(int amount) => coinsDisplay.text = "Coins: " + amount;

    public void UpdateHealthDisplay(int amount) => healthDisplay.text = "Health: " + amount;

    public void UpdateWaveCountDisplay(int amount, int MaxAmount) => waveCountDisplay.text = "Enemies Left: " + amount + "/" + MaxAmount;
    public void UpdateStartWaveButton(bool active) => startWaveButton.SetActive(active);
}

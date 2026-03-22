using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI References")] 
    public TextMeshProUGUI coinsDisplay;
    public TextMeshProUGUI healthDisplay;
    public TextMeshProUGUI waveCountDisplay;
    public GameObject startWaveButton;

    [Header("Queue UI")]
    public Image[] queueSlots; // Drag your 6 UnitIcon images here
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
    public void UpdateQueueUI(UnitScriptableObject[] currentQueue)
    {
        for (int i = 0; i < queueSlots.Length; i++)
        {
            if (i < currentQueue.Length)
            {
                // Show the unit sprite
                queueSlots[i].color = Color.white; // Make sure it's visible
            }
            else
            {
                // Hide or show empty
                queueSlots[i].color = new Color(1, 1, 1, 0.10f); // Transparent if no empty sprite
            }
        }
    }
    /// <summary>
    /// Add menu tween and toggles
    /// </summary>
    public void TogglePauseMeu() { Debug.Log("Toggle Pause Menu"); }
    public void ToggleUnitsMenu() { }
    public void ToggleTurretsMenu() { }
    public void ToggleSettingsMenu() { }
}

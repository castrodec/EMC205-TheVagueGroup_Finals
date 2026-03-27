using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    public float tweenSpeed = 0.5f;
    [Header("UI References")] 
    
    public TextMeshProUGUI healthDisplay;
    public Slider healthBar;

    public TextMeshProUGUI coinsDisplay;


    public TextMeshProUGUI waveCountDisplay;
    public Slider waveBar;

    public GameObject startWaveButton;
    [Header("Tweening Shit")]
    public GameObject buildPanel,summonPanel;
    public RectTransform movingPanels;
    private Tween panelTween;
    [Header("Queue UI")]
    public Image[] queueSlots; // Drag your 6 UnitIcon images here
    public Slider queueBar;
    private int queueLeft;
    public static UIManager Instance { get; private set; }    

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

    }

    public void UpdateCoinsDisplay(int amount) => coinsDisplay.text = "" + amount;

    public void UpdateHealthDisplay(int amount){ healthDisplay.text = amount + "/5"; healthBar.value = amount;}

    public void UpdateWaveCountDisplay(int amount, int MaxAmount){waveCountDisplay.text = "Enemies Left: " + amount + "/" + MaxAmount;waveBar.maxValue = MaxAmount;waveBar.value = amount;}
    public void UpdateStartWaveButton(bool active) => startWaveButton.SetActive(active);
   public void UpdateQueueUI(UnitScriptableObject[] currentQueue)
    {
    queueBar.maxValue = queueSlots.Length;

    for (int i = 0; i < queueSlots.Length; i++)
    {
        if (i < currentQueue.Length)
        {
            queueSlots[i].color = Color.white;
        }
        else
        {
            queueSlots[i].color = new Color(1, 1, 1, 0.10f);
        }
    }

    // 🔄 Inverted logic
    queueBar.value = queueSlots.Length - currentQueue.Length;
    }
    /// <summary>
    /// Add menu tween and toggles
    /// </summary>
    public void TogglePauseMeu() { Debug.Log("Toggle Pause Menu"); }
    public void ToggleSettingsMenu() { }
    public void ToggleSummonPanel()
    {
        if(buildPanel.activeSelf){QuickClosePanel(buildPanel);summonPanel.SetActive(true);OpenPanel(summonPanel);}
        else if(summonPanel.activeSelf){ClosePanel(summonPanel);}
        else{
        OpenPanel(summonPanel);;
        }
    }
        
    public void ToggleBuildPanel()
    {
        if(summonPanel.activeSelf){QuickClosePanel(summonPanel);OpenPanel(buildPanel);}
        else if(buildPanel.activeSelf){ClosePanel(buildPanel);}
        else
        {
            OpenPanel(buildPanel);
        }
    }

    //All tweening Fucntions
     public void OpenPanel(GameObject panelToOpen)
    {
        KillTween();
        StopCoroutine(DelayClose(panelToOpen));
        panelToOpen.SetActive(true);
        panelTween = movingPanels.DOAnchorPosX(560f, tweenSpeed)
            .SetEase(Ease.InOutSine);
    }

    public void ClosePanel(GameObject panelToClose)
    {
        KillTween();

        panelTween = movingPanels.DOAnchorPosX(960f, tweenSpeed)
            .SetEase(Ease.InOutSine);
        StartCoroutine(DelayClose(panelToClose));
    }

    private void QuickClosePanel(GameObject panelToClose)
    {
        KillTween();

        panelTween = movingPanels.DOAnchorPosX(960f, tweenSpeed)
            .SetEase(Ease.InOutSine);
        panelToClose.SetActive(false);
    }


    private IEnumerator DelayClose(GameObject panelToClose)
    {
        yield return new WaitForSeconds(tweenSpeed);
        panelToClose.SetActive(false);
    }

    // 🔹 Stop any current tween
    public void KillTween()
    {
        if (panelTween != null && panelTween.IsActive())
        {
            panelTween.Kill(true);
        }
    }
    
}

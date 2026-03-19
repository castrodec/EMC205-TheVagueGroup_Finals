using UnityEngine;

public class TurretBuilder : MonoBehaviour
{
    [Header("Settings")]
    public float groundY = -4f;
    public float maxBuildX = 10f; // Prevent building too far into enemy territory
    public LayerMask obstacleLayer;
    
    [Header("Ghost Visuals")]
    public GameObject ghostPrefab; // A simple prefab with a SpriteRenderer
    private SpriteRenderer _ghostSR;
    private TurretScriptableObject _pendingTurret;
    private bool _isBuilding = false;

    void Awake()
    {
        _ghostSR = ghostPrefab.GetComponent<SpriteRenderer>();
        ghostPrefab.SetActive(false);
    }

    void Update()
    {
        if (!_isBuilding) return;

        // Follow Mouse on X, stick to Ground on Y
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float clampedX = Mathf.Clamp(mousePos.x, -maxBuildX, maxBuildX);
        ghostPrefab.transform.position = new Vector2(clampedX, groundY);

        // Check if placement is valid
        bool isAreaClear = !Physics2D.OverlapBox(ghostPrefab.transform.position, new Vector2(1, 1), 0, obstacleLayer);
        _ghostSR.color = isAreaClear ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f);

        if (Input.GetMouseButtonDown(0) && isAreaClear)
        {
            ConfirmPlacement();
        }
        
        if (Input.GetKeyDown(KeyCode.Escape)) ExitBuildMode();
    }

    public void EnterBuildMode(TurretScriptableObject data)
    {
        _pendingTurret = data;
        _isBuilding = true;
        ghostPrefab.SetActive(true);
        // Change ghost sprite to match turret
        // _ghostSR.sprite = data.turretSprite; 
    }

    private void ConfirmPlacement()
    {
        ICommand placeCmd = new PlaceTurretCommand(_pendingTurret, ghostPrefab.transform.position);
        placeCmd.Execute();
        ExitBuildMode();
    }

    public void ExitBuildMode()
    {
        _isBuilding = false;
        ghostPrefab.SetActive(false);
        _pendingTurret = null;
    }
}
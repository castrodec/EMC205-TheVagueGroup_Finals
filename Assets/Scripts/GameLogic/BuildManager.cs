using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance { get; private set; }

    [Header("Settings")]
    public LayerMask placementLayer; // A layer for your 'Lanes' or 'Plots'
    public Color validColor = new Color(0, 1, 0, 0.5f); // Semi-transparent Green
    public Color invalidColor = new Color(1, 0, 0, 0.5f); // Semi-transparent Red

    private TurretScriptableObject _selectedTurret;
    private GameObject _ghostInstance;
    private SpriteRenderer _ghostRenderer;

    void Awake() => Instance = this;

    void Update()
    {
        if (_selectedTurret != null)
        {
            HandleGhostMovement();
            
            if (Input.GetMouseButtonDown(0)) PlaceTurret();
            if (Input.GetMouseButtonDown(1)) CancelBuilding();
        }
    }

    public void EnterBuildMode(TurretScriptableObject data)
    {
        _selectedTurret = data;
        
        // Create the visual preview
        if (_ghostInstance != null) Destroy(_ghostInstance);
        _ghostInstance = new GameObject("BuildGhost");
        _ghostRenderer = _ghostInstance.AddComponent<SpriteRenderer>();
        _ghostRenderer.sprite = data.turretSprite;
        _ghostRenderer.sortingOrder = 10; // Appear above everything
    }

    private void HandleGhostMovement()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        // Snapping to grid (adjust 1.0f to your lane size)
        float snappedX = Mathf.Round(mousePos.x);
        float snappedY = Mathf.Round(mousePos.y);
        Vector2 targetPos = new Vector2(snappedX, snappedY);

        _ghostInstance.transform.position = targetPos;

        // Check if the spot is already occupied or off-lane
        bool canPlace = CheckPlacement(targetPos);
        _ghostRenderer.color = canPlace ? validColor : invalidColor;
    }

    private bool CheckPlacement(Vector2 pos)
    {
        // 1. Check if we are over a valid placement layer
        Collider2D laneHit = Physics2D.OverlapPoint(pos, placementLayer);
        if (laneHit == null) return false;

        // 2. Check if a turret already exists at this EXACT position
        // We use a small circle to see if any TurretController is there
        Collider2D turretHit = Physics2D.OverlapCircle(pos, 0.1f, LayerMask.GetMask("Turrets"));
        return turretHit == null;
    }

    private void PlaceTurret()
    {
        Vector2 pos = _ghostInstance.transform.position;
        if (CheckPlacement(pos))
        {
            if (ResourceManager.Instance.RemoveCoins(_selectedTurret.turretCost))
            {
                ObjectPooler.Instance.SpawnTurret(_selectedTurret, pos);
                CancelBuilding();
            }
        }
    }

    public void CancelBuilding()
    {
        if (_ghostInstance != null) Destroy(_ghostInstance);
        _selectedTurret = null;
    }
}
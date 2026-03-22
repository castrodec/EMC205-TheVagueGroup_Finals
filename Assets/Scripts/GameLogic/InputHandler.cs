using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [Header("Camera Input Settings")]
    public CameraController cameraController;
    public float keyboardSpeed = 30f;
    public float dragSensitivity = 0.05f; 
    private Vector3 _lastMousePosition;

    [Header("References")]
    public UnitSpawner unitSpawner;
    public AirStrikeHandler airStrikeHandler;
    public GameManager playerManager;
    public BuildManager buildManager;

    [Header("UnitData")]
    public UnitScriptableObject footSoldier, assaultSoldier, jetpacker;

    [Header("TurretData")]
    public TurretScriptableObject basicTurret, missileSilo, shockTrap;
    

    private void Update()
    {
        HandleKeyboard();
        HandleMouseDrag();
        HandleHotkeys(); // New method
    }

    private void HandleKeyboard()
    {
        float x = Input.GetAxisRaw("Horizontal");
        if (x != 0)
        {
            float speed = x * keyboardSpeed * Time.deltaTime;
            ExecuteScroll(speed);
        }
    }

    private void HandleHotkeys()
    {
        // --- UNITS (1, 2, 3) ---
        if (Input.GetKeyDown(KeyCode.Alpha1)) unitSpawner.EnqueueSummon(footSoldier);
        if (Input.GetKeyDown(KeyCode.Alpha2)) unitSpawner.EnqueueSummon(assaultSoldier);
        if (Input.GetKeyDown(KeyCode.Alpha3)) unitSpawner.EnqueueSummon(jetpacker);

        // --- BUILD MODE (E, R, T) ---
        if (Input.GetKeyDown(KeyCode.E)) buildManager.EnterBuildMode(basicTurret);
        if (Input.GetKeyDown(KeyCode.R)) buildManager.EnterBuildMode(missileSilo);
        if (Input.GetKeyDown(KeyCode.T)) buildManager.EnterBuildMode(shockTrap);

        // --- ABILITIES (Q) ---
        if (Input.GetKeyDown(KeyCode.Q)) airStrikeHandler.StartAirStrike();

        // --- PAUSE (ESC) ---
        if (Input.GetKeyDown(KeyCode.Escape)) playerManager.TogglePause();
    }

    private void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            // Calculate how far the mouse moved IN PIXELS since last frame
            Vector3 delta = Input.mousePosition - _lastMousePosition;
            
            if (delta.x != 0)
            {
                // Invert delta.x so dragging left moves camera right
                float moveDistance = -delta.x * dragSensitivity;
                ExecuteScroll(moveDistance);
            }

            _lastMousePosition = Input.mousePosition;
        }
    }

    private void ExecuteScroll(float amount)
    {
        ICommand cmd = new ScrollCommand(cameraController, amount);
        cmd.Execute();
    }
}
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public CameraController cameraController;
    public float keyboardSpeed = 30f;
    public float dragSensitivity = 0.05f; 
    public TurretBuilder turretBuilder;
    public TurretScriptableObject basicData, siloData, shockerData;
    private Vector3 _lastMousePosition;

    private void Update()
    {
        HandleKeyboard();
        HandleMouseDrag();
        HandleBuildInputs();
    }

    private void HandleKeyboard()
    {
        float x = UnityEngine.Input.GetAxisRaw("Horizontal");
        if (x != 0)
        {
            float speed = x * keyboardSpeed * Time.deltaTime;
            ExecuteScroll(speed);
        }
    }

    private void HandleBuildInputs()
    {
        if (Input.GetKeyDown(KeyCode.E)) ExecuteBuildCommand(basicData);
        if (Input.GetKeyDown(KeyCode.R)) ExecuteBuildCommand(siloData);
        if (Input.GetKeyDown(KeyCode.T)) ExecuteBuildCommand(shockerData);
    }

    private void ExecuteBuildCommand(TurretScriptableObject data)
    {
        ICommand cmd = new PrepareTurretCommand(turretBuilder, data);
        cmd.Execute();
    }

    private void HandleMouseDrag()
    {
        if (UnityEngine.Input.GetMouseButtonDown(0))
        {
            _lastMousePosition = UnityEngine.Input.mousePosition;
        }

        if (UnityEngine.Input.GetMouseButton(0))
        {
            // Calculate how far the mouse moved IN PIXELS since last frame
            Vector3 delta = UnityEngine.Input.mousePosition - _lastMousePosition;
            
            if (delta.x != 0)
            {
                // Invert delta.x so dragging left moves camera right
                float moveDistance = -delta.x * dragSensitivity;
                ExecuteScroll(moveDistance);
            }

            _lastMousePosition = UnityEngine.Input.mousePosition;
        }
    }

    private void ExecuteScroll(float amount)
    {
        ICommand cmd = new ScrollCommand(cameraController, amount);
        cmd.Execute();
    }
}
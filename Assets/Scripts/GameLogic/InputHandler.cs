using UnityEngine;

public class InputHandler : MonoBehaviour
{

    public CameraController cameraController;
    public float keyboardSpeed = 30f;
    public float dragSensitivity = 0.05f; 
    private Vector3 _lastMousePosition;

    private void Update()
    {
        HandleKeyboard();
        HandleMouseDrag();
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
using UnityEngine;

/// <summary>
/// Controller class for camera behavior.
/// Contains methods for moving the camera and getting mouse world position.
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Bounds")]
    [SerializeField] private float minX = -40f;
    [SerializeField] private float maxX = 40f;

    private Camera _cam;

    void Awake() => _cam = GetComponent<Camera>();

    public void Move(float xOffset)
    {
        Vector3 pos = transform.position;
        pos.x += xOffset;
        pos.x = Mathf.Clamp(pos.x, minX, maxX); // Clamp camera movement to boundaries
        transform.position = pos;
    }

    public Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f; 
        return _cam.ScreenToWorldPoint(mousePos);
    }
}
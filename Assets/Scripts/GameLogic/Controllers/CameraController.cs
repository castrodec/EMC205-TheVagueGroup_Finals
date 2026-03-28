using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Bounds")]
    [SerializeField] private float minX = 0f;
    [SerializeField] private float maxX = 50f;

    private Camera _cam;

    void Awake() => _cam = GetComponent<Camera>();

    public void Move(float xOffset)
    {
        Vector3 pos = transform.position;
        pos.x += xOffset;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        transform.position = pos;
    }

    public Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f; 
        return _cam.ScreenToWorldPoint(mousePos);
    }
}
using UnityEngine;

public class FixedCameraFollow : MonoBehaviour
{
    public static FixedCameraFollow Instance { get; private set; }

    public Transform target;
    public Vector3 offset = new Vector3(0, 18, -10);
    public Vector3 rotation = new Vector3(60, 0, 0);

    [Header("Zoom Settings")]
    public float zoomSpeed = 5f;
    public float minDistance = 5f;
    public float maxDistance = 40f;
    
    private float currentDistance;

    [Header("Shake Settings")]
    private float shakeIntensity = 0f;
    private float shakeTimer = 0f;
    private float totalShakeTime = 0f;
    private float startingIntensity = 0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        currentDistance = offset.magnitude;
    }

    public void Shake(float intensity, float time)
    {
        shakeIntensity = intensity;
        startingIntensity = intensity;
        shakeTimer = time;
        totalShakeTime = time;
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            // Zoom with Scroll Wheel
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                currentDistance -= scroll * zoomSpeed * 10f;
                currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
                offset = offset.normalized * currentDistance;
            }

            // Zoom with Right Mouse Drag
            if (Input.GetMouseButton(1))
            {
                float mouseY = Input.GetAxis("Mouse Y");
                currentDistance -= mouseY * zoomSpeed;
                currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
                offset = offset.normalized * currentDistance;
            }

            Vector3 shakeOffset = Vector3.zero;
            if (shakeTimer > 0)
            {
                shakeTimer -= Time.deltaTime;
                float currentIntensity = Mathf.Lerp(startingIntensity, 0f, 1 - (shakeTimer / totalShakeTime));
                shakeOffset = Random.insideUnitSphere * currentIntensity;
            }

            transform.position = target.position + offset + shakeOffset;
            transform.rotation = Quaternion.Euler(rotation);
        }
    }
}

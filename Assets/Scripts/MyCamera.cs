using System.Collections;
using UnityEngine;

// Class that controls the camera movement and shake
public class MyCamera : MonoBehaviour
{
    [SerializeField] AnimationCurve shakeCurve;
    private Camera cam;
    private Vector3 shakeOffset = Vector3.zero;
    
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    
    public static MyCamera Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        cam = GetComponent<Camera>();
        transform.position = target.position + offset;
    }
    private void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
        transform.position = shakeOffset + smoothedPosition;
    }
    
    public void SnapToDesiredPosition()
    {
        transform.position = target.position + offset;
    }
    
    public void ShakeCamera(float intensity = 1, float shakeDuration = 1f)
    {
        StartCoroutine(ShakeCoroutine(intensity, shakeDuration));
    }

    public IEnumerator ShakeCoroutine(float intensity = 1, float shakeDuration = 1f)
    {
        float elapsed = 0;
        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            shakeOffset = Random.insideUnitSphere * shakeCurve.Evaluate(elapsed/shakeDuration) * intensity;
            shakeOffset.z = 0;
            yield return null;
        }
        shakeOffset = Vector3.zero;
    }
}

using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] float shakeDuration;
    [SerializeField] AnimationCurve shakeCurve;
    private Camera cam;
    private Vector3 shakeOffset = Vector3.zero;
    
    public static CameraShake Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        cam = GetComponent<Camera>();
    }
    private void FixedUpdate()
    {
        transform.position = shakeOffset + new Vector3(0,0,transform.position.z);
    }
    
    public void ShakeCamera(float intensity = 1)
    {
        StartCoroutine(ShakeCoroutine(intensity));
    }

    public IEnumerator ShakeCoroutine(float intensity = 1)
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

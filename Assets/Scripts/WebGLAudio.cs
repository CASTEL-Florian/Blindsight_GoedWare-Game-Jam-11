using UnityEngine;

// Since 3D sounds don't work very well in WebGL, this script is used to simulate 3D sound by adjusting the volume of
// the AudioSource based on the distance to the AudioListener.
public class WebGLAudio : MonoBehaviour
{
    private AudioSource audioSource;
    private AudioListener audioListener;
    
    private float baseVolume;
    
    private void Start()
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer) return;
        audioSource = GetComponent<AudioSource>();
        audioListener = FindObjectOfType<AudioListener>();
        audioSource.spatialBlend = 0;
        baseVolume = audioSource.volume;
    }
    
    private void Update()
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer) return;
        Vector2 distance = transform.position - audioListener.transform.position;
        AudioRolloffMode rolloffMode = audioSource.rolloffMode;
        float volume = 0;
        if (rolloffMode == AudioRolloffMode.Custom)
        {
            volume = audioSource.GetCustomCurve(AudioSourceCurveType.CustomRolloff).Evaluate(distance.magnitude/audioSource.maxDistance);
        }
        else if (rolloffMode == AudioRolloffMode.Linear)
        {
            float minDistance = audioSource.minDistance;
            float maxDistance = audioSource.maxDistance;
            if (distance.magnitude < minDistance)
            {
                volume = 1;
            }
            else if (distance.magnitude > maxDistance)
            {
                volume = 0;
            }
            else
            {
                volume = 1 - (distance.magnitude - minDistance) / (maxDistance - minDistance);
            }
        }
        else
        {
            Debug.Log("Rolloff mode not supported");
        }
        
        volume *= baseVolume;
        audioSource.volume = volume;
    }
    
    public void SetVolume(float volume)
    {
        baseVolume = volume;
    }
}

using System.Collections;
using UnityEngine;


// A bat that emits sound rays at random intervals.
public class Bat : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    
    [SerializeField] private AudioClip doubleSqueakSound;
    
    [SerializeField] private float raySpeed = 2.5f;
    [SerializeField] private float rayLifetime = 2f;
    [SerializeField] private int rayDirectionCount = 6;
    [SerializeField] private float timeBetweenSqueaks = 0.7f;
    
    [SerializeField] private float minimumTimeBetweenSounds = 3f;
    [SerializeField] private float maximumTimeBetweenSounds = 10f;
    
    private float currentTime;

    private void Start()
    {
        currentTime = Random.Range(minimumTimeBetweenSounds, maximumTimeBetweenSounds);
    }
    
    private void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
        }
        
        if (currentTime <= 0)
        {
            
            StartCoroutine(SqueakCoroutine());
            currentTime = Random.Range(minimumTimeBetweenSounds, maximumTimeBetweenSounds);
        }
    }
    
    private IEnumerator SqueakCoroutine()
    {
        audioSource.PlayOneShot(doubleSqueakSound);
        float angle = Random.Range(0, 360);
        var position = transform.position;
        SoundEmitter.Instance.EmitSound(position, rayDirectionCount, raySpeed, rayLifetime, SoundEmitter.SoundType.Bat, angle);
        yield return new WaitForSeconds(timeBetweenSqueaks);
        angle = Random.Range(0, 360);
        SoundEmitter.Instance.EmitSound(position, rayDirectionCount, raySpeed, rayLifetime, SoundEmitter.SoundType.Bat, angle);
    }
}

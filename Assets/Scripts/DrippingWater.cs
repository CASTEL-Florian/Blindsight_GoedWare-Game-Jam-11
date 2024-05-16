using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// Dripping water that emits sound at regular intervals.
public class DrippingWater : MonoBehaviour
{
    [SerializeField] private float raySpeed = 2f;
    [SerializeField] private float rayLifetime = 2f;
    [SerializeField] private int rayDirectionCount = 4;
    [SerializeField] private float timeBetweenDrips = 1f;
    
    [SerializeField] private List<AudioClip> dripSounds;
    [SerializeField] private AudioSource audioSource;
    
    private float currentTime = 1f;


    private void Start()
    {
        currentTime = Random.Range(0, timeBetweenDrips);
    }

    private void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
        }
        
        if (currentTime <= 0)
        {
            float angle = Random.Range(0, 360);
            SoundEmitter.Instance.EmitSound(transform.position, rayDirectionCount, raySpeed, rayLifetime, SoundEmitter.SoundType.Drip, angle, 0, 1f, SoundEmitter.RayColor.Blue);
            audioSource.PlayOneShot(dripSounds[Random.Range(0, dripSounds.Count)]);
            currentTime = timeBetweenDrips;
        }
    }
}

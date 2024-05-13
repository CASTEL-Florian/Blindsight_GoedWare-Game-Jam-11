using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// Triggers a sound and emits sound rays from a list of sources. Used in the last level when the player hears the sound
// of the monster from far away.
public class SoundTrigger : MonoBehaviour
{
    [Serializable]
    public class SoundRaySource
    {
        public AudioSource targetSource;
        public float emissionTimestamp;
    }
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private int soundRayCount = 5;
    [SerializeField] private float soundRaySpeed = 2f;
    [SerializeField] private float soundRayLifetime = 1f;
    [SerializeField] private List<SoundRaySource> soundRaySources;

    private bool triggered;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (other.CompareTag("Player"))
        {
            triggered = true;
            TriggerSound();
        }
    }

    private void TriggerSound()
    {
        audioSource.Play();
        foreach (var source in soundRaySources)
        {
            StartCoroutine(EmitSound(source));
        }
    }

    private IEnumerator EmitSound(SoundRaySource source)
    {
        yield return new WaitForSeconds(source.emissionTimestamp);
        float angle = Random.Range(0, 360);
        SoundEmitter.Instance.EmitSound(source.targetSource.transform.position, soundRayCount, soundRaySpeed, soundRayLifetime, SoundEmitter.SoundType.Rock, angle, 0);
        source.targetSource.Play();
    }
}

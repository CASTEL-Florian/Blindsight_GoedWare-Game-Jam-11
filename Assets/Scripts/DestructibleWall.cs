using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

// Walls that can be destroyed by the player's clap.
public class DestructibleWall : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource rockfallAudioSource;
    [SerializeField] private float destroyTime = 1.35f;
    [SerializeField] private List<Transform> soundEmitters;
    [SerializeField] private float maxPlayerDistance = 2f;
    
    [SerializeField] private int soundCount = 5;
    [SerializeField] private float soundSpeed = 3f;
    [SerializeField] private float soundLifetime = 1f;
    [SerializeField] private float startAlpha = 0.5f;
    
    [SerializeField] private List<AudioClip> rockSounds;
    [SerializeField] private float soundEmissionCooldown = 1f;
    
    [SerializeField] private UnityEvent onDestroy;
    
    [SerializeField] private GameObject Colider;
    
    private float currentCooldown = 0f;
    
    
    private bool isDestroying = false;
    
    public float MaxPlayerDistance => maxPlayerDistance;

    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDestroying) return;
        
        if (other.gameObject.CompareTag("SoundRay"))
        {
            SoundRay soundRay = other.gameObject.GetComponent<SoundRay>();
            if (soundRay.RaySoundType == SoundEmitter.SoundType.PlayerClap && soundRay.DistanceTravelled < maxPlayerDistance)
            {
                isDestroying = true;
                rockfallAudioSource.Play();
                onDestroy.Invoke();
                StartCoroutine(DestroyCoroutine());
                return;
            }

            if (soundRay.RaySoundType == SoundEmitter.SoundType.Win ||
                soundRay.RaySoundType == SoundEmitter.SoundType.Death || 
                soundRay.RaySoundType == SoundEmitter.SoundType.Rock)
            {
                return;
            }
            if (currentCooldown > 0) return;
            PlayRockSound();
        }
    }
    
    private System.Collections.IEnumerator DestroyCoroutine()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
        Colider.layer = LayerMask.NameToLayer("Player");
        Shuffle(soundEmitters);
        for (int i = 0; i < soundEmitters.Count; i++)
        {
            float angle = Random.Range(0, 360);
            SoundEmitter.Instance.EmitSound(soundEmitters[i].position, soundCount, soundSpeed, soundLifetime, SoundEmitter.SoundType.Rock, angle, 0f, startAlpha);
            yield return new WaitForSeconds(destroyTime / soundEmitters.Count);
        }
        Destroy(gameObject);
        GameManager.Instance.UpdateNavMeshNextFrame();
    }

    private void PlayRockSound()
    {
        currentCooldown = soundEmissionCooldown;
        int randomIndex = Random.Range(0, rockSounds.Count - 1);
        audioSource.PlayOneShot(rockSounds[randomIndex]);
        rockSounds.Add(rockSounds[randomIndex]);
        rockSounds.RemoveAt(randomIndex);
        
        Shuffle(soundEmitters);
        for (int i = 0; i < 2; i++)
        {
            float angle = Random.Range(0, 360);
            SoundEmitter.Instance.EmitSound(soundEmitters[i].position, soundCount, soundSpeed, soundLifetime, SoundEmitter.SoundType.Rock, angle, 0f, startAlpha);
        }
    }

    private void Update()
    {
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
        }
    }
    
    private void Shuffle(List<Transform> list)
    {
        for (int i = 0; i < list.Count - 1; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }
}

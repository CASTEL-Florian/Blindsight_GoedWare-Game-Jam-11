using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    
    [SerializeField] private List<AudioClip> footstepSounds;
    [SerializeField] private AudioClip clapSound;
    [SerializeField] private AudioClip deathSound;
    
    public void PlayFootstepSound(float volumeScale = 1f)
    {
        int randomIndex = Random.Range(0, footstepSounds.Count - 1);
        audioSource.PlayOneShot(footstepSounds[randomIndex], volumeScale);
        footstepSounds.Add(footstepSounds[randomIndex]);
        footstepSounds.RemoveAt(randomIndex);
    }
    
    public void PlayClapSound()
    {
        audioSource.PlayOneShot(clapSound);
    }
    
    public void PlayDeathSound()
    {
        audioSource.PlayOneShot(deathSound);
    }
    
}

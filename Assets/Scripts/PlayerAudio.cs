using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioSource stepsAudioSource;
    
    [SerializeField] private List<AudioClip> footstepSounds;
    [SerializeField] private AudioClip clapSound;
    [SerializeField] private AudioClip deathSound;
    
    [SerializeField] private AudioClip enterWaterSound;
    [SerializeField] private List<AudioClip> waterFootstepSounds;

    [SerializeField] private AudioSource clapAudioSource;
    
    [SerializeField] private AudioSource waterAudioSource;
    
    [SerializeField] private AudioSource deathAudioSource;
    public void PlayFootstepSound(float volumeScale = 1f)
    {
        int randomIndex = Random.Range(0, footstepSounds.Count - 1);
        stepsAudioSource.PlayOneShot(footstepSounds[randomIndex], volumeScale);
        footstepSounds.Add(footstepSounds[randomIndex]);
        footstepSounds.RemoveAt(randomIndex);
    }
    
    public void PlayWaterFootstepSound(float volumeScale = 1f)
    {
        int randomIndex = Random.Range(0, waterFootstepSounds.Count - 1);
        waterAudioSource.PlayOneShot(waterFootstepSounds[randomIndex], volumeScale);
        waterFootstepSounds.Add(waterFootstepSounds[randomIndex]);
        waterFootstepSounds.RemoveAt(randomIndex);
    }
    
    public void PlayEnterWaterSound()
    {
        waterAudioSource.PlayOneShot(enterWaterSound);
    }
    
    public void PlayClapSound()
    {
        clapAudioSource.PlayOneShot(clapSound);
    }
    
    public void PlayDeathSound()
    {
        deathAudioSource.PlayOneShot(deathSound);
    }
    
}

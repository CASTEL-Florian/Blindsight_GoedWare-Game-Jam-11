using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The rockfall at the end of the boss.
public class Rockfall : MonoBehaviour
{
    [SerializeField] private AudioSource firstAudioSource;
    [SerializeField] private AudioSource secondAudioSource;
    [SerializeField] private List<SoundTrigger.SoundRaySource> soundRaySources;
    
    [SerializeField] private int soundRayCount1 = 7;
    [SerializeField] private float soundRaySpeed1 = 3f;
    [SerializeField] private float soundRayLifetime1 = 2f;
    
    [SerializeField] private int soundRayCount2 = 15;

    [SerializeField] private GameObject wall;

    public void TriggerFirstRockfall()
    {
        firstAudioSource.Play();
        foreach (var source in soundRaySources)
        {
            StartCoroutine(EmitSound(source, soundRayCount1));
        }
    }
    
    private IEnumerator EmitSound(SoundTrigger.SoundRaySource source, int soundRayCount)
    {
        yield return new WaitForSeconds(source.emissionTimestamp);
        float angle = Random.Range(0, 360);
        SoundEmitter.Instance.EmitSound(source.targetSource.transform.position, soundRayCount, soundRaySpeed1, soundRayLifetime1, SoundEmitter.SoundType.Rock, angle, 0);
        source.targetSource.Play();
    }

    public void TriggerSecondRockfall()
    {
        MyCamera.Instance.ShakeCamera(0.2f, 3f);
        wall.SetActive(true);
        secondAudioSource.Play();
        foreach (var source in soundRaySources)
        {
            StartCoroutine(EmitSound(source, soundRayCount2));
        }
    }
}

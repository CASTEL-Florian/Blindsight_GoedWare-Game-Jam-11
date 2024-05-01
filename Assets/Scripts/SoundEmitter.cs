using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    public enum SoundType
    {
        Normal,
        Death
    }
    [SerializeField] private GameObject soundRayPrefab;
    [SerializeField] private GameObject deathSoundRayPrefab;
    
    public void EmitSound(Vector3 position, int directionCount, float speed, float lifetime, SoundType soundType = SoundType.Normal, float angleOffset = 0f)
    {
        for (int i = 0; i < directionCount; i++)
        {
            float angle = angleOffset + i * 360f / directionCount;
            Vector3 direction = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad), 0);
            GameObject soundRay;
            if (soundType == SoundType.Normal)
            {
                soundRay = Instantiate(soundRayPrefab, position, Quaternion.identity);
            } 
            else
            {
                soundRay = Instantiate(deathSoundRayPrefab, position, Quaternion.identity);
            }
            soundRay.transform.right = direction;
            soundRay.GetComponent<SoundRay>().Init(speed, lifetime);
        }
    }
}

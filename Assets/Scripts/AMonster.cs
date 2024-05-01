using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class AMonster : MonoBehaviour
{
    public event Action<SoundRay> OnSoundHeard;
    public enum MonsterBehaviour
    {
        Idle,
        Flee,
        Chase,
        Sleep,
        WakeUp,
        Patrol
    }
    
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 4f;
    [SerializeField] private float smoothRotation = 0.3f;
    
    public void Move(Vector2 movement, bool running)
    {
        transform.position += new Vector3(movement.x, movement.y, 0) * (Time.deltaTime * (running ? runSpeed : walkSpeed));
        
        if (movement.magnitude < 0.1f) return;
        float angle = -Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), smoothRotation * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SoundRay"))
        {
            SoundRay soundRay = other.GetComponent<SoundRay>();
            OnSoundHeard?.Invoke(soundRay);
            ReactToSound(soundRay);
        } 
        else if (other.CompareTag("Player"))
        {
            // TODO: Attack player
        }
    }

    protected abstract void ReactToSound(SoundRay soundRay);
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


// Squirrel don't live in caves, but I didn't know how to call it. It's a small cave animal.
public class Squirrel : MonoBehaviour
{
    [SerializeField] private float raySpeed = 3f;
    [SerializeField] private float rayLifetime = 6f;
    [SerializeField] private int rayDirectionCount = 50;
    
    [SerializeField] private bool fleeWhenPlayerIsNear = true;
    [SerializeField] private float playerDetectionRadius = 5f;
    [SerializeField] private float distanceBetweenFootsteps = 0.3f;
    [SerializeField] private float footOffset = 0.07f; 
    
    [SerializeField] private GameObject leftFootPrefab;
    [SerializeField] private GameObject rightFootPrefab;
    [SerializeField] private List<Transform> targetTransforms;
    
    [SerializeField] private List<AudioClip> footstepSounds;
    [SerializeField] private List<AudioClip> squeakSounds;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioSource audioSource;
    
    [SerializeField] private float squeakRaySpeed = 3f;
    [SerializeField] private float squeakRayLifetime = 2f;
    [SerializeField] private int squeakRayDirectionCount = 11;
    
    [SerializeField] private float deathRaySpeed = 4f;
    [SerializeField] private float deathRayLifetime = 3f;
    [SerializeField] private int deathRayDirectionCount = 11;
    private NavMeshAgent navMeshAgent;
    
    public Vector3 Target { get; private set; }
    
    private bool isFleeing = false;
    private bool isDead = false;
    
    private Vector3 lastStepPosition;
    private bool useRightFoot = true;
    
    private Player player;
    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        if (isDead)
        {
            navMeshAgent.enabled = false;
            return;
        }
        if (!isFleeing)
        {
            if (Vector3.Distance(transform.position,player.transform.position) < playerDetectionRadius && fleeWhenPlayerIsNear)
            {
                Flee();
            }
            return;
        }

        if (Vector3.Distance(transform.position, lastStepPosition) > distanceBetweenFootsteps)
        {
            GameObject footPrefab = useRightFoot ? rightFootPrefab : leftFootPrefab;
            
            float angle = -Mathf.Atan2(transform.position.x - lastStepPosition.x, transform.position.y - lastStepPosition.y) * Mathf.Rad2Deg;
            Vector3 offset = Quaternion.Euler(0, 0, angle) * new Vector3(footOffset * (useRightFoot ? 1 : -1), 0, 0);
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            
            Footstep footstep = Instantiate(footPrefab, transform.position + offset, rotation).GetComponent<Footstep>();
            lastStepPosition = transform.position;
            useRightFoot = !useRightFoot;
            
            footstep.FadeOut(2f);
            
            SoundEmitter.Instance.EmitSound(transform.position + offset, rayDirectionCount, raySpeed, rayLifetime, SoundEmitter.SoundType.PlayerWalk);
            
            int randomIndex = UnityEngine.Random.Range(0, footstepSounds.Count - 1);
            audioSource.PlayOneShot(footstepSounds[randomIndex]);
            footstepSounds.Add(footstepSounds[randomIndex]);
            footstepSounds.RemoveAt(randomIndex);
            
        }
    }

    public void Flee()
    {
        isFleeing = true;

        int randomIndex = Random.Range(0, targetTransforms.Count);
        Target = targetTransforms[randomIndex].position;

        navMeshAgent.SetDestination(Target);

        PlaySqueakSound();
        
        SoundEmitter.Instance.EmitSound(transform.position, squeakRayDirectionCount, squeakRaySpeed, squeakRayLifetime);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;
        if (other.CompareTag("Monster"))
        {
            Die();
        }
    }
    
    private void PlaySqueakSound()
    {
        int randomSqueakIndex = Random.Range(0, squeakSounds.Count - 1);
        audioSource.PlayOneShot(squeakSounds[randomSqueakIndex]);
        squeakSounds.Add(squeakSounds[randomSqueakIndex]);
        squeakSounds.RemoveAt(randomSqueakIndex);
    }
    
    private void Die()
    {
        SoundEmitter.Instance.EmitSound(transform.position, deathRayDirectionCount, deathRaySpeed, deathRayLifetime, SoundEmitter.SoundType.Death);
        PlaySqueakSound();
        audioSource.PlayOneShot(deathSound);
        isDead = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }
}

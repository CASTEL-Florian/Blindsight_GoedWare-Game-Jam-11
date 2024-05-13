using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// The boss in the last level. It has 3 phases. In the first phase, it roams around first room.
// Between the first and second phase, it runs to the second room. In the second phase, to roam around the second room.
// In the third phase, it chases the player.
public class Boss : MonoBehaviour
{
    [SerializeField] private float playerDetectionRadius = 10f;

    [SerializeField] private int WakeUpRayaDirectionCount = 35;
    [SerializeField] private float WakeUpRaySpeed = 7;
    [SerializeField] private float WakeUpRaylifetime = 4;
    [SerializeField] private List<Transform> roamingPoints1;
    [SerializeField] private List<Transform> roamingPoints2;
    
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float roamSpeed = 2f;
    
    [SerializeField] private float distanceBetweenSteps = 1f;
    [SerializeField] private float footOffset = 0.5f; 
    [SerializeField] private GameObject leftFootPrefab;
    [SerializeField] private GameObject rightFootPrefab;
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> footstepSounds;
    [SerializeField] private AudioSource roarAudioSource;
    [SerializeField] private AudioSource deathAudioSource;
    
    [SerializeField] private float footStepRaySpeed = 3f;
    [SerializeField] private float footStepRayLifetime = 2f;
    [SerializeField] private int footStepRayDirectionCount = 15;
    
    [SerializeField] private float targetOvershootDistance = 2f;
    [SerializeField] private Transform checkpoint;



    [SerializeField] private Transform phase2Target;
    [SerializeField] private Transform phase3Target;

    private Player player;
    private NavMeshAgent navMeshAgent;
    
    private Vector3 lastStepPosition;
    private Vector3 lastPosition;
    private bool useRightFoot = true;
    
    private bool wakeUpStarted = false;
    private bool chaseStarted = false;
    
    
    private bool chasing = false;
    private Coroutine pauseCoroutine;

    private Vector3 target;

    private bool inAttackPause;
    private bool inRoamingPause;

    private float timeSinceLastPauseEnd;
    
    private bool inPhase2Transition = false;
    private bool inPhase2 = false;
    
    private bool inPhase3Transition = false;
    private bool inPhase3 = false;
    
    private bool dead = false;

    private const float playerEscapeTime = 2.84f;
    private void Start()
    {
        player = FindObjectOfType<Player>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
    }

    private void Update()
    {
        if (dead) return;
        timeSinceLastPauseEnd += Time.deltaTime;
        if (!wakeUpStarted && Vector3.Distance(player.transform.position, transform.position) < playerDetectionRadius)
        {
            wakeUpStarted = true;
            StartCoroutine(WakeUpCoroutine());
        }

        if (!chaseStarted) return;

        navMeshAgent.SetDestination(target);
        navMeshAgent.speed = chasing ? chaseSpeed : roamSpeed;
        
        
        if (Vector3.Distance(transform.position, lastStepPosition) > distanceBetweenSteps)
        {
            SpawnFootstep();
        }
        
        if (!inPhase3Transition && !inPhase3 && !inPhase2Transition && !inAttackPause && !inRoamingPause && timeSinceLastPauseEnd > 2 && (Vector2.Distance(transform.position, target) < 0.1f || Vector2.Distance(transform.position, lastPosition) < navMeshAgent.speed * Time.deltaTime / 10f))
        {
            if (chasing)
            {
                inAttackPause = true;
            }
            else
            {
                inRoamingPause = true;
            }
            pauseCoroutine = StartCoroutine(PauseCoroutine(chasing ? 0.1f : 2f));
        }

        if (inPhase2Transition && Vector2.Distance(transform.position, phase2Target.position) < 0.1f)
        {
            inPhase2Transition = false;
            inPhase2 = true;
            pauseCoroutine = StartCoroutine(PauseCoroutine(chasing ? 0.1f : 2f));
        }

        if (inPhase3)
        {
            target = player.transform.position;
        }
        lastPosition = transform.position;
    }

    private void SpawnFootstep()
    {
        GameObject footPrefab = useRightFoot ? rightFootPrefab : leftFootPrefab;
        float angle = -Mathf.Atan2(transform.position.x - lastStepPosition.x, transform.position.y - lastStepPosition.y) * Mathf.Rad2Deg;
        Vector3 offset = Quaternion.Euler(0, 0, angle) * new Vector3(footOffset * (useRightFoot ? 1 : -1), 0, 0);
            
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
            
        Footstep footstep = Instantiate(footPrefab, transform.position + offset, rotation).GetComponent<Footstep>();
        lastStepPosition = transform.position;
        useRightFoot = !useRightFoot;
            
        footstep.FadeOut(2f);
            
        SoundEmitter.Instance.EmitSound(transform.position + offset, footStepRayDirectionCount, footStepRaySpeed, footStepRayLifetime, SoundEmitter.SoundType.Boss);
            
        int randomIndex = UnityEngine.Random.Range(0, footstepSounds.Count - 1);
        audioSource.PlayOneShot(footstepSounds[randomIndex]);
        footstepSounds.Add(footstepSounds[randomIndex]);
        footstepSounds.RemoveAt(randomIndex);
    }
    private IEnumerator WakeUpCoroutine()
    {
        CheckpointManager.Instance.SetCheckpoint(checkpoint.position, -90);
        roarAudioSource.Play();
        Vector3 position = transform.position;
        SoundEmitter.Instance.EmitSound(position, WakeUpRayaDirectionCount, WakeUpRaySpeed, WakeUpRaylifetime, SoundEmitter.SoundType.Boss, 0f, 1f);
        yield return new WaitForSeconds(0.2f);
        MyCamera.Instance.ShakeCamera(0.2f);
        yield return new WaitForSeconds(1.5f);
        chaseStarted = true;
        target = position;
        lastStepPosition = position;
        lastPosition = position;
        Vector3 randomRoamingPoint = GetRandomRoamingPoint();
        target = randomRoamingPoint;
        timeSinceLastPauseEnd = 0;
    }
    
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!chaseStarted || dead) return;
        
        if (other.CompareTag("SoundRay"))
        {
            SoundRay soundRay = other.GetComponent<SoundRay>();
            ReactToSound(soundRay);
        } 
    }

    private void ReactToSound(SoundRay soundRay)
    {
        if (!chaseStarted || inAttackPause || chasing || inPhase2Transition || inPhase3Transition) return;
        if (soundRay.RaySoundType != SoundEmitter.SoundType.PlayerWalk &&
            soundRay.RaySoundType != SoundEmitter.SoundType.PlayerClap)
        {
            return;
        }
        float distanceFromRayOrigin = Vector3.Distance(transform.position, soundRay.Origin);
        target = (soundRay.Origin - transform.position).normalized * (distanceFromRayOrigin + targetOvershootDistance) + transform.position;
        chasing = true;
        if (pauseCoroutine != null)
        {
            StopCoroutine(pauseCoroutine);
            inRoamingPause = false;
        }
    }
    
    private IEnumerator PauseCoroutine(float duration = 2f)
    {
        yield return new WaitForSeconds(duration);
        Vector3 randomRoamingPoint;
        if (inPhase2)
        {
            randomRoamingPoint = GetRandomRoamingPoint(1);
        }
        else
        {
            randomRoamingPoint = GetRandomRoamingPoint();
        }
        target = randomRoamingPoint;
        chasing = false;
        inAttackPause = false;
        inRoamingPause = false;
        timeSinceLastPauseEnd = 0;
    }

    private Vector3 GetRandomRoamingPoint(int section = 0)
    {
        List<Transform> roamingPoints = section == 0 ? roamingPoints1 : roamingPoints2;
        int randomIndex = UnityEngine.Random.Range(0, roamingPoints.Count - 1);
        Vector3 position = roamingPoints[randomIndex].position;
        roamingPoints.Add(roamingPoints[randomIndex]);
        roamingPoints.RemoveAt(randomIndex);
        return position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }

    public void EnterPhase2()
    {
        if (pauseCoroutine != null)
        {
            StopCoroutine(pauseCoroutine);
            inRoamingPause = false;
            inAttackPause = false;
        }
        StartCoroutine(EnterPhase2Coroutine());
    }
    
    private IEnumerator EnterPhase2Coroutine()
    {
        target = transform.position;
        inPhase2Transition = true;
        chasing = true;
        roarAudioSource.Play();
        SoundEmitter.Instance.EmitSound(transform.position, WakeUpRayaDirectionCount, WakeUpRaySpeed, WakeUpRaylifetime, SoundEmitter.SoundType.Boss, 0f, 1f);
        yield return new WaitForSeconds(0.2f);
        MyCamera.Instance.ShakeCamera(0.2f);
        yield return new WaitForSeconds(0.5f);
        target = phase2Target.position;
    }
    
    public void EnterPhase3()
    {
        if (pauseCoroutine != null)
        {
            StopCoroutine(pauseCoroutine);
            inRoamingPause = false;
            inAttackPause = false;
        }
        StartCoroutine(EnterPhase3Coroutine());
    }
    
    private IEnumerator EnterPhase3Coroutine()
    {
        inPhase3Transition = true;
        target = phase3Target.position;
        navMeshAgent.SetDestination(target);
        yield return null;
        float remainingDistance = navMeshAgent.remainingDistance;
        target = transform.position;
        chasing = true;
        roarAudioSource.Play();
        SoundEmitter.Instance.EmitSound(transform.position, WakeUpRayaDirectionCount, WakeUpRaySpeed, WakeUpRaylifetime, SoundEmitter.SoundType.Boss, 0f, 1f);
        MyCamera.Instance.ShakeCamera(0.2f);
        float timeToReachTarget = remainingDistance / chaseSpeed;
        if (timeToReachTarget + 0.7f > playerEscapeTime && timeToReachTarget < playerEscapeTime)
        {
            yield return new WaitForSeconds(playerEscapeTime - timeToReachTarget);
        }
        else if (timeToReachTarget + 0.7f < playerEscapeTime)
        {
            yield return new WaitForSeconds(0.7f);
        }
        
        inPhase3Transition = false;
        inPhase3 = true;
    }

    public void KillBoss()
    {
        dead = true;
        navMeshAgent.enabled = false;
        SoundEmitter.Instance.EmitSound(transform.position, WakeUpRayaDirectionCount, WakeUpRaySpeed, WakeUpRaylifetime, SoundEmitter.SoundType.Death, 0f, 1f);
        deathAudioSource.Play();
    }
}

using UnityEngine;

// This class is used to manage the checkpoint system in the game. Each time a level is loaded, the GameManager will
// check if a checkpoint has been set. If it has, the player will spawn at the checkpoint position.
public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    public bool CheckpointSet { get; private set; }

    public Vector3 LastCheckpointPosition { get; private set; }
    public float LastCheckpointRotation { get; private set; }
    
    public bool IsLastLevelBeaten { get; set; }

    private void Awake()
    {
        if (Instance == null) { Instance = this; } else if (Instance != this) { Destroy(gameObject); }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SetCheckpoint(Vector3 position, float rotation = 0f)
    {
        LastCheckpointPosition = position;
        LastCheckpointRotation = rotation;
        CheckpointSet = true;
    }
    
    public void ResetCheckpoint()
    {
        LastCheckpointPosition = Vector3.zero;
        LastCheckpointRotation = 0f;
        CheckpointSet = false;
    }
}

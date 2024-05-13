using UnityEngine;


// This script is attached to a child object of the player. It's used to check if the player is colliding with a trigger.
public class PlayerTriggerCheck : MonoBehaviour
{
    [SerializeField] private Player player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        player.TriggerEnter(other);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        player.TriggerExit(other);
    }
}

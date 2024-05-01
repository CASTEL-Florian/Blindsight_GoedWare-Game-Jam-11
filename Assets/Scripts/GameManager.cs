using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private Fader fader;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        fader.FadeIn();
    }
    
    public void Restart()
    {
        fader.TransitionToScene(SceneManager.GetActiveScene().buildIndex);
    }
}

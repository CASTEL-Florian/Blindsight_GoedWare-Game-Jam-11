using UnityEngine;

// Class used to emit sound rays in the game. It's used whenever a sound is made.
public class SoundEmitter : MonoBehaviour
{
    public enum SoundType
    {
        PlayerWalk,
        PlayerSneak,
        PlayerClap,
        Rock,
        Death,
        Win,
        MonsterWalk,
        Drip,
        Bat,
        Boss,
        Ending
    }
    
    public enum RayColor
    {
        Default,
        White,
        Red,
        Blue,
        Green,
        Orange,
        Yellow,
        LightBlue,
        Purple
    }
    [SerializeField] private GameObject soundRayPrefab;
    [SerializeField] private GameObject deathSoundRayPrefab;
    
    public static SoundEmitter Instance;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    
    // Instead of having multiple prefabs for different sound rays, I should have used only one prefab and changed its
    // color and properties in the SoundRay script... 
    public void EmitSound(Vector3 centerPosition, int directionCount, float speed, float lifetime, SoundType soundType = SoundType.PlayerWalk, float angleOffset = 0f, float distanceFromCenter = 0f, float startAlpha = 1f, RayColor rayColor = RayColor.Default)
    {
        for (int i = 0; i < directionCount; i++)
        {
            float angle = angleOffset + i * 360f / directionCount;
            Vector3 direction = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad), 0);
            SoundRay soundRay;
            Vector3 spawnPosition = centerPosition + direction * distanceFromCenter;
            if (soundType == SoundType.PlayerWalk || soundType == SoundType.PlayerSneak || soundType == SoundType.Rock || soundType == SoundType.PlayerClap || soundType == SoundType.Bat)
            {
                soundRay = ObjectPooler.Instance.Spawn("SoundRay", spawnPosition, Quaternion.identity).GetComponent<SoundRay>();
                soundRay.objectPoolTag = "SoundRay";
            } 
            else if (soundType == SoundType.Win)
            {
                soundRay = ObjectPooler.Instance.Spawn("WinSoundRay", spawnPosition, Quaternion.identity).GetComponent<SoundRay>();
                soundRay.objectPoolTag = "WinSoundRay";
            }
            else if (soundType == SoundType.Drip)
            {
                soundRay = ObjectPooler.Instance.Spawn("WaterSoundRay", spawnPosition, Quaternion.identity).GetComponent<SoundRay>();
                soundRay.objectPoolTag = "WaterSoundRay";
            }
            else if (soundType == SoundType.Boss)
            {
                soundRay = ObjectPooler.Instance.Spawn("BossSoundRay", spawnPosition, Quaternion.identity).GetComponent<SoundRay>();
                soundRay.objectPoolTag = "BossSoundRay";
            }
            else if (soundType == SoundType.Ending)
            {
                soundRay = ObjectPooler.Instance.Spawn("SoundRay", spawnPosition, Quaternion.identity).GetComponent<SoundRay>();
                soundRay.objectPoolTag = "SoundRay";
                soundRay.gameObject.layer = LayerMask.NameToLayer("EndingSoundRay");
                soundRay.SetOrderInLayer(150);
            }
            else
            {
                soundRay = ObjectPooler.Instance.Spawn("DeathSoundRay", spawnPosition, Quaternion.identity).GetComponent<SoundRay>();
                soundRay.objectPoolTag = "DeathSoundRay";
            }
            soundRay.transform.right = direction;
            soundRay.Init(speed, lifetime, soundType, startAlpha);
            switch (rayColor)
            {
                case RayColor.White:
                    soundRay.SetColor(Color.white);
                    break;
                case RayColor.Red:
                    soundRay.SetColor(Color.red);
                    break;
                case RayColor.Blue:
                    soundRay.SetColor(Color.blue);
                    break;
                case RayColor.Green:
                    soundRay.SetColor(Color.green);
                    break;
                case RayColor.Orange:
                    soundRay.SetColor(new Color(1f, 0.5f, 0f));
                    break;
                case RayColor.Yellow:
                    soundRay.SetColor(Color.yellow);
                    break;
                case RayColor.LightBlue:
                    soundRay.SetColor(new Color(0.8f, 0.8f, 1f));
                    break;
                case RayColor.Purple:
                    soundRay.SetColor(new Color(0.5f, 0f, 0.7f));
                    break;
            }
        }
    }
}

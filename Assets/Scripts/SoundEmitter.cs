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
        Purple,
        Boss
    }
    [SerializeField] private GameObject soundWaveOutlinePrefab;
    [SerializeField] private GameObject monochromeSoundWaveOutlinePrefab;
    
    public static SoundEmitter Instance;
    public readonly SoundType[] monochromeSoundTypes = {SoundType.Death, SoundType.Boss, SoundType.Win, SoundType.Drip, SoundType.MonsterWalk};
    
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
    
    private bool IsMonoChromeSoundType(SoundType soundType)
    {
        foreach (var type in monochromeSoundTypes)
        {
            if (type == soundType)
            {
                return true;
            }
        }
        return false;
    }
    
    // Instead of having multiple prefabs for different sound rays, I should have used only one prefab and changed its
    // color and properties in the SoundRay script... 
    public void EmitSound(Vector3 centerPosition, int directionCount, float speed, float lifetime, SoundType soundType = SoundType.PlayerWalk, float angleOffset = 0f, float distanceFromCenter = 0f, float startAlpha = 1f, RayColor rayColor = RayColor.Default, float widthMultiplier = 1f)
    {
        bool isMonoChrome = IsMonoChromeSoundType(soundType);
        SoundWaveOutline soundWaveOutline;
        if (isMonoChrome)
        {
            soundWaveOutline = Instantiate(monochromeSoundWaveOutlinePrefab, centerPosition, Quaternion.identity).GetComponent<SoundWaveOutline>();
        }
        else
        {
            soundWaveOutline = Instantiate(soundWaveOutlinePrefab, centerPosition, Quaternion.identity).GetComponent<SoundWaveOutline>();
        }
        for (int i = 0; i < directionCount; i++)
        {
            float angle = angleOffset + i * 360f / directionCount;
            Vector3 direction = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad), 0);
            SoundRay soundRay;
            Vector3 spawnPosition = centerPosition + direction * distanceFromCenter;


            soundRay = ObjectPooler.Instance.Spawn("SoundRay", spawnPosition, Quaternion.identity).GetComponent<SoundRay>();
            soundRay.objectPoolTag = "SoundRay";

            
            soundRay.transform.right = direction;
            soundRay.Init(speed, lifetime, soundType, startAlpha);
            
            if (soundType == SoundType.Ending)
            {
                soundRay.gameObject.layer = LayerMask.NameToLayer("EndingSoundRay");
            }
            soundWaveOutline.AddPoint(soundRay.transform);
        }

        if (soundType == SoundType.Ending)
        {
            soundWaveOutline.SetOrderInLayer(150);
        }
        soundWaveOutline.Init(lifetime, widthMultiplier, startAlpha);
        
        switch (rayColor)
        {
            case RayColor.White:
                soundWaveOutline.SetColor(Color.white);
                break;
            case RayColor.Red:
                soundWaveOutline.SetColor(Color.red);
                break;
            case RayColor.Blue:
                soundWaveOutline.SetColor(Color.blue);
                break;
            case RayColor.Green:
                soundWaveOutline.SetColor(Color.green);
                break;
            case RayColor.Orange:
                soundWaveOutline.SetColor(new Color(1f, 0.5f, 0f));
                break;
            case RayColor.Yellow:
                soundWaveOutline.SetColor(Color.yellow);
                break;
            case RayColor.LightBlue:
                soundWaveOutline.SetColor(new Color(0.8f, 0.8f, 1f));
                break;
            case RayColor.Purple:
                soundWaveOutline.SetColor(new Color(0.5f, 0f, 0.7f));
                break;
            case RayColor.Boss:
                soundWaveOutline.SetColor(new Color(0.5294118f, 0.08627451f, 0f));
                break;
        }
    }
}

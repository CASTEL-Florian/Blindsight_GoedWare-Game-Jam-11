using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// What better way to end than listening to some Beethoven!
public class Piano : MonoBehaviour
{
    [Serializable]
    public class SongData
    {
        public List<ChannelData> channels;
    }
    
    [Serializable]
    public class ChannelData
    {
        public List<TickData> ticks;
    }
    
    [Serializable]
    public class TickData
    {
        public List<int> pitches;
    }
    
    [Serializable]
    public class ChannelRayData
    {
        public int directionCount;
        public float speed;
        public float lifetime;
        public SoundEmitter.RayColor color;
    }

    [Serializable]
    public class ColorRange
    {
        public float min;
        public float max;
        public SoundEmitter.RayColor color;
    }
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private TextAsset pitchData; // JSON file with the pitch data, created with a Python script.
    [SerializeField] private float pianoLength = 10f;
    [SerializeField] private float tempo = 140f;
    [SerializeField] private int ticksPerBeat = 4;
    [SerializeField] private List<ChannelRayData> channelRayData;
    
    [SerializeField] private List<Squirrel> squirrels;
    
    [SerializeField] private bool useColorRange = false;
    [SerializeField] private List<ColorRange> ranges;
    
    private float timePerTick;
    private float currentTime = 0f;
    private SongData songData;
    private Vector2Int minMaxPitch;
    
    private bool songPlaying = false;
    private bool endTriggered = false;

    private void Start()
    {
        songData = JsonUtility.FromJson<SongData>(pitchData.text);
        minMaxPitch = ComputeMinMaxPitch(songData);
        
        timePerTick = 60f / tempo / ticksPerBeat;
    }

    private void Update()
    {
        if (audioSource.time > 0)
        {
            songPlaying = true;
        }
        if (audioSource.time - currentTime > timePerTick)
        {
            CreateSoundRays(audioSource.time, currentTime);
            currentTime = audioSource.time;
        }
        if (songPlaying && audioSource.time == 0 && !endTriggered)
        {
            StartCoroutine(PostMusicCoroutine());
        }
    }

    private void CreateSoundRays(float time, float lastTime)
    {
        for (int i = 0; i < songData.channels.Count; i++)
        {
            List<int> pitches = GetPitchesAtTime(time, lastTime, i);
            foreach (int pitch in pitches)
            {
                float pitchNormalized = (float)(pitch - minMaxPitch.x) / (minMaxPitch.y - minMaxPitch.x);
                float x = pitchNormalized * pianoLength;
                float y = 0f;
                Vector3 position = transform.position + new Vector3(x, y, 0f);
                ChannelRayData channelData = channelRayData[i];
                SoundEmitter.RayColor color = channelData.color;
                if (useColorRange)
                {
                    foreach (ColorRange range in ranges)
                    {
                        if (pitchNormalized >= range.min && pitchNormalized <= range.max)
                        {
                            color = range.color;
                            break;
                        }
                    }
                }
                SoundEmitter.Instance.EmitSound(position, channelData.directionCount, channelData.speed,
                    channelData.lifetime, rayColor:color);
            }
        }
    }
    
    private List<int> GetPitchesAtTime(float time, float lastTime, int channelIndex)
    {
        int lastTickIndex = Mathf.FloorToInt(lastTime / timePerTick);
        int tickIndex = Mathf.FloorToInt(time / timePerTick);
        
        List<int> pitches = new List<int>();
        for (int i = lastTickIndex + 1; i <= tickIndex; i++)
        {
            ChannelData channel = songData.channels[channelIndex];
            if (i >= channel.ticks.Count) continue;
            TickData tick = channel.ticks[i];
            pitches.AddRange(tick.pitches);
        }
        return pitches;
    }

    private Vector2Int ComputeMinMaxPitch(SongData songData)
    {
        int minPitch = int.MaxValue;
        int maxPitch = int.MinValue;
        foreach (ChannelData channel in songData.channels)
        {
            foreach (TickData tick in channel.ticks)
            {
                foreach (int pitch in tick.pitches)
                {
                    minPitch = Mathf.Min(minPitch, pitch);
                    maxPitch = Mathf.Max(maxPitch, pitch);
                }
            }
        }
        return new Vector2Int(minPitch, maxPitch);
    }

    private IEnumerator PostMusicCoroutine()
    {
        endTriggered = true;
        yield return new WaitForSeconds(2);

        for (int i = 0; i < squirrels.Count; i++)
        {
            squirrels[i].Flee();
            yield return new WaitForSeconds(0.3f);
        }
    }

    public void StartPlaying()
    {
        audioSource.Play();
        
        
        
        for (int i = 0; i < songData.channels.Count; i++)
        {
            ChannelData channel = songData.channels[i];
            TickData tick = channel.ticks[0];
            List<int> pitches = tick.pitches;
            foreach (int pitch in pitches)
            {
                float pitchNormalized = (float)(pitch - minMaxPitch.x) / (minMaxPitch.y - minMaxPitch.x);
                float x = pitchNormalized * pianoLength;
                float y = 0f;
                Vector3 position = transform.position + new Vector3(x, y, 0f);
                ChannelRayData channelData = channelRayData[i];
                
                SoundEmitter.RayColor color = channelData.color;
                if (useColorRange)
                {
                    foreach (ColorRange range in ranges)
                    {
                        if (pitchNormalized >= range.min && pitchNormalized <= range.max)
                        {
                            color = range.color;
                            break;
                        }
                    }
                }
                SoundEmitter.Instance.EmitSound(position, channelData.directionCount, channelData.speed,
                    channelData.lifetime, rayColor:color);
            }
        }
    }
}

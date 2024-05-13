using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundRay : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private List<TrailRenderer> trailRenderers;
    [SerializeField] private AnimationCurve alphaCurve;
    
    public float DistanceTravelled { get; private set; }
    public Vector3 Origin { get; private set; }
    
    public float StartTime { get; private set; }
    
    public SoundEmitter.SoundType RaySoundType { get; private set; }
    
    public string objectPoolTag;

    private float startAlpha = 1f;

    private Color mainColor = Color.white;

    public void Init(float speed, float lifetime, SoundEmitter.SoundType soundType = SoundEmitter.SoundType.PlayerWalk, float startAlpha = 1f)
    {
        SetColor(Color.white);
        this.startAlpha = startAlpha;
        DistanceTravelled = 0;
        Origin = transform.position;
        StartTime = Time.time;
        RaySoundType = soundType;
        rb.velocity = transform.right * speed;
        StartCoroutine(FadeOut(lifetime));
        foreach (var trailRenderer in trailRenderers)
        {
            trailRenderer.Clear();
        }
    }
    
    public void SetColor(Color color)
    {
        mainColor = color;
        foreach (var renderer in trailRenderers)
        {
            renderer.material.color = color;
        }
    }
    
    public void SetOrderInLayer(int order)
    {
        foreach (var renderer in trailRenderers)
        {
            renderer.sortingOrder = order;
        }
    }


    private IEnumerator FadeOut(float lifetime)
    {
        float startTime = Time.time;
        while (Time.time - startTime < lifetime)
        {
            float alpha = alphaCurve.Evaluate((Time.time - startTime) / lifetime) * startAlpha;
            foreach (var renderer in trailRenderers)
            {
                Color col = mainColor;
                col.a = alpha;
                renderer.material.color = col;
            }
            yield return null;
        }
        ObjectPooler.Instance.ReturnObject(objectPoolTag, gameObject);
    }

    private void Update()
    {
        DistanceTravelled += rb.velocity.magnitude * Time.deltaTime;
    }
}

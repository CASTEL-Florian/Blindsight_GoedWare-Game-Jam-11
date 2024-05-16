using System.Collections;
using TMPro;
using UnityEngine;

// This class is used in the first level with the destructible walls. If the player doesn't figure out by themselves
// that they can clap to destroy the walls, this class will show a tip after a certain amount of time.
public class ClapTip : MonoBehaviour
{
    [SerializeField] private float timeBeforeTip = 30f;
    [SerializeField] private TextMeshProUGUI tipText;
    [SerializeField] private SpriteRenderer tipImage;
    [SerializeField] private float tipFadeTime = 3f;
    
    private float timeSinceStart = 0f;
    private bool fadeStarted;
    private bool tipCancelled;
    
    private void Update()
    {
        if (fadeStarted || tipCancelled) return;
        timeSinceStart += Time.deltaTime;
        if (timeSinceStart > timeBeforeTip)
        {
            fadeStarted = true;
            StartCoroutine(FadeInTip());
        }
    }
    
    public void CancelTip()
    {
        tipCancelled = true;
        StopAllCoroutines();
        StartCoroutine(FadeOutTip());
    }
    
    private IEnumerator FadeInTip()
    {
        float elapsedTime = 0f;
        while (elapsedTime < tipFadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = elapsedTime / tipFadeTime;
            tipText.color = new Color(tipText.color.r, tipText.color.g, tipText.color.b, alpha);
            tipImage.color = new Color(tipImage.color.r, tipImage.color.g, tipImage.color.b, alpha);
            yield return null;
        }
    }
    
    private IEnumerator FadeOutTip()
    {
        float elapsedTime = 0f;
        float startAlpha = tipText.color.a;
        while (elapsedTime < tipFadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = (1 - elapsedTime / tipFadeTime) * startAlpha;
            tipText.color = new Color(tipText.color.r, tipText.color.g, tipText.color.b, alpha);
            tipImage.color = new Color(tipImage.color.r, tipImage.color.g, tipImage.color.b, alpha);
            yield return null;
        }
    }
}

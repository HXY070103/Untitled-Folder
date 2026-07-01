using System.Collections;
using UnityEngine;

public class PlayerVFX : EntityVFX {
    [Header("Phase VFX info")]
    [SerializeField] private float targetAlpha = 0.2f;
    [SerializeField] private float fadeSpeed = 2;
    [SerializeField] private float fadeInterval = 0.05f;

    private Coroutine fadeCo;

    public void ResetTransparency() {
        Color newColor = sr.color;
        newColor.a = 1;
        sr.color = newColor;
    }

    public void DoFadeEffect() {
        if (fadeCo != null) {
            StopCoroutine(fadeCo);
        }

        fadeCo = StartCoroutine(FadeEffectCo());
    }

    private IEnumerator FadeEffectCo() {
        Color colorTracker = sr.color;
        float alphaDifference = Mathf.Abs(colorTracker.a - targetAlpha);
        
        while (alphaDifference > 0.01f) {
            colorTracker.a -= fadeSpeed * fadeInterval;
            sr.color = colorTracker;
            alphaDifference = Mathf.Abs(colorTracker.a- targetAlpha);
            yield return new WaitForSeconds(fadeInterval);
        }

        colorTracker.a = targetAlpha;
        sr.color = colorTracker;
    }
}

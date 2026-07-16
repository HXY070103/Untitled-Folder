using System.Collections;
using UnityEngine;

public class PlayerVFX : EntityVFX {
    [Header("Phase VFX info")]
    [SerializeField] private float targetAlpha = 0.2f;
    [SerializeField] private float fadeSpeed = 2;
    [SerializeField] private float fadeInterval = 0.05f;

    private Coroutine fadeCo;

    [Header("Global pause info")]
    [SerializeField] private float globalPauseScale = 0.08f;
    [SerializeField] private float globalPauseDuration = 0.05f;

    private Coroutine globalPauseCo;
    private float originalTimeScale;
    private float originalFixedDeltaTime;

    protected override void Awake() {
        base.Awake();

        originalTimeScale = Time.timeScale;
        originalFixedDeltaTime = Time.fixedDeltaTime;
    }

    public override void PlayOnDamageVFX() {
        base.PlayOnDamageVFX();

        DoGlobalPause();
    }

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
    
    public void DoGlobalPause() {
        if (globalPauseCo != null) {
            StopCoroutine(globalPauseCo);
        }

        globalPauseCo = StartCoroutine(GlobalPauseCo());
    }

    private IEnumerator GlobalPauseCo() {
        if (Time.timeScale <= 0f) {
            yield break;
        }

        Time.timeScale = globalPauseScale;
        Time.fixedDeltaTime = originalFixedDeltaTime * globalPauseScale;

        yield return new WaitForSecondsRealtime(globalPauseDuration);

        Time.timeScale = originalTimeScale;
        Time.fixedDeltaTime = originalFixedDeltaTime;
    }
}

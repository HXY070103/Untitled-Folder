using System.Collections;
using UnityEngine;

public class PlayerVFX : EntityVFX {
    [Header("Particle effect info")]
    [SerializeField] private GameObject particleEffectPrefab;

    [Header("Phase VFX info")]
    [SerializeField] private float targetAlpha = 0.2f;
    [SerializeField] private float fadeSpeed = 2;
    [SerializeField] private float fadeInterval = 0.05f;

    private Coroutine fadeCo;

    public void GenerateParticleEffect(Color color){
        GameObject particleEffect = 
            Instantiate(particleEffectPrefab, transform.position, Quaternion.identity);
        ParticleSystem ps = particleEffect.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule main = ps.main;

        main.startColor = color; 
        ps.Play();

        float destroyDelay = main.duration + main.startLifetime.constantMax;
        Destroy(particleEffect, destroyDelay);
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
}

using System.Collections;
using UnityEngine;

public class EntityVFX : MonoBehaviour {
    protected SpriteRenderer sr;
    private Entity entity;

    [Header("Particle effect info")]
    [SerializeField] private GameObject particleEffectPrefab;

    [Header("Image echo VFX info")]
    [Range(0.01f, 0.20f)]
    [SerializeField] private float imageEchoInterval = 0.05f;
    [SerializeField] private GameObject imageEchoVFXPrefab;

    private Coroutine imageEcho;

    [Header("On Taking damage VFX")]
    [SerializeField] private Material onDamageVFXMaterial;
    [SerializeField] private float onDamageVFXDurarion = 0.2f;

    private Material originalMaterial; 
    private Coroutine onDamageVFXCo;

    [Header("On doing damage VFX")]
    [SerializeField] Color onHitVFXColor = Color.white;
    [SerializeField] GameObject onHitVFX;
    [SerializeField] GameObject onCritHitVFX;

    protected virtual void Awake() {
        sr = GetComponentInChildren<SpriteRenderer>();
        entity = GetComponent<Entity>();
        originalMaterial = sr.material;
    }

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

    public void DoImageEchoEffect(float duration) {
        if (imageEcho != null) {
            StopCoroutine(imageEcho);
        }

        imageEcho = StartCoroutine(ImageEchoEffectCo(duration));
    }

    public void StopImageEchoEffect() {
        if (imageEcho != null) {
            StopCoroutine(imageEcho);
        }
    }

    private IEnumerator ImageEchoEffectCo(float duration) {
        float timeTracker = 0;
        
        while (timeTracker < duration) {
            CreateImageEcho();
            yield return new WaitForSeconds(imageEchoInterval);
            timeTracker += imageEchoInterval;
        }
    }

    private void CreateImageEcho() {
        Vector3 position = entity.anim.transform.position;
        float scale = entity.anim.transform.localScale.x * entity.transform.localScale.x;

        GameObject imageEcho = Instantiate(imageEchoVFXPrefab, position, transform.rotation);

        imageEcho.transform.localScale = new Vector3(scale, scale, scale);
        imageEcho.GetComponentInChildren<SpriteRenderer>().sprite = sr.sprite;
        imageEcho.GetComponentInChildren<SpriteRenderer>().flipX = sr.flipX;
    }

    public void CreateOnHitVFX(Transform target, bool isCrit) {
        GameObject vfxPrefab = isCrit ? onCritHitVFX : onHitVFX;
        GameObject vfx = Instantiate(vfxPrefab, target.position, Quaternion.identity);
        vfx.GetComponentInChildren<SpriteRenderer>().color = onHitVFXColor;

        if (entity.facingDir == -1 && isCrit) {
            vfx.transform.Rotate(0, 180, 0);
        }
    }

    public virtual void PlayOnDamageVFX() {
        if (onDamageVFXCo != null) {
            StopCoroutine(onDamageVFXCo);
        }

        onDamageVFXCo = StartCoroutine(OnDamageVFXCo());
    }

    private IEnumerator OnDamageVFXCo() {
        sr.material = onDamageVFXMaterial;
        yield return new WaitForSeconds(onDamageVFXDurarion);
        sr.material = originalMaterial;
    }
}
using System.Collections;
using UnityEngine;

public class EntityVFX : MonoBehaviour {
    protected SpriteRenderer sr;
    private Entity entity;

    [Header("On Taking damage VFX")]
    [SerializeField] private Material onDamageVFXMaterial;
    [SerializeField] private float onDamageVFXDurarion = 0.2f;

    private Material originalMaterial; 
    private Coroutine onDamageVFXCo;

    [Header("On doing damage VFX")]
    [SerializeField] Color onHitVFXColor = Color.white;
    [SerializeField] GameObject onHitVFX;
    [SerializeField] GameObject onCritHitVFX;

    private void Awake() {
        sr = GetComponentInChildren<SpriteRenderer>();
        entity = GetComponent<Entity>();
        originalMaterial = sr.material;
    }

    public void CreateOnHitVFX(Transform target, bool isCrit) {
        GameObject vfxPrefab = isCrit ? onCritHitVFX : onHitVFX;
        GameObject vfx = Instantiate(vfxPrefab, target.position, Quaternion.identity);
        vfx.GetComponentInChildren<SpriteRenderer>().color = onHitVFXColor;

        if (entity.facingDir == -1 && isCrit) {
            vfx.transform.Rotate(0, 180, 0);
        }
    }

    public void PlayOnDamageVFX() {
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
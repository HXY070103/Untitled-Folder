using System.Collections;
using UnityEngine;

public class ArenaDynamicPlatform : MonoBehaviour {
    [SerializeField] private SpriteRenderer platformRenderer;
    [SerializeField] private Collider2D platformCollider;
    [SerializeField] private GameObject warningVisual;

    private Coroutine transitionCo;

    private void Awake() {
        if (platformRenderer == null) platformRenderer = GetComponent<SpriteRenderer>();
        if (platformCollider == null) platformCollider = GetComponent<Collider2D>();
        SetAvailable(true);
    }

    public void Disappear(float warningDuration) {
        if (transitionCo != null) StopCoroutine(transitionCo);
        transitionCo = StartCoroutine(DisappearCo(warningDuration));
    }

    public void Restore() {
        if (transitionCo != null) StopCoroutine(transitionCo);
        SetAvailable(true);
    }

    private IEnumerator DisappearCo(float warningDuration) {
        if (warningVisual != null) warningVisual.SetActive(true);
        yield return new WaitForSeconds(warningDuration);
        SetAvailable(false);
    }

    private void SetAvailable(bool available) {
        if (platformRenderer != null) platformRenderer.enabled = available;
        if (platformCollider != null) platformCollider.enabled = available;
        if (warningVisual != null) warningVisual.SetActive(false);
    }
}

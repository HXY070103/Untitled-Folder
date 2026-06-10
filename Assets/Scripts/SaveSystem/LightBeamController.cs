using System.Collections;
using UnityEngine;

public class LightBeamController : MonoBehaviour {
    private SpriteRenderer sr;

    [SerializeField] private float fadeSpeed = 1;
    [SerializeField] private float targetAlpha = 0.6f;
    [SerializeField] private float destroyDelay = 2;

    private void Awake() {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start() {
        StartCoroutine(FadeInCo());
        Destroy(gameObject, destroyDelay);
    }

    private IEnumerator FadeInCo() {
        Color colorTracker = Color.white;
        colorTracker.a = 0;

        while (colorTracker.a < targetAlpha) {
            colorTracker.a += fadeSpeed * Time.deltaTime;
            sr.color = colorTracker;
            yield return null;
        }

        sr.color = colorTracker;
    }
}

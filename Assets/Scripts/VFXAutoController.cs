using System.Collections;
using UnityEngine;

public class VFXAutoController : MonoBehaviour {
    private SpriteRenderer sr;

    [Header("Fade effect info")]
    [SerializeField] private bool canFade = false;
    [SerializeField] private float fadeSpeed = 1;

    [Header("Destroy info")]
    [SerializeField] private bool autoDestroy = true;
    [SerializeField] private float destroyDelay = 1;

    [Header("Random offset info")]
    [SerializeField] private bool randomOffset = true;
    [Space]
    [SerializeField] private float xMinOffset = -0.5f;
    [SerializeField] private float xMaxOffset = 0.5f;
    [Space]
    [SerializeField] private float yMinOffset = -0.5f;
    [SerializeField] private float yMaxOffset = 0.5f;

    [Header("Random rotation info")]
    [SerializeField] private bool randomRotation = true;
    [Space]
    [SerializeField] private float zMinRotation = -180;
    [SerializeField] private float zMaxRotation = 180;

    private void Awake() {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start() {
        if (canFade) {
            StartCoroutine(FadeCo());
        }

        ApplyRandomOffset();
        ApplyRandomRotation();

        if (autoDestroy) {
            Destroy(gameObject, destroyDelay);
        }
    }

    private IEnumerator FadeCo() {
        Color targetColor = Color.white;

        while (targetColor.a > 0) {
            targetColor.a -= fadeSpeed * Time.deltaTime;
            sr.color = targetColor;
            yield return null;
        }

        sr.color = targetColor;
    }

    private void ApplyRandomOffset() {
        if (!randomOffset) {
            return;
        }

        float xOffset = Random.Range(xMinOffset, xMaxOffset);
        float yOffset = Random.Range(yMinOffset, yMaxOffset);
        transform.position += new Vector3(xOffset, yOffset, 0);
    }

    private void ApplyRandomRotation() {
        if (!randomRotation) {
            return;
        }

        float zRotation = Random.Range(zMinRotation, zMaxRotation);
        transform.Rotate(0, 0, zRotation);
    }
}

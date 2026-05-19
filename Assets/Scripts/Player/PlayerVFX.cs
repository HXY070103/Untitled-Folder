using System.Collections;
using UnityEngine;

public class PlayerVFX : EntityVFX {
    [Header("Image echo VFX info")]
    [Range(0.01f, 0.20f)]
    [SerializeField] private float imageEchoInterval = 0.05f;
    [SerializeField] private GameObject imageEchoVFX;

    private Coroutine imageEcho;

    public void DoImageEchoEffect(float duration) {
        if (imageEcho != null) {
            StopCoroutine(imageEcho);
        }

        imageEcho = StartCoroutine(ImageEchoEffectCo(duration));
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
        GameObject imageEcho = Instantiate(imageEchoVFX, transform.position, transform.rotation);
        imageEcho.GetComponentInChildren<SpriteRenderer>().sprite = sr.sprite;
    }
}

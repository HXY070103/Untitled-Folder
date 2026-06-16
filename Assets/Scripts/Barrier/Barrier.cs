using System.Collections;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Collider2D barrierCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Fade")]
    [SerializeField] private bool isFade;
    [SerializeField] private float fadeDuration = 0.5f;

    private Coroutine fadeCoroutine;

    public void OpenBarrier()
    {
        if (barrierCollider != null)
        {
            barrierCollider.enabled = false;
        }

        if (isFade && spriteRenderer != null)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = StartCoroutine(FadeOutCo());
        } else if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
    }

    public void CloseBarrier()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        if (barrierCollider != null)
        {
            barrierCollider.enabled = true;
        }  
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 0.35f;
            spriteRenderer.color = color;
        }
    }

    private IEnumerator FadeOutCo()
    {
        Color startColor = spriteRenderer.color;
        float startAlpha = startColor.a;
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            Color color = spriteRenderer.color;
            color.a = Mathf.Lerp(startAlpha, 0f, timer / fadeDuration);
            spriteRenderer.color = color;
            yield return null;
        }
        spriteRenderer.enabled = false;
    }
}
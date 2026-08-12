using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Ingame : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private RectTransform healthRect;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Shield UI")]
    [SerializeField] private CanvasGroup shieldCanvasGroup;
    [SerializeField] private RectTransform permanentShieldFill;
    [SerializeField] private RectTransform temporaryShieldFill;
    [SerializeField] private float shieldFadeInDuration = 0.15f;
    [SerializeField] private float shieldFadeOutDuration = 0.25f;

    private Coroutine shieldFadeCoroutine;
    private bool shieldVisibilityInitialized;
    private bool shieldShouldBeVisible;
    private bool subscribedToPlayerHealth;

    private void Start()
    {
        player.health.OnHealthUpdate += UpdateHealthBar;
        player.health.ShieldManager.OnShieldChanged += UpdateHealthBar;
        subscribedToPlayerHealth = true;
        UpdateHealthBar();
    }

    private void OnDestroy()
    {
        if (!subscribedToPlayerHealth || player == null || player.health == null)
        {
            return;
        }

        player.health.OnHealthUpdate -= UpdateHealthBar;
        player.health.ShieldManager.OnShieldChanged -= UpdateHealthBar;
    }

    private void UpdateHealthBar()
    {
        float rawMaxHealth = Mathf.Max(player.stats.GetMaxHP(), 0.01f);
        int maxHealth = Mathf.Max(Mathf.RoundToInt(rawMaxHealth), 1);
        int currentHealth = Mathf.RoundToInt(player.health.GetCurrentHP());
        float sizeDiffrence = Mathf.Abs(maxHealth - healthRect.sizeDelta.x);

        if (sizeDiffrence > 0.1f) {
            healthRect.sizeDelta = new Vector2(400 * Mathf.Log10(maxHealth / 10f), healthRect.sizeDelta.y);
        }

        UpdateShieldBar(rawMaxHealth);

        int shieldValue = Mathf.CeilToInt(
            Mathf.Max(player.health.ShieldManager.Shield, 0)
        );
        healthText.text = shieldValue > 0
            ? $"{currentHealth} / {maxHealth}  +{shieldValue}"
            : $"{currentHealth} / {maxHealth}";
        healthSlider.value = player.health.GetHPPercentage();
    }

    private void UpdateShieldBar(float maxHealth)
    {
        ShieldManager shieldManager = player.health.ShieldManager;
        float permanentFraction = Mathf.Clamp01(shieldManager.PermanentShield / maxHealth);
        float temporaryFraction = Mathf.Clamp(
            shieldManager.TemporaryShield / maxHealth,
            0,
            1 - permanentFraction
        );

        SetHorizontalFill(permanentShieldFill, 0, permanentFraction);
        SetHorizontalFill(
            temporaryShieldFill,
            permanentFraction,
            permanentFraction + temporaryFraction
        );
        UpdateShieldVisibility(shieldManager.HasShield);
    }

    private void SetHorizontalFill(RectTransform fill, float start, float end)
    {
        if (fill == null)
        {
            return;
        }

        fill.anchorMin = new Vector2(start, 0);
        fill.anchorMax = new Vector2(end, 1);
        fill.offsetMin = Vector2.zero;
        fill.offsetMax = Vector2.zero;
    }

    private void UpdateShieldVisibility(bool hasShield)
    {
        if (shieldCanvasGroup == null)
        {
            return;
        }

        //还未初始化的时候
        if (!shieldVisibilityInitialized)  
        {
            shieldVisibilityInitialized = true;
            shieldShouldBeVisible = hasShield;
            shieldCanvasGroup.alpha = hasShield ? 1 : 0;
            shieldCanvasGroup.interactable = false;
            shieldCanvasGroup.blocksRaycasts = false;
            shieldCanvasGroup.gameObject.SetActive(hasShield);
            return;
        }

        //已经完成显示的同步
        if (shieldShouldBeVisible == hasShield)
        {
            return;
        }

        shieldShouldBeVisible = hasShield;

        if (shieldFadeCoroutine != null)
        {
            StopCoroutine(shieldFadeCoroutine);
        }

        if (hasShield)
        {
            shieldCanvasGroup.gameObject.SetActive(true);
        }

        float targetAlpha = hasShield ? 1 : 0;
        float duration = hasShield
            ? shieldFadeInDuration
            : shieldFadeOutDuration;
        shieldFadeCoroutine = StartCoroutine(
            FadeShield(targetAlpha, duration)
        );
    }

    private IEnumerator FadeShield(float targetAlpha, float duration)
    {
        float startAlpha = shieldCanvasGroup.alpha;
        float elapsedTime = 0;
        duration = Mathf.Max(duration, 0);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            shieldCanvasGroup.alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                elapsedTime / duration
            );
            yield return null;
        }

        shieldCanvasGroup.alpha = targetAlpha;
        shieldFadeCoroutine = null;

        if (targetAlpha <= 0 && !shieldShouldBeVisible)
        {
            shieldCanvasGroup.gameObject.SetActive(false);
        }
    }
}

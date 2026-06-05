using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Ingame : MonoBehaviour
{
    private Player player;
    [SerializeField] private RectTransform healthRect;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;

    private void Start()
    {
        player = FindFirstObjectByType<Player>();
        player.health.OnHealthUpdate += UpdateHealthBar;
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        float maxHealth = Mathf.RoundToInt(player.stats.GetMaxHP());
        float currentHealth = Mathf.RoundToInt(player.health.GetCurrentHP());
        float sizeDiffrence = Mathf.Abs(maxHealth - healthRect.sizeDelta.x);

        if (sizeDiffrence > 0.1f) {
            healthRect.sizeDelta = new Vector2(400 * Mathf.Log10(maxHealth / 10), healthRect.sizeDelta.y);
        }

        healthText.text = currentHealth + " / " + maxHealth;
        healthSlider.value = player.health.GetHPPercentage();
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class UI_DashCooldown : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Player player;

    [Header("UI")]
    [SerializeField] private Image cooldownMask;
    [SerializeField] private TextMeshProUGUI keyText;

    [Header("Key")]
    [SerializeField] private string keyname = "K";

    private void Start()
    {
        player = Player.instance;
        if (keyText != null)
        {
            keyText.text = keyname;
        }
        if (cooldownMask != null)
        {
            cooldownMask.fillAmount = 0f;
        }
    }

    private void Update()
    {
        UpdateCooldownUI();
    }

    private void UpdateCooldownUI()
    {
        if (player == null)
        {
            player = Player.instance;
        }
        if (player == null)
        {
            return;
        }

        float currentCooldown = player.GetReaminCooldown();
        float maxCooldown = player.dashCooldown;


        if (currentCooldown <= 0f || maxCooldown <= 0f)
        {
            cooldownMask.fillAmount = 0f;
            return;
        }

        cooldownMask.fillAmount = currentCooldown / maxCooldown;
    }
}
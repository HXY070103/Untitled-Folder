using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class UI_SkillCooldown : MonoBehaviour
{
    [Header("Skill")]
    [SerializeField] private PlayerSkillManager playerSkill;
    [SerializeField] private SkillBase skill;

    [Header("UI")]
    [SerializeField] private Image cooldownMask;
    [SerializeField] private TextMeshProUGUI keyText;

    [Header("Key")]
    [SerializeField] private string keyname = "Q";

    private void Start()
    {
        skill = playerSkill.GetCurrentSkill();
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
        if (skill == null || cooldownMask == null)
        {
            return;
        }

        float currentCooldown = skill.GetCurrentCooldown();
        float maxCooldown = skill.GetMaxCooldown();

        if (currentCooldown <= 0f || maxCooldown <= 0f)
        {
            cooldownMask.fillAmount = 0f;
            return;
        }

        cooldownMask.fillAmount = currentCooldown / maxCooldown;
    }
}
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SkillDuration : MonoBehaviour
{
    [SerializeField] private PlayerSkillManager manager;
    private SkillBase skill;

    [Header("UI")]
    [SerializeField] private GameObject durationGroup;
    [SerializeField] private TextMeshProUGUI durationText;
    [SerializeField] private Image durationFillImage;

    private void Update()
    {
        skill = manager.GetCurrentSkill();
        if (skill.IsSkillActive)
        {
            durationGroup.SetActive(true);
            durationText.text = skill.RemainingDuration.ToString("F1") + "s";
            float fillAmount = skill.RemainingDuration / skill.GetSkillDuration();
            durationFillImage.fillAmount = fillAmount;
        }
        else
        {
            durationGroup.SetActive(false);
            durationFillImage.fillAmount = 0;
        }
    }
}

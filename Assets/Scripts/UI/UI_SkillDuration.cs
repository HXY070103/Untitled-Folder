using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SkillDuration : MonoBehaviour
{
    [SerializeField] private SkillBase skill;

    [Header("UI")]
    [SerializeField] private GameObject durationGroup;
    [SerializeField] private TextMeshProUGUI durationText;
    [SerializeField] private Image durationFillImage;

    private void Update()
    {
        if (skill.IsSkillActive)
        {
            durationGroup.SetActive(true);
            durationText.text = skill.RemainingDuration.ToString("F1") + "s";
            float fillAmount = skill.RemainingDuration / skill.SkillDuration;
            durationFillImage.fillAmount = fillAmount;
        }
        else
        {
            durationGroup.SetActive(false);
            durationFillImage.fillAmount = 0;
        }
    }
}

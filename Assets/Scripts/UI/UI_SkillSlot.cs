using UnityEngine;
using TMPro;
using System.Data;

public class UI_SkillSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Parameter1Name;
    [SerializeField] private TextMeshProUGUI Parameter1Value;
    [SerializeField] private TextMeshProUGUI Parameter2Name;
    [SerializeField] private TextMeshProUGUI Parameter2Value;
    [SerializeField] private TextMeshProUGUI skillDuration;
    [SerializeField] private PlayerSkillManager skillManager;

    private SkillBase currentSkill;

    private void Awake() 
    {
        currentSkill = skillManager.GetCurrentSkill();
    }

    private void Start() 
    {
        UpdateSkillInfo();
    }

    public void UpdateSkillInfo()
    {
        if (currentSkill == null) return;
        skillDuration.text = $"{currentSkill.GetSkillDuration():F1}s";
        if (currentSkill.SkillType == SkillType.LifeSteal) 
        {
            Parameter1Name.text = "Conversion Rate";
            SkillLifeSteal lifeStealSkill = currentSkill as SkillLifeSteal;
            Parameter1Value.text = $"{lifeStealSkill.GetConversionRate() * 100:F1}%";
            Parameter2Name.text = "";
            Parameter2Value.text = "";
        } 
        else if (currentSkill.SkillType == SkillType.TimeControl) 
        {
            Parameter1Name.text = "Slowdown Percentage";
            SkillTimeControl timeControlSkill = currentSkill as SkillTimeControl;
            Parameter1Value.text = $"{timeControlSkill.GetSlowDownPercentage() * 100:F1}%";
            Parameter2Name.text = "Domain Max Size";
            Parameter2Value.text = $"{timeControlSkill.GetMaxSize():F1} units";
        } 
        else if (currentSkill.SkillType == SkillType.Phase) 
        {
            Parameter1Name.text = "Heavy Attack Rate";
            SkillPhase phaseSkill = currentSkill as SkillPhase;
            Parameter1Value.text = $"{phaseSkill.GetDamageMultiplier() * 100:F1}%";
            Parameter2Name.text = "Speed Up Rate";
            Parameter2Value.text = $"{phaseSkill.GetSpeedUpPercentage() * 100:F1}%";
        }
    }

}

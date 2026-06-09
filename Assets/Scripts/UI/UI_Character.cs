using UnityEngine;

public class UI_Character : MonoBehaviour
{
    private UI_StatSlot[] statSlots;
    [SerializeField] private UI_SkillSlot skillSlot;
    [SerializeField] private PlayerSkillManager playerSkill;
    [SerializeField] private UI_SkillDecay skillDecayUI;
    private void Awake()
    {
        statSlots = GetComponentsInChildren<UI_StatSlot>(true);
    }

    public void ToggleCharacterUI()
    {
        bool shouldOpen = !gameObject.activeSelf;

        gameObject.SetActive(shouldOpen);

        
        if (shouldOpen)
        {
            UpdateAllStatSlots();
            skillSlot.UpdateSkillInfo();
            skillDecayUI.UpdateDecayUI(playerSkill.SkillDecay);
        }
    }

    public void UpdateAllStatSlots()
    {
        foreach (UI_StatSlot slot in statSlots)
        {
            slot.UpdateStatValue();
        }
    }
}
using UnityEngine;

public class UI_Character : MonoBehaviour
{
    private UI_StatSlot[] statSlots;

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
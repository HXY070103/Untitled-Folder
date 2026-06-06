using UnityEngine;
using UnityEngine.UI;

public class UI_SkillDecay : MonoBehaviour
{
    [SerializeField] private Image[] filledImages; // Array of filled images representing the skill decay

    private void Awake()
    {
        UpdateDecayUI(0); // Initialize the UI with no decay
    }

    public void UpdateDecayUI(int decaylevel)
    {
        decaylevel = Mathf.Clamp(decaylevel, 0, filledImages.Length); // Ensure decay level is within bounds
    
        for (int i = 0; i < filledImages.Length; i++)
        {
            filledImages[i].enabled = i < decaylevel; // Enable images up to the current decay level
        }
    }
}

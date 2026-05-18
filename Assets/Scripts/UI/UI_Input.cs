using UnityEngine;

public class UI_Input : MonoBehaviour
{
    [SerializeField] private UI_Character characterUI;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            characterUI.ToggleCharacterUI();
        }
    }
}
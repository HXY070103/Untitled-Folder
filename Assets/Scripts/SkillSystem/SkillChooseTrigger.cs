using UnityEngine;
using UnityEngine.InputSystem;

public class SkillChooseTrigger : MonoBehaviour
{
    [SerializeField] private SkillChoosePanel skillChoosePanel;
    [SerializeField] private GameObject interactHint;

    [Header("Input")]
    [SerializeField] private InputActionReference interactAction;

    private bool playerInRange;

    private void Awake()
    {
        if (interactHint != null)
        {
            interactHint.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.action.performed += OnInteractPerformed;
            interactAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.action.performed -= OnInteractPerformed;
            interactAction.action.Disable();
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (playerInRange == false)
        {
            return;
        }

        TryOpenSkillChoosePanel();
    }

    private void TryOpenSkillChoosePanel()
    {
        if (skillChoosePanel == null)
        {
            Debug.LogError("SkillChoosePanel is not assigned.");
            return;
        }

        skillChoosePanel.OpenPanel();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() == null)
        {
            return;
        }

        playerInRange = true;

        if (interactHint != null)
        {
            interactHint.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() == null)
        {
            return;
        }

        playerInRange = false;

        if (interactHint != null)
        {
            interactHint.SetActive(false);
        }
    }
}
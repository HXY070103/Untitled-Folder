using UnityEngine;
using UnityEngine.InputSystem;

public class ResetEnemy : MonoBehaviour
{
    [Header("Interact")]
    [SerializeField] private bool playerInRange;

    [Header("Enemy Detect Area")]
    [SerializeField] private Transform areaCenter;
    [SerializeField] private Vector2 areaSize = new Vector2(6f, 3f);
    [SerializeField] private LayerMask enemyLayer;

    [Header("Enemy Reset")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform enemyInitialPoint;

    [Header("Self Reset")]
    [SerializeField] private bool resetSelfPosition = false;

    [Header("Input")]
    [SerializeField] private InputActionReference interactAction;

    private Vector3 selfInitialPosition;
    private Quaternion selfInitialRotation;

    private void Awake()
    {
        selfInitialPosition = transform.position;
        selfInitialRotation = transform.rotation;
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

        ResetEnemyInArea();
    }

    private void ResetEnemyInArea()
    {
        if (areaCenter == null)
        {
            Debug.LogWarning($"{gameObject.name}: areaCenter 没有设置");
            return;
        }

        if (enemyPrefab == null)
        {
            Debug.LogWarning($"{gameObject.name}: enemyPrefab 没有设置");
            return;
        }

        if (enemyInitialPoint == null)
        {
            Debug.LogWarning($"{gameObject.name}: enemyInitialPoint 没有设置");
            return;
        }

        Collider2D enemyCollider = Physics2D.OverlapBox(
            areaCenter.position,
            areaSize,
            0f,
            enemyLayer
        );

        if (enemyCollider != null)
        {
            Destroy(enemyCollider.gameObject);
        }

        Instantiate(
            enemyPrefab,
            enemyInitialPoint.position,
            enemyInitialPoint.rotation
        );

        if (resetSelfPosition)
        {
            transform.position = selfInitialPosition;
            transform.rotation = selfInitialRotation;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() == null)
        {
            return;
        }
        playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Player>() == null)
        {
            return;
        }
        playerInRange = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (areaCenter == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(areaCenter.position, areaSize);
    }
}
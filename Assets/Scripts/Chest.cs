using UnityEngine;

public class Chest : MonoBehaviour, IDamageable, ISaveable {
    private Animator anim => GetComponentInChildren<Animator>();
    private Rigidbody2D rb => GetComponent<Rigidbody2D>();
    private EntityVFX fx => GetComponent<EntityVFX>();

    [Header("Save info")]
    [SerializeField] private string chestID;

    [Header("Open info")]
    [SerializeField] private Vector2 knockback = new Vector2(0, 5);

    [Header("Chest info")]
    [SerializeField] private bool isBetter;

    private bool isChoosing;
    private bool isClaimed;

    public void TakeDamage(Damage damage, Transform damageDealer) {
        if (isChoosing) return;
        if (isClaimed) return;
        OpenChestForChoosing();
    }

    private void OpenChestForChoosing()
    {
        isChoosing = true;
        anim.SetBool("Open", true);
        rb.linearVelocity = knockback;
        rb.angularVelocity = Random.Range(-180, 180);
        fx.PlayOnDamageVFX();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Chest: Player not found in scene.");
            return;
        }
        if (UI_ChestReward.Instance == null)
        {
            Debug.LogError("Chest: UI_ChestReward instance not found.");
            return;
        }
        UI_ChestReward.Instance.Open(player, this, isBetter);
    }

    public void ClaimReward()
    {
        isChoosing = false;
        isClaimed = true;

        if (anim != null)
        {
            anim.SetBool("Open", true);
        }
    }

    private void ResetToUnopened()
    {
        isChoosing = false;
        isClaimed = false;
        if (anim != null)
        {
            anim.SetBool("Open", false);
        }
    }

    private void SetClaimedWithoutReward()
    {
        isChoosing = false;
        isClaimed = true;

        if (anim != null)
        {
            anim.SetBool("Open", true);
        }
    }

    public bool IsInvulnerable() {
        return false;
    }

    public void TryApplyInstantKill() {}

    public void LoadData(GameData data)
    {
        if (string.IsNullOrEmpty(chestID))
        {
            Debug.LogWarning($"{gameObject.name} has no chestID");
            return;
        }
        if (data.openedChestID.Contains(chestID))
        {
            SetClaimedWithoutReward();
        } else
        {
            ResetToUnopened();
        }
    }

    public void SaveData(ref GameData data)
    {
        if (string.IsNullOrEmpty(chestID))
        {
            Debug.LogWarning($"{gameObject.name} has no chestID");
            return;
        }
        if (isClaimed && !data.openedChestID.Contains(chestID))
        {
            data.openedChestID.Add(chestID);
        }
    }
}

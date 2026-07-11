using System.Collections;
using UnityEngine;

public class ArenaBreakableFloorZone : MonoBehaviour {
    [Header("Floor")]
    [SerializeField] private SpriteRenderer floorRenderer;
    [SerializeField] private Sprite normalFloorSprite;
    [SerializeField] private Sprite damagedFloorSprite;
    [SerializeField] private Collider2D floorCollider;

    [Header("Spike pit")]
    [SerializeField] private GameObject spikeRoot;
    [SerializeField] private Collider2D spikeDamageTrigger;
    [SerializeField] private float spikeDamage = 20;
    [SerializeField] private float damageInterval = 0.75f;

    [Header("Optional missing assets")]
    [SerializeField] private GameObject optionalBreakVFX;
    [SerializeField] private AudioClip optionalBreakSFX;

    private float nextDamageTime;
    public bool IsBroken { get; private set; }

    private void Awake() {
        RestoreImmediate();
    }

    public IEnumerator RunHazard(float warningDuration, float brokenDuration, float restoreDelay) {
        if (IsBroken) yield break;

        if (floorRenderer != null && damagedFloorSprite != null) {
            floorRenderer.sprite = damagedFloorSprite;
        }
        yield return new WaitForSeconds(warningDuration);

        IsBroken = true;
        if (optionalBreakVFX != null) Instantiate(optionalBreakVFX, transform.position, Quaternion.identity);
        if (optionalBreakSFX != null) AudioSource.PlayClipAtPoint(optionalBreakSFX, transform.position);
        if (floorRenderer != null) floorRenderer.enabled = false;
        if (floorCollider != null) floorCollider.enabled = false;
        if (spikeRoot != null) spikeRoot.SetActive(true);
        if (spikeDamageTrigger != null) spikeDamageTrigger.enabled = true;

        yield return new WaitForSeconds(brokenDuration);

        if (spikeDamageTrigger != null) spikeDamageTrigger.enabled = false;
        if (spikeRoot != null) spikeRoot.SetActive(false);
        yield return new WaitForSeconds(restoreDelay);

        MoveActorsAboveRestoringFloor();
        RestoreImmediate();
    }

    private void RestoreImmediate() {
        IsBroken = false;
        if (floorRenderer != null) {
            floorRenderer.enabled = true;
            if (normalFloorSprite != null) floorRenderer.sprite = normalFloorSprite;
        }
        if (floorCollider != null) floorCollider.enabled = true;
        if (spikeDamageTrigger != null) spikeDamageTrigger.enabled = false;
        if (spikeRoot != null) spikeRoot.SetActive(false);
    }

    private void MoveActorsAboveRestoringFloor() {
        Player player = Player.instance;
        if (floorCollider == null) return;
        Bounds bounds = floorCollider.bounds;
        if (player != null) MoveActorAboveFloor(player.transform, bounds);
        foreach (Enemy enemy in FindObjectsByType<Enemy>(FindObjectsInactive.Exclude)) {
            if (enemy is EnemyBoss) continue;
            MoveActorAboveFloor(enemy.transform, bounds);
        }
    }

    private void MoveActorAboveFloor(Transform actor, Bounds bounds) {
        Vector3 position = actor.position;
        if (position.x < bounds.min.x || position.x > bounds.max.x ||
            position.y < bounds.min.y - 2 || position.y > bounds.max.y + 1) return;
        position.y = bounds.max.y + 1.1f;
        actor.position = position;
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (!IsBroken || Time.time < nextDamageTime) return;
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;
        nextDamageTime = Time.time + damageInterval;
        playerHealth.TakeDamage(new Damage(spikeDamage, false), transform);
    }
}

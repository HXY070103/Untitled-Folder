using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossArenaController : MonoBehaviour {
    [SerializeField] private ArenaBreakableFloorZone[] floorZones;
    [SerializeField] private Transform[] summonPoints;
    [SerializeField] private float warningDuration = 1.1f;
    [SerializeField] private float brokenDuration = 2.5f;
    [SerializeField] private float restoreDelay = 0.35f;

    private bool hazardActive;
    public bool HazardActive => hazardActive;
    public Transform[] SummonPoints => summonPoints;

    public bool TryStartFloorAttack(MonoBehaviour runner, int requestedZones) {
        if (hazardActive || floorZones == null || floorZones.Length < 2) return false;
        runner.StartCoroutine(FloorAttackCo(requestedZones));
        return true;
    }

    private IEnumerator FloorAttackCo(int requestedZones) {
        hazardActive = true;
        int count = Mathf.Clamp(requestedZones, 1, floorZones.Length - 1);
        List<int> available = new List<int>();
        for (int i = 0; i < floorZones.Length; i++) available.Add(i);

        for (int i = 0; i < count; i++) {
            int pick = Random.Range(0, available.Count);
            ArenaBreakableFloorZone zone = floorZones[available[pick]];
            available.RemoveAt(pick);
            if (zone != null) StartCoroutine(zone.RunHazard(warningDuration, brokenDuration, restoreDelay));
        }

        yield return new WaitForSeconds(warningDuration + brokenDuration + restoreDelay + 0.1f);
        hazardActive = false;
    }
}

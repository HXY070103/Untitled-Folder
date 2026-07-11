using System.Collections;
using UnityEngine;

public class NecromancerArenaController : MonoBehaviour {
    [SerializeField] private ArenaDynamicPlatform[] dynamicPlatforms;
    [SerializeField] private Transform[] flightAnchors;
    [SerializeField] private Transform[] summonPoints;
    [SerializeField] private float platformWarningDuration = 0.8f;
    [SerializeField] private float platformSequenceInterval = 0.35f;

    public Transform[] FlightAnchors => flightAnchors;
    public Transform[] SummonPoints => summonPoints;

    private void Awake() {
        RestorePlatforms();
    }

    public IEnumerator RemovePlatformsSequence() {
        foreach (ArenaDynamicPlatform platform in dynamicPlatforms) {
            if (platform == null) continue;
            platform.Disappear(platformWarningDuration);
            yield return new WaitForSeconds(platformSequenceInterval);
        }
        yield return new WaitForSeconds(platformWarningDuration);
    }

    public void RestorePlatforms() {
        if (dynamicPlatforms == null) return;
        foreach (ArenaDynamicPlatform platform in dynamicPlatforms) {
            platform?.Restore();
        }
    }
}

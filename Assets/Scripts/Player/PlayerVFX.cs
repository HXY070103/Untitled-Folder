using System.Collections;
using UnityEngine;

public class PlayerVFX : EntityVFX {
    [Header("Global pause info")]
    [SerializeField] private float globalPauseScale = 0.08f;
    [SerializeField] private float globalPauseDuration = 0.05f;

    private Coroutine globalPauseCo;
    private float originalTimeScale;
    private float originalFixedDeltaTime;

    protected override void Awake() {
        base.Awake();
        
        originalTimeScale = Time.timeScale;
        originalFixedDeltaTime = Time.fixedDeltaTime;
    }

    public override void PlayOnDamageVFX() {
        base.PlayOnDamageVFX();

        DoGlobalPause();
    }
    
    public void DoGlobalPause() {
        if (globalPauseCo != null) {
            StopCoroutine(globalPauseCo);
        }

        globalPauseCo = StartCoroutine(GlobalPauseCo());
    }

    private IEnumerator GlobalPauseCo() {
        if (Time.timeScale <= 0f) {
            yield break;
        }

        Time.timeScale = globalPauseScale;
        Time.fixedDeltaTime = originalFixedDeltaTime * globalPauseScale;

        yield return new WaitForSecondsRealtime(globalPauseDuration);

        Time.timeScale = originalTimeScale;
        Time.fixedDeltaTime = originalFixedDeltaTime;
    }
}

using System.Collections;
using UnityEngine;

public class EnemyVFX : EntityVFX {
    [Header("Counter attack window info")]
    [SerializeField] private GameObject attackAlert;

    [Header("Pre-attack info")]
    [SerializeField] private Color preAttackVFXColor = Color.red;
    [SerializeField] private float preAttackVFXDuration = 0.3f;

    private Color originalColor;
    private Coroutine preAttackVFXCo;

    protected override void Awake() {
        base.Awake();

        originalColor = sr.color;
    }

    public void SetAttackAlert(bool enable) {
        attackAlert.SetActive(enable);
    }

    public void PlayPreAttackVFX() {
        if (preAttackVFXCo != null) {
            StopCoroutine(preAttackVFXCo);
        }

        preAttackVFXCo = StartCoroutine(PreAttackVFXCo());
    }

    private IEnumerator PreAttackVFXCo() {
        sr.color = preAttackVFXColor;
        yield return new WaitForSeconds(preAttackVFXDuration);
        sr.color = originalColor;
    }
}

using UnityEngine;

public class FireVFXController : MonoBehaviour {
    public void Shrink() {
        GetComponentInChildren<Animator>().SetTrigger("Shrink");
        GetComponentInChildren<SpriteRenderer>().color = Color.yellow;
    }

    public void Healing() {
        GetComponentInParent<SkillLifeSteal>().Healing();
        Destroy(gameObject);
    }
}

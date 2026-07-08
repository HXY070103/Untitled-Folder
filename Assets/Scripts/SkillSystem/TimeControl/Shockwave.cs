using UnityEngine;

public class Shockwave : MonoBehaviour {
    [SerializeField] private LayerMask whatIsEnemy;

    private SkillTimeControl skill;

    public void SetUpShockwave(SkillTimeControl skill) {
        this.skill = skill;

        DamageEnemies();
        Destroy(gameObject, skill.shockwaveDestoryDelay);
    }

    private void DamageEnemies() {
        foreach (Collider2D collider in GetEnemiesInExplosionRange()) {
            Enemy enemy = collider.GetComponentInParent<Enemy>();

            if (enemy == null) {
                continue;
            }

            enemy.health.ReduceHP(skill.shockwaveDamage);
            enemy.vfx.PlayOnDamageVFX();
        }
    }

    private Collider2D[] GetEnemiesInExplosionRange() {
        return 
            Physics2D.OverlapCircleAll(transform.position, skill.shockwaveDamageRange, whatIsEnemy);
    }
}

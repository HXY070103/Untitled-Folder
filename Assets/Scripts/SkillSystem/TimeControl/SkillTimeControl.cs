using UnityEngine;

public class SkillTimeControl : SkillBase {
    [SerializeField] private GameObject domainPrefab;
    [SerializeField] private float maxSize = 20;
    [SerializeField] private float slowDownPercentage = 0.8f;

    private GameObject domain;

    public float MaxSize => maxSize * stats.GetSkillEffect();
    public float SlowDownPercentage => slowDownPercentage * stats.GetSkillEffect();

    protected override void StartSkill() {
        base.StartSkill();

        CreateDomain();
    }

    protected override void EndSkill() {
        base.EndSkill();

        DestroyDomain();
    }

    private void CreateDomain() {
        domain = Instantiate(domainPrefab, transform.position, Quaternion.identity);
        domain.GetComponent<Domain>().SetUpDomain(this);
    }

    private void DestroyDomain() {
        domain.GetComponent<Domain>().ShrinkDomain();
    }

    public void FreezeDomain() {
        slowDownPercentage = 1;
    }
}

using UnityEngine;

public class SkillTimeControl : SkillBase {
    [SerializeField] private GameObject domainPrefab;
    public float maxSize = 20;
    public float expandSpeed = 2;
    public float slowDownPercentage = 0.8f;

    private GameObject domain;

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
}

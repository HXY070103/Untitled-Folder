using UnityEngine;

public class SkillTimeControl : SkillBase {
    [Header("Domain info")]
    [SerializeField] private GameObject domainPrefab;
    public float maxSize = 20;
    public float expandSpeed = 2;
    public float slowDownPercentage = 0.8f;

    public override void ApplySkill() {
        base.ApplySkill();

        CreatDomain();
    }

    private void CreatDomain() {
        GameObject domain = Instantiate(domainPrefab, transform.position, Quaternion.identity);
        domain.GetComponent<Domain>().SetUpDomain(this);
    }
}

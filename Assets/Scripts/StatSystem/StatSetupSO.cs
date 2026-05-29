using UnityEngine;

[CreateAssetMenu(menuName = "Default Stat Setup", fileName = "Default Stat Setup")]
public class StatSetupSO : ScriptableObject {
    [Header("Resources")]
    public float maxHP = 100;

    [Header("Offense")]
    public float damage = 10;
    public float critPower = 2;
    public float critChance = 20;

    [Header("Defense")]
    public float armor = 0;

    [Header("Player Skill")]
    public float skillEffect = 1;
}

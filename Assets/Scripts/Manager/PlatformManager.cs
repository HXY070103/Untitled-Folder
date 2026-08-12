using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    [SerializeField] private GameObject platform;

    private readonly HashSet<Enemy> summonedEnemies = new HashSet<Enemy>();
    private int activeSummonRegistrations;

    private void Awake()
    {
        int ignoredLayers = LayerMask.GetMask("Enemy");

        foreach (Collider2D platformCollider in platform.GetComponents<Collider2D>())
        {
            platformCollider.excludeLayers |= ignoredLayers;
        }
    }

    public void HidePlatform()
    {
        platform.SetActive(false);
    }

    public void ShowPlatform()
    {
        platform.SetActive(true);
    }

    public void BeginSummon()
    {
        activeSummonRegistrations += 1;
        HidePlatform();
    }

    public void RegisterSummonedEnemy(Enemy enemy)
    {
        if (enemy != null)
        {
            summonedEnemies.Add(enemy);
        }
    }

    public void CompleteSummon()
    {
        activeSummonRegistrations = Mathf.Max(0, activeSummonRegistrations - 1);
        TryShowPlatform();
    }

    private void HandleEnemyKilled(Enemy enemy)
    {
        if (summonedEnemies.Remove(enemy))
        {
            TryShowPlatform();
        }
    }

    private void TryShowPlatform()
    {
        if (activeSummonRegistrations == 0 && summonedEnemies.Count == 0)
        {
            ShowPlatform();
        }
    }

    private void OnEnable()
    {
        EnemyHealth.OnEnemyKilled += HandleEnemyKilled;
    }

    private void OnDisable()
    {
        EnemyHealth.OnEnemyKilled -= HandleEnemyKilled;
    }
}

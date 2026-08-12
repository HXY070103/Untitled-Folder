using UnityEngine;

public static class EnemyBossLightningPattern {
    public const int RegionCount = 5;

    public static int[][] All = {
        new[] { 0, 3 },
        new[] { 0, 4 },
        new[] { 1, 4 },
        new[] { 0, 2 },
        new[] { 2, 4 },
        new[] { 0, 1, 4 },
        new[] { 0, 3, 4 },
        new[] { 0, 1, 2 },
        new[] { 2, 3, 4 }
    };
}

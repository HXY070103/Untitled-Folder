using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class EliteBossSceneBuilder {
    private const string GeneratedRootName = "__GENERATED_ELITE_BOSS_ARENA__";
    private const string SceneFolder = "Assets/Scenes";

    private static readonly string FloorSpritePath =
        "Assets/Graphics/Surroundings/Medieval_Castle/Tiles/floor_tile_1.png";
    private static readonly string DamagedSpritePath =
        "Assets/Graphics/Surroundings/Medieval_Castle/Tiles/damaged_brick_1.png";
    private static readonly string SpikeSpritePath =
        "Assets/Graphics/Surroundings/Medieval_Castle/Tiles/spikes.png";

    [MenuItem("Tools/Elite Boss Arenas/Build Review Scenes")]
    public static void BuildAll() {
        RestoreTemplatesAfterReviewBuild();
        ConfigureEnemyPrefabs();
        BuildEliteScene1();
        BuildEliteScene2();
        BuildEliteScene3();
        BuildEliteScene4();
        BuildBossScene();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        ValidateAll();
        Debug.Log("Generated EliteScene_1-4 and BossScene with Unity Editor APIs.");
    }

    private static void RestoreTemplatesAfterReviewBuild() {
        string[] templateNames = {
            "EliteLevel_1.unity", "EliteLevel_2.unity", "EliteLevel_3.unity",
            "EliteLevel_4.unity", "BossLevel.unity"
        };
        foreach (string templateName in templateNames) {
            Scene scene = EditorSceneManager.OpenScene($"{SceneFolder}/{templateName}", OpenSceneMode.Single);
            GameObject generatedRoot = scene.GetRootGameObjects()
                .FirstOrDefault(root => root.name == GeneratedRootName);
            if (generatedRoot != null) UnityEngine.Object.DestroyImmediate(generatedRoot);

            if (templateName == "EliteLevel_2.unity" && FindSceneObject<EnemySkeletonKing>(scene) == null) {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                    "Assets/Prefab/Enemy/EnemySkeletonKing.prefab");
                GameObject instance = PrefabUtility.InstantiatePrefab(prefab, scene) as GameObject;
                if (instance == null) throw new InvalidOperationException("Could not restore EliteLevel_2 SkeletonKing.");
                instance.name = "EnemySkeletonKing";
                instance.transform.position = new Vector3(23.56f, -17.85f, 0);
            }

            if (templateName == "BossLevel.unity") {
                EnemyBoss boss = FindSceneObject<EnemyBoss>(scene);
                Player player = FindSceneObject<Player>(scene);
                if (boss != null) {
                    RevertPrefabPropertyOverride(boss, "arenaController");
                    boss.transform.position = new Vector3(12.32f, -2.06f, 0);
                }
                if (player != null) player.transform.position = new Vector3(-12.1f, -1.7f, 0);
            }
            if (templateName == "EliteLevel_4.unity") {
                EnemyNecromancer necromancer = FindSceneObject<EnemyNecromancer>(scene);
                if (necromancer != null) RevertPrefabPropertyOverride(necromancer, "arenaController");
            }
            Save(scene);
        }
    }

    [MenuItem("Tools/Elite Boss Arenas/Validate Review Scenes")]
    public static void ValidateAll() {
        ValidatePrefabReferences();
        ValidateScene("EliteScene_1.unity", 1, typeof(EnemySkeletonKing), 0);
        ValidateScene("EliteScene_2.unity", 0, null, 0);
        ValidateScene("EliteScene_3.unity", 1, typeof(EnemyDarkKnight), 0);
        ValidateScene("EliteScene_4.unity", 1, typeof(EnemyNecromancer), 1);
        ValidateScene("BossScene.unity", 1, typeof(EnemyBoss), 1);
        Debug.Log("Elite/Boss review scene validation passed.");
    }

    private static void ValidatePrefabReferences() {
        ValidateAssignedReference<EnemySkeletonKing>(
            "Assets/Prefab/Enemy/EnemySkeletonKing.prefab", "horizontalSweepPrefab");
        ValidateAssignedReference<EnemyNecromancer>(
            "Assets/Prefab/Enemy/EnemyNecromancer.prefab", "skeletonPrefab");
        GameObject bossPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/Enemy/EnemyBoss.prefab");
        EnemyBoss boss = bossPrefab != null ? bossPrefab.GetComponent<EnemyBoss>() : null;
        if (boss == null) throw new InvalidOperationException("EnemyBoss prefab is missing its component.");
        SerializedObject serializedBoss = new SerializedObject(boss);
        if (serializedBoss.FindProperty("phaseTwoSummonPrefabs").arraySize < 2 ||
            serializedBoss.FindProperty("phaseThreeSummonPrefabs").arraySize < 4) {
            throw new InvalidOperationException("EnemyBoss phase summon pools are not configured.");
        }
    }

    private static void ValidateAssignedReference<T>(string prefabPath, string propertyName) where T : Component {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        T component = prefab != null ? prefab.GetComponent<T>() : null;
        if (component == null) throw new InvalidOperationException($"{typeof(T).Name} missing in {prefabPath}.");
        SerializedProperty property = new SerializedObject(component).FindProperty(propertyName);
        if (property == null || property.objectReferenceValue == null) {
            throw new InvalidOperationException($"{typeof(T).Name}.{propertyName} is not configured.");
        }
    }

    private static void BuildEliteScene1() {
        Scene scene = CreateReviewScene("EliteLevel_1.unity", "EliteScene_1.unity");
        EnemySkeletonKing enemy = FindSceneObject<EnemySkeletonKing>(scene);
        if (enemy == null) throw new InvalidOperationException("SkeletonKing missing in EliteLevel_1 template.");

        Transform root = CreateGeneratedRoot(scene, "SkeletonKing_RoyalCrypt");
        Vector3 center = enemy.transform.position;
        CreateArenaPlatform(root, "Left_Evasion_Platform", center + new Vector3(-5, 2.4f), new Vector2(4, 0.45f));
        CreateArenaPlatform(root, "Right_Evasion_Platform", center + new Vector3(5, 2.4f), new Vector2(4, 0.45f));
        CreateArenaWall(root, "Left_WallJump_Surface", center + new Vector3(-14, 3));
        CreateArenaWall(root, "Right_WallJump_Surface", center + new Vector3(14, 3));
        Save(scene);
    }

    private static void BuildEliteScene2() {
        Scene scene = CreateReviewScene("EliteLevel_2.unity", "EliteScene_2.unity");
        foreach (Enemy enemy in FindSceneObjects<Enemy>(scene)) {
            UnityEngine.Object.DestroyImmediate(enemy.gameObject);
        }
        Transform root = CreateGeneratedRoot(scene, "Future_Elite_Placeholder");
        GameObject marker = new GameObject("FourthElite_Reserved_NoContent");
        SceneManager.MoveGameObjectToScene(marker, scene);
        marker.transform.SetParent(root, false);
        Save(scene);
    }

    private static void BuildEliteScene3() {
        Scene scene = CreateReviewScene("EliteLevel_3.unity", "EliteScene_3.unity");
        EnemyDarkKnight enemy = FindSceneObject<EnemyDarkKnight>(scene);
        if (enemy == null) throw new InvalidOperationException("DarkKnight missing in EliteLevel_3 template.");

        Transform root = CreateGeneratedRoot(scene, "DarkKnight_BrokenBladeHall");
        Vector3 center = enemy.transform.position;
        CreateArenaPlatform(root, "Left_Air_Route", center + new Vector3(-8, 2.6f), new Vector2(3, 0.4f));
        CreateArenaPlatform(root, "Right_Air_Route", center + new Vector3(8, 2.6f), new Vector2(3, 0.4f));
        CreateArenaWall(root, "Left_WallJump_Surface", center + new Vector3(-15, 3));
        CreateArenaWall(root, "Right_WallJump_Surface", center + new Vector3(15, 3));
        CreateMarkerRow(root, "Ceiling_SwordWave_Anchors", center + Vector3.up * 8, 5, 7);
        Save(scene);
    }

    private static void BuildEliteScene4() {
        Scene scene = CreateReviewScene("EliteLevel_4.unity", "EliteScene_4.unity");
        EnemyNecromancer enemy = FindSceneObject<EnemyNecromancer>(scene);
        if (enemy == null) throw new InvalidOperationException("Necromancer missing in EliteLevel_4 template.");

        Transform root = CreateGeneratedRoot(scene, "Necromancer_FloatingAltar");
        Vector3 center = enemy.transform.position;
        ArenaDynamicPlatform[] platforms = new ArenaDynamicPlatform[3];
        platforms[0] = CreateDynamicPlatform(root, "DynamicPlatform_Left", center + new Vector3(-6, 2.2f), new Vector2(4, 0.45f));
        platforms[1] = CreateDynamicPlatform(root, "DynamicPlatform_Center", center + new Vector3(0, 3.2f), new Vector2(4, 0.45f));
        platforms[2] = CreateDynamicPlatform(root, "DynamicPlatform_Right", center + new Vector3(6, 2.2f), new Vector2(4, 0.45f));
        CreateArenaWall(root, "Left_WallJump_Surface", center + new Vector3(-13, 3));
        CreateArenaWall(root, "Right_WallJump_Surface", center + new Vector3(13, 3));

        Transform anchorRoot = CreateChild(root, "FlightAnchors", Vector3.zero);
        Transform[] anchors = {
            CreateChild(anchorRoot, "FlightAnchor_Left", center + new Vector3(-6, 4.8f)),
            CreateChild(anchorRoot, "FlightAnchor_Center", center + new Vector3(0, 5.5f)),
            CreateChild(anchorRoot, "FlightAnchor_Right", center + new Vector3(6, 4.8f))
        };
        Transform summonRoot = CreateChild(root, "SummonPoints", Vector3.zero);
        Transform[] summonPoints = {
            CreateChild(summonRoot, "SummonPoint_Left", center + new Vector3(-7, 0)),
            CreateChild(summonRoot, "SummonPoint_Right", center + new Vector3(7, 0))
        };

        NecromancerArenaController controller = root.gameObject.AddComponent<NecromancerArenaController>();
        SetObjectArray(controller, "dynamicPlatforms", platforms.Cast<UnityEngine.Object>().ToArray());
        SetObjectArray(controller, "flightAnchors", anchors.Cast<UnityEngine.Object>().ToArray());
        SetObjectArray(controller, "summonPoints", summonPoints.Cast<UnityEngine.Object>().ToArray());
        SetObjectReference(enemy, "arenaController", controller);
        Save(scene);
    }

    private static void BuildBossScene() {
        Scene scene = CreateReviewScene("BossLevel.unity", "BossScene.unity");
        EnemyBoss boss = FindSceneObject<EnemyBoss>(scene);
        Player player = FindSceneObject<Player>(scene);
        if (boss == null) throw new InvalidOperationException("EnemyBoss missing in BossLevel template.");

        Transform root = CreateGeneratedRoot(scene, "EnemyBoss_FinalCourt");
        Vector3 originalBossPosition = boss.transform.position;
        float centerX = player != null ? (boss.transform.position.x + player.transform.position.x) * 0.5f : boss.transform.position.x;
        Collider2D bossCollider = boss.GetComponent<Collider2D>();
        float originalFloorY = bossCollider != null ? bossCollider.bounds.min.y : boss.transform.position.y - 1.7f;
        float raisedFloorY = originalFloorY + 2f;

        boss.transform.position = originalBossPosition + Vector3.up * 2f;
        if (player != null) player.transform.position += Vector3.up * 2f;

        const int zoneCount = 4;
        const float zoneWidth = 7f;
        ArenaBreakableFloorZone[] zones = new ArenaBreakableFloorZone[zoneCount];
        for (int i = 0; i < zoneCount; i++) {
            float x = centerX + (i - (zoneCount - 1) * 0.5f) * zoneWidth;
            zones[i] = CreateBreakableFloorZone(root, $"BreakableFloorZone_{i + 1}", new Vector3(x, raisedFloorY), zoneWidth);
        }

        CreateArenaPlatform(root, "Left_Short_Platform", new Vector3(centerX - 10, raisedFloorY + 3), new Vector2(3, 0.4f));
        CreateArenaPlatform(root, "Right_Short_Platform", new Vector3(centerX + 10, raisedFloorY + 3), new Vector2(3, 0.4f));

        Transform summonRoot = CreateChild(root, "SummonPoints", Vector3.zero);
        Transform[] summonPoints = {
            CreateChild(summonRoot, "SummonPoint_Left", new Vector3(centerX - 11, raisedFloorY + 1)),
            CreateChild(summonRoot, "SummonPoint_Right", new Vector3(centerX + 11, raisedFloorY + 1))
        };
        BossArenaController controller = root.gameObject.AddComponent<BossArenaController>();
        SetObjectArray(controller, "floorZones", zones.Cast<UnityEngine.Object>().ToArray());
        SetObjectArray(controller, "summonPoints", summonPoints.Cast<UnityEngine.Object>().ToArray());
        SetObjectReference(boss, "arenaController", controller);
        Save(scene);
    }

    private static Scene CreateReviewScene(string templateName, string outputName) {
        string templatePath = $"{SceneFolder}/{templateName}";
        string outputPath = $"{SceneFolder}/{outputName}";
        Scene scene = EditorSceneManager.OpenScene(templatePath, OpenSceneMode.Single);
        if (!EditorSceneManager.SaveScene(scene, outputPath, false)) {
            throw new InvalidOperationException($"Could not save review scene: {outputPath}");
        }
        GameObject oldRoot = GameObject.Find(GeneratedRootName);
        if (oldRoot != null && oldRoot.scene == scene) UnityEngine.Object.DestroyImmediate(oldRoot);
        return scene;
    }

    private static Transform CreateGeneratedRoot(Scene scene, string arenaName) {
        GameObject root = new GameObject(GeneratedRootName);
        SceneManager.MoveGameObjectToScene(root, scene);
        GameObject arena = new GameObject(arenaName);
        arena.transform.SetParent(root.transform, false);
        return arena.transform;
    }

    private static ArenaDynamicPlatform CreateDynamicPlatform(Transform parent, string name, Vector3 position, Vector2 size) {
        GameObject platform = CreateArenaPlatform(parent, name, position, size);
        return platform.AddComponent<ArenaDynamicPlatform>();
    }

    private static GameObject CreateArenaPlatform(Transform parent, string name, Vector3 position, Vector2 size) {
        Sprite sprite = LoadSprite(FloorSpritePath);
        GameObject go = new GameObject(name);
        go.layer = LayerMask.NameToLayer("Ground");
        go.transform.SetParent(parent, false);
        go.transform.position = position;
        CreateScaledSpriteVisual(go.transform, "Visual", sprite, size);
        BoxCollider2D collider = go.AddComponent<BoxCollider2D>();
        collider.size = size;
        return go;
    }

    private static void CreateArenaWall(Transform parent, string name, Vector3 position) {
        CreateArenaPlatform(parent, name, position, new Vector2(0.6f, 6));
    }

    private static ArenaBreakableFloorZone CreateBreakableFloorZone(Transform parent, string name, Vector3 position, float width) {
        GameObject go = new GameObject(name);
        go.layer = LayerMask.NameToLayer("Ground");
        go.transform.SetParent(parent, false);
        go.transform.position = position;

        Sprite normalSprite = LoadSprite(FloorSpritePath);
        Sprite damagedSprite = LoadSprite(DamagedSpritePath);
        SpriteRenderer floorRenderer = CreateScaledSpriteVisual(
            go.transform, "FloorVisual", normalSprite, new Vector2(width, 0.5f));
        BoxCollider2D floorCollider = go.AddComponent<BoxCollider2D>();
        floorCollider.size = new Vector2(width, 0.5f);
        Rigidbody2D body = go.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.constraints = RigidbodyConstraints2D.FreezeAll;

        GameObject spikeRoot = new GameObject("SpikePit");
        spikeRoot.transform.SetParent(go.transform, false);
        spikeRoot.transform.localPosition = new Vector3(0, -1.05f, 0);
        CreateScaledSpriteVisual(spikeRoot.transform, "Visual", LoadSprite(SpikeSpritePath), new Vector2(width, 1.5f));
        BoxCollider2D spikeTrigger = spikeRoot.AddComponent<BoxCollider2D>();
        spikeTrigger.isTrigger = true;
        spikeTrigger.size = new Vector2(width, 1.5f);

        ArenaBreakableFloorZone zone = go.AddComponent<ArenaBreakableFloorZone>();
        SetObjectReference(zone, "floorRenderer", floorRenderer);
        SetObjectReference(zone, "normalFloorSprite", normalSprite);
        SetObjectReference(zone, "damagedFloorSprite", damagedSprite);
        SetObjectReference(zone, "floorCollider", floorCollider);
        SetObjectReference(zone, "spikeRoot", spikeRoot);
        SetObjectReference(zone, "spikeDamageTrigger", spikeTrigger);
        spikeRoot.SetActive(false);
        return zone;
    }

    private static void CreateMarkerRow(Transform parent, string name, Vector3 center, int count, float spacing) {
        Transform root = CreateChild(parent, name, Vector3.zero);
        for (int i = 0; i < count; i++) {
            float x = center.x + (i - (count - 1) * 0.5f) * spacing;
            CreateChild(root, $"Anchor_{i + 1}", new Vector3(x, center.y, center.z));
        }
    }

    private static SpriteRenderer CreateScaledSpriteVisual(
        Transform parent, string name, Sprite sprite, Vector2 worldSize) {
        GameObject visual = new GameObject(name);
        visual.transform.SetParent(parent, false);
        SpriteRenderer renderer = visual.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.drawMode = SpriteDrawMode.Simple;
        renderer.sortingLayerName = "Ground";
        Vector2 sourceSize = sprite.bounds.size;
        visual.transform.localScale = new Vector3(
            sourceSize.x > 0 ? worldSize.x / sourceSize.x : 1,
            sourceSize.y > 0 ? worldSize.y / sourceSize.y : 1,
            1);
        return renderer;
    }

    private static Transform CreateChild(Transform parent, string name, Vector3 worldPosition) {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.position = worldPosition;
        return go.transform;
    }

    private static void ConfigureEnemyPrefabs() {
        GameObject energyWave = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/Enemy/EnemyBossEnergyWave.prefab");
        GameObject skeleton = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/Enemy/EnemySkeleton.prefab");
        GameObject knight = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/Enemy/EnemyKnight.prefab");
        GameObject mage = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/Enemy/EnemyMage.prefab");
        GameObject archer = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/Enemy/EnemyArcherElf.prefab");

        EditPrefab<EnemySkeletonKing>("Assets/Prefab/Enemy/EnemySkeletonKing.prefab", component => {
            SetObjectReference(component, "horizontalSweepPrefab", energyWave);
        });
        EditPrefab<EnemyNecromancer>("Assets/Prefab/Enemy/EnemyNecromancer.prefab", component => {
            SetObjectReference(component, "skeletonPrefab", skeleton);
        });
        EditPrefab<EnemyBoss>("Assets/Prefab/Enemy/EnemyBoss.prefab", component => {
            SetObjectArray(component, "phaseTwoSummonPrefabs", new UnityEngine.Object[] { skeleton, knight });
            SetObjectArray(component, "phaseThreeSummonPrefabs", new UnityEngine.Object[] { skeleton, knight, mage, archer });
        });
    }

    private static void EditPrefab<T>(string path, Action<T> edit) where T : Component {
        GameObject root = PrefabUtility.LoadPrefabContents(path);
        try {
            T component = root.GetComponent<T>();
            if (component == null) throw new InvalidOperationException($"{typeof(T).Name} missing in {path}");
            edit(component);
            PrefabUtility.SaveAsPrefabAsset(root, path);
        } finally {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void SetObjectReference(UnityEngine.Object target, string propertyName, UnityEngine.Object value) {
        SerializedObject serializedObject = new SerializedObject(target);
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        if (property == null) throw new MissingFieldException(target.GetType().Name, propertyName);
        property.objectReferenceValue = value;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(target);
    }

    private static void SetObjectArray(UnityEngine.Object target, string propertyName, UnityEngine.Object[] values) {
        SerializedObject serializedObject = new SerializedObject(target);
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        if (property == null) throw new MissingFieldException(target.GetType().Name, propertyName);
        property.arraySize = values.Length;
        for (int i = 0; i < values.Length; i++) property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(target);
    }

    private static void RevertPrefabPropertyOverride(UnityEngine.Object target, string propertyName) {
        SerializedObject serializedObject = new SerializedObject(target);
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        if (property == null) return;
        if (property.prefabOverride) {
            PrefabUtility.RevertPropertyOverride(property, InteractionMode.AutomatedAction);
        }
    }

    private static Sprite LoadSprite(string path) {
        Sprite sprite = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().FirstOrDefault();
        if (sprite == null) throw new InvalidOperationException($"Sprite not found: {path}");
        return sprite;
    }

    private static T FindSceneObject<T>(Scene scene) where T : Component {
        return FindSceneObjects<T>(scene).FirstOrDefault();
    }

    private static T[] FindSceneObjects<T>(Scene scene) where T : Component {
        return Resources.FindObjectsOfTypeAll<T>()
            .Where(item => item.gameObject.scene == scene && !EditorUtility.IsPersistent(item))
            .ToArray();
    }

    private static void Save(Scene scene) {
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene)) throw new InvalidOperationException($"Could not save {scene.path}");
    }

    private static void ValidateScene(
        string sceneName, int expectedEnemyCount, Type expectedEnemyType, int expectedControllerCount) {
        string path = $"{SceneFolder}/{sceneName}";
        Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
        Enemy[] enemies = FindSceneObjects<Enemy>(scene);
        if (enemies.Length != expectedEnemyCount) {
            throw new InvalidOperationException($"{sceneName}: expected {expectedEnemyCount} enemies, got {enemies.Length}.");
        }
        if (expectedEnemyType != null && enemies.Any(enemy => enemy.GetType() != expectedEnemyType)) {
            throw new InvalidOperationException($"{sceneName}: unexpected enemy type.");
        }
        int controllers = FindSceneObjects<NecromancerArenaController>(scene).Length +
            FindSceneObjects<BossArenaController>(scene).Length;
        if (controllers != expectedControllerCount) {
            throw new InvalidOperationException($"{sceneName}: expected {expectedControllerCount} arena controllers, got {controllers}.");
        }
        GameObject generatedRoot = scene.GetRootGameObjects().FirstOrDefault(root => root.name == GeneratedRootName);
        if (generatedRoot == null) throw new InvalidOperationException($"{sceneName}: generated arena root missing.");
        int missingScripts = CountMissingScripts(generatedRoot);
        if (missingScripts > 0) throw new InvalidOperationException($"{sceneName}: {missingScripts} missing scripts in generated arena.");
    }

    private static int CountMissingScripts(GameObject root) {
        int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(root);
        foreach (Transform child in root.transform) count += CountMissingScripts(child.gameObject);
        return count;
    }
}

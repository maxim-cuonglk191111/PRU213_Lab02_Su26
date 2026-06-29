// Assets/Editor/SnowBoarderSetup_Part1.cs
// SnowBoarder PRD v1.0.1 — Auto Setup Part 1
// Menu: SnowBoarder > Setup > Run Full Setup   (runs Part1 + Part2)
// Menu: SnowBoarder > Setup > Part 1 – Assets & Prefabs

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;

public static class SnowBoarderSetup_Part1
{
    // ── Paths ──────────────────────────────────────────────────
    const string SCRIPTS   = "Assets/Scripts";
    const string SPRITES   = "Assets/Sprites";
    const string PREFABS   = "Assets/Prefabs";
    const string PHYSICS   = "Assets/Physics";
    const string TILEMAPS  = "Assets/Tilemaps";

    // ── Menu entries ───────────────────────────────────────────
    [MenuItem("SnowBoarder/Setup/Run Full Setup")]
    public static void RunAll()
    {
        RunPart1();
        SnowBoarderSetup_Part2.RunPart2();
        SnowBoarderSetup_Part3.RunPart3();
        Debug.Log("=== SnowBoarder Full Setup Complete ===");
    }

    [MenuItem("SnowBoarder/Setup/Part 1 – Assets and Prefabs")]
    public static void RunPart1()
    {
        EnsureFolders();
        CreateInputConfigs();
        CreatePhysicsMaterial();
        CreatePrefabs();
        SetBuildScenes();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[Part1] Assets, prefabs, physics material done.");
    }

    // ── Folders ────────────────────────────────────────────────
    static void EnsureFolders()
    {
        string[] dirs = {
            PREFABS, PHYSICS, TILEMAPS,
            SCRIPTS+"/Player", SCRIPTS+"/Managers", SCRIPTS+"/UI", SCRIPTS+"/Environment",
            SPRITES+"/Characters", SPRITES+"/Environment", SPRITES+"/Clouds", SPRITES+"/UI"
        };
        foreach (var d in dirs)
        {
            if (!AssetDatabase.IsValidFolder(d))
            {
                var parts = d.Split('/');
                var parent = string.Join("/", parts, 0, parts.Length - 1);
                AssetDatabase.CreateFolder(parent, parts[parts.Length - 1]);
            }
        }
    }

    // ── InputConfig ScriptableObjects ──────────────────────────
    static void CreateInputConfigs()
    {
        CreateInputConfigAsset("Assets/Scripts/Player/InputConfig_P1.asset",
            KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.UpArrow);
        CreateInputConfigAsset("Assets/Scripts/Player/InputConfig_P2.asset",
            KeyCode.A, KeyCode.D, KeyCode.W);
    }

    static void CreateInputConfigAsset(string path, KeyCode left, KeyCode right, KeyCode thrust)
    {
        if (AssetDatabase.LoadAssetAtPath<ScriptableObject>(path) != null) return;
        var cfg = ScriptableObject.CreateInstance<InputConfig>();
        cfg.rotateLeft  = left;
        cfg.rotateRight = right;
        cfg.thrust      = thrust;
        AssetDatabase.CreateAsset(cfg, path);
        Debug.Log($"[Part1] Created {path}");
    }

    // ── Physics Material ───────────────────────────────────────
    static void CreatePhysicsMaterial()
    {
        const string path = "Assets/Physics/SnowMaterial.physicsMaterial2D";
        if (AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(path) != null) return;
        var mat = new PhysicsMaterial2D("SnowMaterial") { friction = 0.4f, bounciness = 0f };
        AssetDatabase.CreateAsset(mat, path);
        Debug.Log($"[Part1] Created {path}");
    }

    static void EnsureTags()
    {
        string[] requiredTags = { "Player2", "Obstacle", "Pickup", "PowerUp" };
        var asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if (asset != null && asset.Length > 0)
        {
            var so = new SerializedObject(asset[0]);
            var tags = so.FindProperty("tags");
            foreach (var t in requiredTags)
            {
                bool found = false;
                for (int i = 0; i < tags.arraySize; i++)
                {
                    if (tags.GetArrayElementAtIndex(i).stringValue == t) { found = true; break; }
                }
                if (!found)
                {
                    tags.InsertArrayElementAtIndex(tags.arraySize);
                    tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = t;
                }
            }
            so.ApplyModifiedProperties();
        }
    }

    // ── Prefabs ────────────────────────────────────────────────
    static void CreatePrefabs()
    {
        EnsureTags();
        CreatePlayerPrefab("Player1", new Color(1f, 1f, 1f, 1f));              // white
        CreatePlayerPrefab("Player2", new Color(1f, 0.42f, 0.208f, 1f));       // #FF6B35 orange

        CreateObstaclePrefab("Rock_Small",  0.3f);
        CreateObstaclePrefab("Rock_Large",  0.6f);
        CreateObstaclePrefab("Tree",        0.5f);
        CreatePickupPrefab("Snowflake");
        CreatePowerUpPrefab("PowerUp_Speed",  PowerUp.PowerUpType.SpeedBoost);
        CreatePowerUpPrefab("PowerUp_Shield", PowerUp.PowerUpType.Invincibility);
    }

    static void CreatePlayerPrefab(string name, Color tint)
    {
        string path = $"{PREFABS}/{name}.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;

        var root = new GameObject(name);
        root.tag = (name == "Player1") ? "Player" : "Player2";
        root.layer = LayerMask.NameToLayer("Default");

        var rb = root.AddComponent<Rigidbody2D>();
        rb.mass        = 1f;
        rb.angularDamping = 5f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Prevent tunneling through ice

        // Capsule collider for body (torso)
        var cc = root.AddComponent<CapsuleCollider2D>();
        cc.size = new Vector2(0.4f, 0.6f);
        cc.offset = new Vector2(0f, 0.1f);

        // Controllers
        root.AddComponent<PlayerController>();
        root.AddComponent<ScoreManager>();
        root.AddComponent<LivesManager>();
        root.AddComponent<TrickManager>();

        // Boarder_Top child (crash collider)
        var top = new GameObject("Boarder_Top");
        top.transform.SetParent(root.transform);
        top.transform.localPosition = new Vector3(0, 0.35f, 0);
        var topSR = top.AddComponent<SpriteRenderer>();
        topSR.color = tint;
        topSR.sortingLayerName = "Player";
        var topCol = top.AddComponent<CircleCollider2D>();
        topCol.radius = 0.25f;
        top.AddComponent<CrashHandler>();

        // Boarder_Bottom child (board)
        var bot = new GameObject("Boarder_Bottom");
        bot.transform.SetParent(root.transform);
        bot.transform.localPosition = new Vector3(0, -0.3f, 0);
        var botSR = bot.AddComponent<SpriteRenderer>();
        botSR.color = tint;
        botSR.sortingLayerName = "Player";
        
        // Snowboard collider (horizontal) for smooth sliding
        var botCol = bot.AddComponent<CapsuleCollider2D>();
        botCol.direction = CapsuleDirection2D.Horizontal;
        botCol.size = new Vector2(1.2f, 0.15f);
        botCol.offset = new Vector2(0f, -0.05f);

        // GroundChecker
        var gc = bot.AddComponent<GroundChecker>();

        // Load sprites if available
        var sprTop = AssetDatabase.LoadAssetAtPath<Sprite>($"{SPRITES}/Characters/Boarder_Top.png");
        var sprBot = AssetDatabase.LoadAssetAtPath<Sprite>($"{SPRITES}/Characters/Boarder_Bottom.png");
        if (sprTop) topSR.sprite = sprTop;
        if (sprBot) botSR.sprite = sprBot;

        PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        Debug.Log($"[Part1] Created {path}");
    }

    static void CreateObstaclePrefab(string name, float radius)
    {
        string path = $"{PREFABS}/{name}.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;
        var go = new GameObject(name);
        go.tag = "Obstacle";
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = "Environment";
        var col = go.AddComponent<PolygonCollider2D>();
        PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        Debug.Log($"[Part1] Created {path}");
    }

    static void CreatePickupPrefab(string name)
    {
        string path = $"{PREFABS}/{name}.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;
        var go = new GameObject(name);
        go.tag = "Pickup";
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = "Pickups";
        var spr = AssetDatabase.LoadAssetAtPath<Sprite>($"{SPRITES}/UI/snowflake.png");
        if (spr) sr.sprite = spr;
        var col = go.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.3f;
        go.AddComponent<PickupHandler>();
        PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        Debug.Log($"[Part1] Created {path}");
    }

    static void CreatePowerUpPrefab(string name, PowerUp.PowerUpType type)
    {
        string path = $"{PREFABS}/{name}.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;
        var go = new GameObject(name);
        go.tag = "PowerUp";
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = "Pickups";
        if (type == PowerUp.PowerUpType.SpeedBoost)
        {
            var spr = AssetDatabase.LoadAssetAtPath<Sprite>($"{SPRITES}/UI/star_boost.png");
            if (spr) sr.sprite = spr;
        }
        else
        {
            var spr = AssetDatabase.LoadAssetAtPath<Sprite>($"{SPRITES}/UI/shield_icon.png");
            if (spr) sr.sprite = spr;
        }
        var col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        var pu = go.AddComponent<PowerUp>();
        PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        Debug.Log($"[Part1] Created {path}");
    }

    // ── Build Settings ─────────────────────────────────────────
    static void SetBuildScenes()
    {
        var sceneNames = new[] {
            "MainMenu", "ModeSelect", "Level1", "Level1_PvP",
            "ScoreSummary", "PvPSummary"
        };
        var entries = new System.Collections.Generic.List<EditorBuildSettingsScene>();
        foreach (var s in sceneNames)
        {
            string p = $"Assets/Scenes/{s}.unity";
            entries.Add(new EditorBuildSettingsScene(p, true));
        }
        EditorBuildSettings.scenes = entries.ToArray();
        Debug.Log("[Part1] Build Settings updated with all scenes.");
    }
}

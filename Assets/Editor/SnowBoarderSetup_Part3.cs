// Assets/Editor/SnowBoarderSetup_Part3.cs
// Phase 3: instantiate player prefabs into scenes and wire ALL Inspector
// cross-references that Phase 2 (scene skeleton) could not link without
// live GameObject instances.
//
// Run order: Part 1 → Part 2 → Part 3
// Or use: SnowBoarder > Setup > Run Full Setup  (calls all three in sequence)

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

public static class SnowBoarderSetup_Part3
{
    const string SCENES  = "Assets/Scenes";
    const string PREFABS = "Assets/Prefabs";
    const string AUDIO   = "Assets/Audio";
    const string SPRITES = "Assets/Sprites";

    [MenuItem("SnowBoarder/Setup/Part 3 – Wire Scenes")]
    public static void RunPart3()
    {
        WirePrefabs();
        WireMainMenu();
        WireModeSelect();
        WireScoreSummary();
        WirePvPSummary();
        WireLevel1PvP();   // PvP before Solo so Level1_PvP never inherits Solo's GameManager
        WireLevel1Solo();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Done", "Part 3 complete — all scenes and prefabs are wired.", "OK");
        Debug.Log("=== [Part3] Scene wiring complete ===");
    }

    // ─────────────────────────────────────────────────────────────
    // PREFAB PRE-WIRING (audio clips, player tags)
    // ─────────────────────────────────────────────────────────────
    static void WirePrefabs()
    {
        var pickupClip = LoadAudio("pickup.ogg");

        var snowflakePrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PREFABS}/Snowflake.prefab");
        if (snowflakePrefab != null)
        {
            var ph = snowflakePrefab.GetComponent<PickupHandler>();
            if (ph != null) SetField(ph, "pickupClip", pickupClip);
        }

        SetPlayerPrefabTag("Player1", "Player");
        SetPlayerPrefabTag("Player2", "Player2");

        Debug.Log("[Part3] Prefabs pre-wired.");
    }

    static void SetPlayerPrefabTag(string prefabName, string tag)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PREFABS}/{prefabName}.prefab");
        if (prefab == null) return;
        var lm = prefab.GetComponent<LivesManager>();
        if (lm == null) return;
        var so = new SerializedObject(lm);
        so.FindProperty("playerTag").stringValue = tag;
        so.ApplyModifiedProperties();
    }

    // ─────────────────────────────────────────────────────────────
    // MAIN MENU — AudioManager + button wiring
    // ─────────────────────────────────────────────────────────────
    static void WireMainMenu()
    {
        if (!SceneExists("MainMenu")) return;
        var scene = EditorSceneManager.OpenScene($"{SCENES}/MainMenu.unity", OpenSceneMode.Single);

        var audioMgr = EnsureComponent<AudioManager>("AudioManager");
        WireAudioManager(audioMgr, LoadAudio("bgm_snow.ogg"));

        var mmm = Object.FindObjectOfType<MainMenuManager>();
        if (mmm != null)
        {
            var so = new SerializedObject(mmm);
            SetRef(so, "startButton",   FindBtn("Start_Btn"));
            SetRef(so, "optionsButton", FindBtn("Options_Btn"));
            SetRef(so, "quitButton",    FindBtn("Quit_Btn"));
            SetRef(so, "optionsPanel",  GameObject.Find("OptionsPanel"));
            so.ApplyModifiedProperties();
        }

        EditorSceneManager.SaveScene(scene);
        Debug.Log("[Part3] MainMenu wired.");
    }

    // ─────────────────────────────────────────────────────────────
    // MODE SELECT — button wiring
    // ─────────────────────────────────────────────────────────────
    static void WireModeSelect()
    {
        if (!SceneExists("ModeSelect")) return;
        var scene = EditorSceneManager.OpenScene($"{SCENES}/ModeSelect.unity", OpenSceneMode.Single);

        var msm = Object.FindObjectOfType<ModeSelectManager>();
        if (msm != null)
        {
            var so = new SerializedObject(msm);
            SetRef(so, "soloButton", FindBtn("Solo_Btn"));
            SetRef(so, "pvpButton",  FindBtn("PvP_Btn"));
            SetRef(so, "backButton", FindBtn("Back_Btn"));
            so.ApplyModifiedProperties();
        }

        EditorSceneManager.SaveScene(scene);
        Debug.Log("[Part3] ModeSelect wired.");
    }

    // ─────────────────────────────────────────────────────────────
    // SCORE SUMMARY (SOLO) — text + button wiring
    // ─────────────────────────────────────────────────────────────
    static void WireScoreSummary()
    {
        if (!SceneExists("ScoreSummary")) return;
        var scene = EditorSceneManager.OpenScene($"{SCENES}/ScoreSummary.unity", OpenSceneMode.Single);

        var mgr = Object.FindObjectOfType<ScoreSummaryManager>();
        if (mgr != null)
        {
            var so = new SerializedObject(mgr);
            SetRef(so, "currentScoreText", FindTMP("Your_Score:_0"));
            SetRef(so, "bestScoreText",    FindTMP("Best_Score:_0"));
            SetRef(so, "replayButton",     FindBtn("Replay_Btn"));
            SetRef(so, "mainMenuButton",   FindBtn("Main Menu_Btn"));
            so.ApplyModifiedProperties();
        }

        EditorSceneManager.SaveScene(scene);
        Debug.Log("[Part3] ScoreSummary wired.");
    }

    // ─────────────────────────────────────────────────────────────
    // PVP SUMMARY — text + button wiring
    // ─────────────────────────────────────────────────────────────
    static void WirePvPSummary()
    {
        if (!SceneExists("PvPSummary")) return;
        var scene = EditorSceneManager.OpenScene($"{SCENES}/PvPSummary.unity", OpenSceneMode.Single);

        var mgr = Object.FindObjectOfType<PvPSummaryManager>();
        if (mgr != null)
        {
            var so = new SerializedObject(mgr);
            SetRef(so, "winnerText",    FindTMP("Player_1_Wins!"));
            SetRef(so, "p1ScoreText",   FindTMP("P1_Score:_0"));
            SetRef(so, "p2ScoreText",   FindTMP("P2_Score:_0"));
            SetRef(so, "rematchButton",   FindBtn("Rematch_Btn"));
            SetRef(so, "mainMenuButton",  FindBtn("Main Menu_Btn"));
            so.ApplyModifiedProperties();
        }

        EditorSceneManager.SaveScene(scene);
        Debug.Log("[Part3] PvPSummary wired.");
    }

    // ─────────────────────────────────────────────────────────────
    // LEVEL1 PVP — players, managers, HUD, FinishLine
    // ─────────────────────────────────────────────────────────────
    static void WireLevel1PvP()
    {
        if (!SceneExists("Level1_PvP")) return;
        var scene = EditorSceneManager.OpenScene($"{SCENES}/Level1_PvP.unity", OpenSceneMode.Single);

        var crashClip  = LoadAudio("Crash SFX.ogg");
        var finishClip = LoadAudio("Finish SFX.ogg");
        var pickupClip = LoadAudio("pickup.ogg");
        var bgmClip    = LoadAudio("bgm_snow.ogg");

        // Remove any stray Solo GameManager that may have carried over from Level1
        var strayGM = Object.FindObjectOfType<GameManager>();
        if (strayGM != null) Object.DestroyImmediate(strayGM.gameObject);

        // AudioManager
        WireAudioManager(EnsureComponent<AudioManager>("AudioManager"), bgmClip);

        // EventSystem
        EnsureEventSystem();

        // Spawn points
        var sp1 = EnsureGO("SpawnPoint_P1");
        var sp2 = EnsureGO("SpawnPoint_P2");
        sp1.transform.position = new Vector3(-1f, 2f, 0f);
        sp2.transform.position = new Vector3(1f,  2f, 0f);

        // Player instances
        var p1GO = EnsurePlayerInScene("Player1", $"{PREFABS}/Player1.prefab", sp1.transform.position);
        var p2GO = EnsurePlayerInScene("Player2", $"{PREFABS}/Player2.prefab", sp2.transform.position);

        WirePlayerComponents(p1GO, sp1.transform, crashClip, "Player");
        WirePlayerComponents(p2GO, sp2.transform, crashClip, "Player2");

        // PvPGameManager
        var pvp = Object.FindObjectOfType<PvPGameManager>()
                  ?? new GameObject("PvPGameManager").AddComponent<PvPGameManager>();
        var pvpSO = new SerializedObject(pvp);
        SetRef(pvpSO, "player1",       p1GO);
        SetRef(pvpSO, "player2",       p2GO);
        SetRef(pvpSO, "livesManager1", p1GO.GetComponent<LivesManager>());
        SetRef(pvpSO, "livesManager2", p2GO.GetComponent<LivesManager>());
        SetRef(pvpSO, "scoreManager1", p1GO.GetComponent<ScoreManager>());
        SetRef(pvpSO, "scoreManager2", p2GO.GetComponent<ScoreManager>());
        pvpSO.ApplyModifiedProperties();

        // HUDManager_PvP
        var hudPvP = Object.FindObjectOfType<HUDManager_PvP>();
        if (hudPvP != null) WireHUDPvP(hudPvP, p1GO, p2GO);

        // FinishLine audio
        var fl = Object.FindObjectOfType<FinishLine>();
        if (fl != null)
        {
            var flAS = fl.gameObject.GetComponent<AudioSource>() ?? fl.gameObject.AddComponent<AudioSource>();
            flAS.playOnAwake = false;
            var flSO = new SerializedObject(fl);
            flSO.FindProperty("isPvP").boolValue = true;
            SetRef(flSO, "audioSource", flAS);
            SetRef(flSO, "finishClip",  finishClip);
            flSO.ApplyModifiedProperties();
        }

        // Snowflake pickups already in scene
        foreach (var ph in Object.FindObjectsOfType<PickupHandler>())
            SetField(ph, "pickupClip", pickupClip);

        EditorSceneManager.SaveScene(scene);
        Debug.Log("[Part3] Level1_PvP wired.");
    }

    // ─────────────────────────────────────────────────────────────
    // LEVEL1 SOLO — all managers, HUD, FinishLine
    // ─────────────────────────────────────────────────────────────
    static void WireLevel1Solo()
    {
        if (!SceneExists("Level1")) return;
        var scene = EditorSceneManager.OpenScene($"{SCENES}/Level1.unity", OpenSceneMode.Single);

        var crashClip  = LoadAudio("Crash SFX.ogg");
        var finishClip = LoadAudio("Finish SFX.ogg");
        var pickupClip = LoadAudio("pickup.ogg");
        var bgmClip    = LoadAudio("bgm_snow.ogg");

        WireAudioManager(EnsureComponent<AudioManager>("AudioManager"), bgmClip);
        EnsureComponent<GameManager>("GameManager");
        EnsureEventSystem();

        // Respawn / spawn point
        var respawn = EnsureGO("RespawnPoint");
        respawn.transform.position = new Vector3(-1f, 2f, 0f);

        // Player
        var p1GO = EnsurePlayerInScene("Player1", $"{PREFABS}/Player1.prefab", respawn.transform.position);
        WirePlayerComponents(p1GO, respawn.transform, crashClip, "Player");

        // Solo HUD
        var hud = BuildSoloHUDCanvas(p1GO);

        // Wire TrickManager toast target now that HUD is built
        var trick = p1GO.GetComponent<TrickManager>();
        if (trick != null) SetField(trick, "hudManager", hud);

        // FinishLine
        var fl = EnsureFinishLine();
        var flAS = fl.gameObject.GetComponent<AudioSource>() ?? fl.gameObject.AddComponent<AudioSource>();
        flAS.playOnAwake = false;
        var flSO = new SerializedObject(fl);
        flSO.FindProperty("isPvP").boolValue = false;
        SetRef(flSO, "audioSource", flAS);
        SetRef(flSO, "finishClip",  finishClip);
        flSO.ApplyModifiedProperties();

        // Snowflake pickups
        foreach (var ph in Object.FindObjectsOfType<PickupHandler>())
            SetField(ph, "pickupClip", pickupClip);

        EditorSceneManager.SaveScene(scene);
        Debug.Log("[Part3] Level1 (Solo) wired.");
    }

    // ─────────────────────────────────────────────────────────────
    // BUILD SOLO HUD CANVAS
    // ─────────────────────────────────────────────────────────────
    static HUDManager BuildSoloHUDCanvas(GameObject player)
    {
        var cvGO = GameObject.Find("Canvas_HUD") ?? new GameObject("Canvas_HUD");
        var cv   = cvGO.GetComponent<Canvas>() ?? cvGO.AddComponent<Canvas>();
        cv.renderMode = RenderMode.ScreenSpaceOverlay;
        if (!cvGO.GetComponent<CanvasScaler>())
        {
            var cs = cvGO.AddComponent<CanvasScaler>();
            cs.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cs.referenceResolution = new Vector2(1920, 1080);
        }
        if (!cvGO.GetComponent<GraphicRaycaster>()) cvGO.AddComponent<GraphicRaycaster>();

        var cvTr = cvGO.transform;

        var scoreGO = EnsureTMP(cvTr, "ScoreText",     "Score: 0", 24, new Vector2(-750,  480), new Vector2(300, 50));
        var speedGO = EnsureTMP(cvTr, "SpeedText",      "0 km/h",  24, new Vector2(   0,  480), new Vector2(200, 50));
        var multGO  = EnsureTMP(cvTr, "MultiplierText", "×1",      24, new Vector2(-800, -460), new Vector2(150, 50));
        var toastGO = EnsureTMP(cvTr, "ToastText",      "",        36, new Vector2(   0,  100), new Vector2(600, 70));
        toastGO.SetActive(false);

        var sprHeart = AssetDatabase.LoadAssetAtPath<Sprite>($"{SPRITES}/UI/heart_icon.png");
        var hearts   = new Image[3];
        for (int i = 0; i < 3; i++)
        {
            string hName   = $"Heart_{i}";
            var existingTr = cvTr.Find(hName);
            var hGO        = existingTr ? existingTr.gameObject : new GameObject(hName, typeof(RectTransform));
            hGO.transform.SetParent(cvTr, false);
            var hImg = hGO.GetComponent<Image>() ?? hGO.AddComponent<Image>();
            if (sprHeart) hImg.sprite = sprHeart;
            hImg.color = Color.red;
            var hRT = hGO.GetComponent<RectTransform>();
            hRT.anchoredPosition = new Vector2(700 + i * 55, 480);
            hRT.sizeDelta = new Vector2(40, 40);
            hearts[i] = hImg;
        }

        var hudGO  = GameObject.Find("HUDManager") ?? new GameObject("HUDManager");
        var hudMgr = hudGO.GetComponent<HUDManager>() ?? hudGO.AddComponent<HUDManager>();
        var hudSO  = new SerializedObject(hudMgr);
        SetRef(hudSO, "scoreManager",   player.GetComponent<ScoreManager>());
        SetRef(hudSO, "livesManager",   player.GetComponent<LivesManager>());
        SetRef(hudSO, "playerRb",       player.GetComponent<Rigidbody2D>());
        SetRef(hudSO, "scoreText",      scoreGO.GetComponent<TextMeshProUGUI>());
        SetRef(hudSO, "speedText",      speedGO.GetComponent<TextMeshProUGUI>());
        SetRef(hudSO, "multiplierText", multGO.GetComponent<TextMeshProUGUI>());
        SetRef(hudSO, "toastText",      toastGO.GetComponent<TextMeshProUGUI>());
        var heartsArr = hudSO.FindProperty("heartIcons");
        heartsArr.arraySize = 3;
        for (int i = 0; i < 3; i++)
            heartsArr.GetArrayElementAtIndex(i).objectReferenceValue = hearts[i];
        hudSO.ApplyModifiedProperties();

        return hudMgr;
    }

    // ─────────────────────────────────────────────────────────────
    // WIRE PVP HUD
    // ─────────────────────────────────────────────────────────────
    static void WireHUDPvP(HUDManager_PvP hudPvP, GameObject p1GO, GameObject p2GO)
    {
        var hudSO = new SerializedObject(hudPvP);

        WireHUDPanel(hudSO, "p1", "Canvas_HUD_P1");
        WireHUDPanel(hudSO, "p2", "Canvas_HUD_P2");

        SetRef(hudSO, "p1Rb",           p1GO.GetComponent<Rigidbody2D>());
        SetRef(hudSO, "p2Rb",           p2GO.GetComponent<Rigidbody2D>());
        SetRef(hudSO, "p1ScoreManager", p1GO.GetComponent<ScoreManager>());
        SetRef(hudSO, "p2ScoreManager", p2GO.GetComponent<ScoreManager>());
        SetRef(hudSO, "p1LivesManager", p1GO.GetComponent<LivesManager>());
        SetRef(hudSO, "p2LivesManager", p2GO.GetComponent<LivesManager>());

        hudSO.ApplyModifiedProperties();
    }

    static void WireHUDPanel(SerializedObject hudSO, string prefix, string canvasName)
    {
        var cvGO = GameObject.Find(canvasName);
        if (cvGO == null) return;

        var panel = cvGO.transform.Find("HUDPanel");
        if (panel == null) return;

        // Border image (solid color outline — the first child of HUDPanel)
        SetRef(hudSO, $"{prefix}BorderPanel", panel.Find("Border")?.GetComponent<Image>());

        // TMP elements — names are MakeTMP's text.Replace(" ","_").Replace("\n","")
        foreach (var tmp in panel.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            switch (tmp.name)
            {
                case "Score:_0": SetRef(hudSO, $"{prefix}ScoreText",      tmp); break;
                case "0_km/h":   SetRef(hudSO, $"{prefix}SpeedText",      tmp); break;
                case "×1":       SetRef(hudSO, $"{prefix}MultiplierText", tmp); break;
            }
        }

        // Heart icons
        var heartsArr = hudSO.FindProperty($"{prefix}HeartIcons");
        heartsArr.arraySize = 3;
        for (int i = 0; i < 3; i++)
            heartsArr.GetArrayElementAtIndex(i).objectReferenceValue =
                panel.Find($"Heart_{i}")?.GetComponent<Image>();
    }

    // ─────────────────────────────────────────────────────────────
    // PLAYER COMPONENT WIRING
    // ─────────────────────────────────────────────────────────────
    static void WirePlayerComponents(GameObject pGO, Transform respawn, AudioClip crashClip, string playerTag)
    {
        // AudioSource on player root (used by CrashHandler)
        var pAS = pGO.GetComponent<AudioSource>() ?? pGO.AddComponent<AudioSource>();
        pAS.playOnAwake = false;

        // CrashHandler (lives on Boarder_Top child)
        var crash = pGO.GetComponentInChildren<CrashHandler>();
        if (crash != null)
        {
            var so = new SerializedObject(crash);
            SetRef(so, "livesManager",  pGO.GetComponent<LivesManager>());
            SetRef(so, "trickManager",  pGO.GetComponent<TrickManager>());
            SetRef(so, "audioSource",   pAS);
            SetRef(so, "crashClip",     crashClip);
            SetRef(so, "respawnPoint",  respawn);
            so.ApplyModifiedProperties();
        }

        // TrickManager
        var trick = pGO.GetComponent<TrickManager>();
        if (trick != null)
        {
            var so = new SerializedObject(trick);
            SetRef(so, "groundChecker", pGO.GetComponentInChildren<GroundChecker>());
            SetRef(so, "scoreManager",  pGO.GetComponent<ScoreManager>());
            so.ApplyModifiedProperties();
        }

        // LivesManager player tag
        var lm = pGO.GetComponent<LivesManager>();
        if (lm != null)
        {
            var so = new SerializedObject(lm);
            so.FindProperty("playerTag").stringValue = playerTag;
            so.ApplyModifiedProperties();
        }
    }

    // ─────────────────────────────────────────────────────────────
    // HELPERS
    // ─────────────────────────────────────────────────────────────
    static void WireAudioManager(AudioManager mgr, AudioClip bgmClip)
    {
        var go       = mgr.gameObject;
        var musicSrc = GetOrAddNamedChild<AudioSource>(go, "MusicSource");
        var sfxSrc   = GetOrAddNamedChild<AudioSource>(go, "SFXSource");
        musicSrc.playOnAwake = false;
        sfxSrc.playOnAwake   = false;

        var so = new SerializedObject(mgr);
        SetRef(so, "musicSource", musicSrc);
        SetRef(so, "sfxSource",   sfxSrc);
        SetRef(so, "bgmClip",     bgmClip);
        so.ApplyModifiedProperties();
    }

    static FinishLine EnsureFinishLine()
    {
        var fl = Object.FindObjectOfType<FinishLine>();
        if (fl != null) return fl;
        var go  = new GameObject("FinishLine");
        go.transform.position = new Vector3(0f, -30f, 0f);
        var col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(20f, 1f);
        return go.AddComponent<FinishLine>();
    }

    static void EnsureEventSystem()
    {
        if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() != null) return;
        var es = new GameObject("EventSystem");
        es.AddComponent<UnityEngine.EventSystems.EventSystem>();
        es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
    }

    static T EnsureComponent<T>(string goName) where T : Component
    {
        var existing = Object.FindObjectOfType<T>();
        return existing != null ? existing : new GameObject(goName).AddComponent<T>();
    }

    static GameObject EnsureGO(string name)
        => GameObject.Find(name) ?? new GameObject(name);

    static TComp GetOrAddNamedChild<TComp>(GameObject parent, string childName) where TComp : Component
    {
        var tr = parent.transform.Find(childName);
        if (tr != null) return tr.GetComponent<TComp>() ?? tr.gameObject.AddComponent<TComp>();
        var child = new GameObject(childName);
        child.transform.SetParent(parent.transform, false);
        return child.AddComponent<TComp>();
    }

    static GameObject EnsurePlayerInScene(string name, string prefabPath, Vector3 pos)
    {
        var existing = GameObject.Find(name);
        if (existing != null) return existing;

        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError($"[Part3] Prefab missing: {prefabPath} — run Part 1 first.");
            var fallback = new GameObject(name);
            fallback.transform.position = pos;
            return fallback;
        }

        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.name = name;
        instance.transform.position = pos;
        return instance;
    }

    static GameObject EnsureTMP(Transform parent, string name, string text,
                                  int fontSize, Vector2 pos, Vector2 size)
    {
        var existing = parent.Find(name);
        if (existing != null) return existing.gameObject;

        var go  = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text      = text;
        tmp.fontSize  = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color     = Color.white;
        var rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta        = size;
        return go;
    }

    // Scene-file existence check (avoids errors if Part 2 hasn't run yet)
    static bool SceneExists(string sceneName)
    {
        string path = $"{SCENES}/{sceneName}.unity";
        bool exists = System.IO.File.Exists(
            path.Replace("Assets/", Application.dataPath + "/"));
        if (!exists) Debug.LogWarning($"[Part3] {sceneName}.unity not found — run Part 2 first.");
        return exists;
    }

    // SerializedObject helpers
    static void SetRef(SerializedObject so, string prop, Object value)
    {
        var p = so.FindProperty(prop);
        if (p != null) p.objectReferenceValue = value;
        else Debug.LogWarning($"[Part3] SerializedProperty '{prop}' not found on {so.targetObject}");
    }

    static void SetField(Object target, string field, Object value)
    {
        var so = new SerializedObject(target);
        var p  = so.FindProperty(field);
        if (p != null) { p.objectReferenceValue = value; so.ApplyModifiedProperties(); }
    }

    static Button FindBtn(string name) => GameObject.Find(name)?.GetComponent<Button>();

    static TextMeshProUGUI FindTMP(string name)
        => GameObject.Find(name)?.GetComponent<TextMeshProUGUI>();

    static AudioClip LoadAudio(string filename)
        => AssetDatabase.LoadAssetAtPath<AudioClip>($"{AUDIO}/{filename}");
}

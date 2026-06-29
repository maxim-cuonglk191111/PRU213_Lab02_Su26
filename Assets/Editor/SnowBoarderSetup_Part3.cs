using UnityEngine;
using UnityEditor;
using UnityEditor.Events;
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
        WireLevel1PvP();
        WireLevel1Solo();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Done", "Part 3 complete — all scenes and prefabs are wired.", "OK");
        Debug.Log("=== [Part3] Scene wiring complete ===");
    }

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

    static void WireMainMenu()
    {
        if (!SceneExists("MainMenu")) return;
        var scene = EditorSceneManager.OpenScene($"{SCENES}/MainMenu.unity", OpenSceneMode.Single);

        var audioMgr = EnsureComponent<AudioManager>("AudioManager");
        WireAudioManager(audioMgr, LoadAudio("bgm_snow.ogg"));

        var mmm = Object.FindAnyObjectByType<MainMenuManager>();
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

    static void WireModeSelect()
    {
        if (!SceneExists("ModeSelect")) return;
        var scene = EditorSceneManager.OpenScene($"{SCENES}/ModeSelect.unity", OpenSceneMode.Single);

        var msm = Object.FindAnyObjectByType<ModeSelectManager>();
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

    static void WireScoreSummary()
    {
        if (!SceneExists("ScoreSummary")) return;
        var scene = EditorSceneManager.OpenScene($"{SCENES}/ScoreSummary.unity", OpenSceneMode.Single);

        var mgr = Object.FindAnyObjectByType<ScoreSummaryManager>();
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

    static void WirePvPSummary()
    {
        if (!SceneExists("PvPSummary")) return;
        var scene = EditorSceneManager.OpenScene($"{SCENES}/PvPSummary.unity", OpenSceneMode.Single);

        var mgr = Object.FindAnyObjectByType<PvPSummaryManager>();
        if (mgr != null)
        {
            var so = new SerializedObject(mgr);
            SetRef(so, "winnerText",     FindTMP("Player_1_Wins!"));
            SetRef(so, "p1ScoreText",    FindTMP("P1_Score:_0"));
            SetRef(so, "p2ScoreText",    FindTMP("P2_Score:_0"));
            SetRef(so, "rematchButton",  FindBtn("Rematch_Btn"));
            SetRef(so, "mainMenuButton", FindBtn("Main Menu_Btn"));
            so.ApplyModifiedProperties();
        }

        EditorSceneManager.SaveScene(scene);
        Debug.Log("[Part3] PvPSummary wired.");
    }

    static void WireLevel1PvP()
    {
        if (!SceneExists("Level1_PvP")) return;
        var scene = EditorSceneManager.OpenScene($"{SCENES}/Level1_PvP.unity", OpenSceneMode.Single);

        var crashClip  = LoadAudio("Crash SFX.ogg");
        var finishClip = LoadAudio("Finish SFX.ogg");
        var pickupClip = LoadAudio("pickup.ogg");
        var bgmClip    = LoadAudio("bgm_snow.ogg");

        var strayGM = Object.FindAnyObjectByType<GameManager>();
        if (strayGM != null) Object.DestroyImmediate(strayGM.gameObject);

        WireAudioManager(EnsureComponent<AudioManager>("AudioManager"), bgmClip);
        EnsureEventSystem();

        // Adopt the original scene player first, then position spawn points relative to it
        var p1GO = EnsurePlayerInScene("Player1", $"{PREFABS}/Player1.prefab", new Vector3(-43.5f, 14f, 0f));

        var sp1 = GameObject.Find("SpawnPoint_P1");
        if (sp1 == null) sp1 = new GameObject("SpawnPoint_P1");
        sp1.transform.position = p1GO.transform.position + Vector3.up * 3f;

        var sp2 = GameObject.Find("SpawnPoint_P2");
        if (sp2 == null) sp2 = new GameObject("SpawnPoint_P2");
        sp2.transform.position = p1GO.transform.position + new Vector3(3f, 3f, 0f);

        var p2GO = EnsurePlayerInScene("Player2", $"{PREFABS}/Player2.prefab", sp2.transform.position);

        WirePlayerComponents(p1GO, sp1.transform, crashClip, "Player");
        WirePlayerComponents(p2GO, sp2.transform, crashClip, "Player2");

        var pvp = Object.FindAnyObjectByType<PvPGameManager>();
        if (pvp == null) pvp = new GameObject("PvPGameManager").AddComponent<PvPGameManager>();
        var pvpSO = new SerializedObject(pvp);
        SetRef(pvpSO, "player1",       p1GO);
        SetRef(pvpSO, "player2",       p2GO);
        SetRef(pvpSO, "livesManager1", p1GO.GetComponent<LivesManager>());
        SetRef(pvpSO, "livesManager2", p2GO.GetComponent<LivesManager>());
        SetRef(pvpSO, "scoreManager1", p1GO.GetComponent<ScoreManager>());
        SetRef(pvpSO, "scoreManager2", p2GO.GetComponent<ScoreManager>());
        pvpSO.ApplyModifiedProperties();

        var hudPvP = Object.FindAnyObjectByType<HUDManager_PvP>();
        if (hudPvP != null) WireHUDPvP(hudPvP, p1GO, p2GO);

        var fl = Object.FindAnyObjectByType<FinishLine>();
        if (fl != null)
        {
            var flAS = fl.gameObject.GetComponent<AudioSource>();
            if (flAS == null) flAS = fl.gameObject.AddComponent<AudioSource>();
            flAS.playOnAwake = false;
            var flSO = new SerializedObject(fl);
            flSO.FindProperty("isPvP").boolValue = true;
            SetRef(flSO, "audioSource", flAS);
            SetRef(flSO, "finishClip",  finishClip);
            flSO.ApplyModifiedProperties();
        }

        // PvP uses manual split-screen cameras — disable Cinemachine to prevent conflicts
        foreach (var mb in Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (mb == null) continue;
            var tn = mb.GetType().FullName;
            if (tn.Contains("CinemachineBrain") || tn.Contains("CinemachineVirtualCamera"))
            {
                var cso = new SerializedObject(mb);
                cso.FindProperty("m_Enabled").boolValue = false;
                cso.ApplyModifiedProperties();
                Debug.Log($"[Part3] Disabled {tn} for PvP split-screen");
            }
        }

        void WireCam(string camName, GameObject playerTarget, string pTag)
        {
            var camGO = GameObject.Find(camName);
            if (camGO == null) return;
            var cf   = camGO.GetComponent<CameraFollow>() ?? camGO.AddComponent<CameraFollow>();
            var cfSO = new SerializedObject(cf);
            SetRef(cfSO, "target", playerTarget.transform);
            cfSO.FindProperty("playerTag").stringValue = pTag;
            cfSO.ApplyModifiedProperties();

            Vector3 off     = new Vector3(0f, 2f, -10f);
            Vector3 snapPos = playerTarget.transform.position + off;
            snapPos.z = off.z;
            camGO.transform.position = snapPos;
        }

        WireCam("Main Camera", p1GO, "Player");
        WireCam("Camera2",     p2GO, "Player2");

        var pvpCam1 = GameObject.Find("Main Camera")?.GetComponent<Camera>();
        if (pvpCam1 != null) ApplySharedCamSettings(pvpCam1, 0, new Rect(0f, 0f, 0.5f, 1f));
        var pvpCam2 = GameObject.Find("Camera2")?.GetComponent<Camera>();
        if (pvpCam2 != null) ApplySharedCamSettings(pvpCam2, 1, new Rect(0.5f, 0f, 0.5f, 1f));

        foreach (var ph in Object.FindObjectsByType<PickupHandler>(FindObjectsInactive.Exclude))
            SetField(ph, "pickupClip", pickupClip);

        EnsureTerrainTag();
        BuildPausePanel();

        EditorSceneManager.SaveScene(scene);
        Debug.Log("[Part3] Level1_PvP wired.");
    }

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

        foreach (var go in GameObject.FindGameObjectsWithTag("Player2"))
            Object.DestroyImmediate(go);
        var strayP2 = GameObject.Find("Player2");
        if (strayP2 != null) Object.DestroyImmediate(strayP2);
        var strayPvP = Object.FindAnyObjectByType<PvPGameManager>();
        if (strayPvP != null) Object.DestroyImmediate(strayPvP.gameObject);
        foreach (var name in new[] { "Camera2", "Canvas_Divider", "Canvas_HUD_P1", "Canvas_HUD_P2", "HUDManager_PvP", "SpawnPoint_P1", "SpawnPoint_P2" })
        {
            var found = GameObject.Find(name);
            if (found != null) Object.DestroyImmediate(found);
        }
        var mainCamSolo = Object.FindAnyObjectByType<Camera>();
        if (mainCamSolo != null)
        {
            mainCamSolo.rect        = new Rect(0f, 0f, 1f, 1f);
            mainCamSolo.clearFlags  = CameraClearFlags.SolidColor;
            mainCamSolo.backgroundColor = new Color(0.52f, 0.80f, 0.98f, 1f);
            mainCamSolo.orthographic = true;
            mainCamSolo.orthographicSize = 5f;
        }

        // Find or adopt the existing player first, then anchor respawn above it
        var p1GO = EnsurePlayerInScene("Player1", $"{PREFABS}/Player1.prefab", new Vector3(-43.5f, 14f, 0f));

        var respawn = GameObject.Find("RespawnPoint");
        if (respawn == null) respawn = new GameObject("RespawnPoint");
        respawn.transform.position = p1GO.transform.position + Vector3.up * 3f;

        WirePlayerComponents(p1GO, respawn.transform, crashClip, "Player");

        var hud = BuildSoloHUDCanvas(p1GO);

        var trick = p1GO.GetComponent<TrickManager>();
        if (trick != null) SetField(trick, "hudManager", hud);

        var fl   = EnsureFinishLine();
        var flAS = fl.gameObject.GetComponent<AudioSource>();
        if (flAS == null) flAS = fl.gameObject.AddComponent<AudioSource>();
        flAS.playOnAwake = false;
        var flSO = new SerializedObject(fl);
        flSO.FindProperty("isPvP").boolValue = false;
        SetRef(flSO, "audioSource", flAS);
        SetRef(flSO, "finishClip",  finishClip);
        flSO.ApplyModifiedProperties();

        foreach (var ph in Object.FindObjectsByType<PickupHandler>(FindObjectsInactive.Exclude))
            SetField(ph, "pickupClip", pickupClip);

        // Solo: remove stale CameraFollow from cameras (Cinemachine already follows the player)
        foreach (var cam in Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            var stale = cam.GetComponent<CameraFollow>();
            if (stale != null) Object.DestroyImmediate(stale);
        }

        EnsureTerrainTag();
        BuildPausePanel();

        EditorSceneManager.SaveScene(scene);
        Debug.Log("[Part3] Level1 (Solo) wired.");
    }

    static HUDManager BuildSoloHUDCanvas(GameObject player)
    {
        var cvGO = GameObject.Find("Canvas_HUD");
        if (cvGO == null) cvGO = new GameObject("Canvas_HUD");
        var cv = cvGO.GetComponent<Canvas>();
        if (cv == null) cv = cvGO.AddComponent<Canvas>();
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
            var hImg = hGO.GetComponent<Image>();
            if (hImg == null) hImg = hGO.AddComponent<Image>();
            if (sprHeart) hImg.sprite = sprHeart;
            hImg.color = Color.red;
            var hRT = hGO.GetComponent<RectTransform>();
            hRT.anchoredPosition = new Vector2(700 + i * 55, 480);
            hRT.sizeDelta = new Vector2(40, 40);
            hearts[i] = hImg;
        }

        var hudGO  = GameObject.Find("HUDManager");
        if (hudGO == null) hudGO = new GameObject("HUDManager");
        var hudMgr = hudGO.GetComponent<HUDManager>();
        if (hudMgr == null) hudMgr = hudGO.AddComponent<HUDManager>();
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

        SetRef(hudSO, $"{prefix}BorderPanel", panel.Find("Border")?.GetComponent<Image>());

        foreach (var tmp in panel.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            switch (tmp.name)
            {
                case "Score:_0": SetRef(hudSO, $"{prefix}ScoreText",      tmp); break;
                case "0_km/h":   SetRef(hudSO, $"{prefix}SpeedText",      tmp); break;
                case "×1":       SetRef(hudSO, $"{prefix}MultiplierText", tmp); break;
            }
        }

        var heartsArr = hudSO.FindProperty($"{prefix}HeartIcons");
        heartsArr.arraySize = 3;
        for (int i = 0; i < 3; i++)
            heartsArr.GetArrayElementAtIndex(i).objectReferenceValue =
                panel.Find($"Heart_{i}")?.GetComponent<Image>();
    }

    static void WirePlayerComponents(GameObject pGO, Transform respawn, AudioClip crashClip, string playerTag)
    {
        var rb = pGO.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale           = 2.5f;
            rb.linearDamping          = 0f;
            rb.angularDamping         = 4f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation          = RigidbodyInterpolation2D.Interpolate;
            EditorUtility.SetDirty(rb);
        }

        // Add scripts if missing (reference scripts use singletons — no cross-refs to wire)
        if (!pGO.GetComponent<PlayerController>())        pGO.AddComponent<PlayerController>();
        if (!pGO.GetComponent<TrickManager>())            pGO.AddComponent<TrickManager>();
        if (!pGO.GetComponentInChildren<CrashHandler>())  pGO.AddComponent<CrashHandler>();

        // LivesManager is our custom addition for lives/PvP support
        if (!pGO.GetComponent<LivesManager>())            pGO.AddComponent<LivesManager>();
        var lm = pGO.GetComponent<LivesManager>();
        if (lm != null)
        {
            var so = new SerializedObject(lm);
            so.FindProperty("playerTag").stringValue = playerTag;
            so.ApplyModifiedProperties();
        }
    }

    static void WireAudioManager(AudioManager mgr, AudioClip clip)
    {
        var go      = mgr.gameObject;
        var bgmSrc  = GetOrAddNamedChild<AudioSource>(go, "BGMSource");
        var sfxSrc  = GetOrAddNamedChild<AudioSource>(go, "SFXSource");
        var legacyMusicSrc = GetOrAddNamedChild<AudioSource>(go, "MusicSource");
        bgmSrc.playOnAwake         = false;
        sfxSrc.playOnAwake         = false;
        legacyMusicSrc.playOnAwake = false;

        var so = new SerializedObject(mgr);
        SetRef(so, "bgmSource",   bgmSrc);
        SetRef(so, "sfxSource",   sfxSrc);
        SetRef(so, "musicSource", legacyMusicSrc);
        // Wire the BGM clip to all level slots so our single bgm_snow.ogg plays everywhere
        SetRef(so, "menuBGM",   clip);
        SetRef(so, "level1BGM", clip);
        SetRef(so, "level2BGM", clip);
        SetRef(so, "level3BGM", clip);
        so.ApplyModifiedProperties();
    }

    static FinishLine EnsureFinishLine()
    {
        var fl = Object.FindAnyObjectByType<FinishLine>();
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
        if (Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() != null) return;
        var es = new GameObject("EventSystem");
        es.AddComponent<UnityEngine.EventSystems.EventSystem>();
        es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
    }

    static T EnsureComponent<T>(string goName) where T : Component
    {
        var existing = Object.FindAnyObjectByType<T>();
        return existing != null ? existing : new GameObject(goName).AddComponent<T>();
    }

    static GameObject EnsureGO(string name)
    {
        var go = GameObject.Find(name);
        return go != null ? go : new GameObject(name);
    }

    static TComp GetOrAddNamedChild<TComp>(GameObject parent, string childName) where TComp : Component
    {
        var tr = parent.transform.Find(childName);
        if (tr != null)
        {
            var comp = tr.GetComponent<TComp>();
            return comp != null ? comp : tr.gameObject.AddComponent<TComp>();
        }
        var child = new GameObject(childName);
        child.transform.SetParent(parent.transform, false);
        return child.AddComponent<TComp>();
    }

    static GameObject EnsurePlayerInScene(string name, string prefabPath, Vector3 fallbackPos)
    {
        var byName = GameObject.Find(name);
        if (byName != null) return byName;

        // Adopt the first Rigidbody2D with the correct tag (e.g. the original "Barry" tagged "Player")
        string tag = (name == "Player2") ? "Player2" : "Player";
        foreach (var rb in Object.FindObjectsByType<Rigidbody2D>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (!rb.gameObject.CompareTag(tag)) continue;
            if (name == "Player1" && rb.gameObject.name == "Player2") continue;
            if (name == "Player2" && rb.gameObject.name == "Player1") continue;
            rb.gameObject.name = name;
            Debug.Log($"[Part3] Adopted existing '{tag}' object as {name}");
            return rb.gameObject;
        }

        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError($"[Part3] Prefab missing: {prefabPath} — run Part 1 first.");
            var fallback = new GameObject(name);
            fallback.transform.position = fallbackPos;
            return fallback;
        }
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.name = name;
        instance.transform.position = fallbackPos;
        Debug.Log($"[Part3] Created new {name} at {fallbackPos}");
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

    static void BuildPausePanel()
    {
        var cvGO = EnsureGO("Canvas_Pause");
        var cv   = cvGO.GetComponent<Canvas>() ?? cvGO.AddComponent<Canvas>();
        cv.renderMode   = RenderMode.ScreenSpaceOverlay;
        cv.sortingOrder = 100;
        if (!cvGO.GetComponent<CanvasScaler>())
        {
            var cs = cvGO.AddComponent<CanvasScaler>();
            cs.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cs.referenceResolution = new Vector2(1920, 1080);
        }
        if (!cvGO.GetComponent<GraphicRaycaster>()) cvGO.AddComponent<GraphicRaycaster>();

        var cvTr = cvGO.transform;

        var panelTr = cvTr.Find("PausePanel");
        var panelGO = panelTr != null ? panelTr.gameObject : new GameObject("PausePanel", typeof(RectTransform));
        panelGO.transform.SetParent(cvTr, false);

        var panelImg = panelGO.GetComponent<Image>() ?? panelGO.AddComponent<Image>();
        panelImg.color = new Color(0f, 0f, 0f, 0.7f);
        var panelRT = panelGO.GetComponent<RectTransform>();
        panelRT.anchorMin = Vector2.zero;
        panelRT.anchorMax = Vector2.one;
        panelRT.offsetMin = Vector2.zero;
        panelRT.offsetMax = Vector2.zero;

        var titleGO  = EnsureTMP(panelGO.transform, "PauseTitle", "PAUSED", 72, new Vector2(0, 150), new Vector2(600, 100));
        var titleTMP = titleGO.GetComponent<TextMeshProUGUI>();
        titleTMP.fontStyle = FontStyles.Bold;

        var resumeBtn   = EnsureButton(panelGO.transform, "Resume_Btn",   "Resume",    new Vector2(0,  20));
        var mainMenuBtn = EnsureButton(panelGO.transform, "MainMenu_Btn", "Main Menu", new Vector2(0, -80));

        var ppUI = panelGO.GetComponent<PausePanelUI>() ?? panelGO.AddComponent<PausePanelUI>();
        if (resumeBtn.onClick.GetPersistentEventCount() == 0)
            UnityEventTools.AddPersistentListener(resumeBtn.onClick,   ppUI.Resume);
        if (mainMenuBtn.onClick.GetPersistentEventCount() == 0)
            UnityEventTools.AddPersistentListener(mainMenuBtn.onClick, ppUI.GoToMainMenu);

        panelGO.SetActive(false);
    }

    static Button EnsureButton(Transform parent, string name, string label, Vector2 pos)
    {
        var existing = parent.Find(name);
        if (existing != null) return existing.GetComponent<Button>();

        var btnGO = new GameObject(name, typeof(RectTransform));
        btnGO.transform.SetParent(parent, false);
        var img = btnGO.AddComponent<Image>();
        img.color = new Color(0.15f, 0.15f, 0.15f, 1f);
        var btn = btnGO.AddComponent<Button>();
        var rt  = btnGO.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta        = new Vector2(280, 60);

        var labelGO = new GameObject("Label", typeof(RectTransform));
        labelGO.transform.SetParent(btnGO.transform, false);
        var tmp = labelGO.AddComponent<TextMeshProUGUI>();
        tmp.text      = label;
        tmp.fontSize  = 28;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color     = Color.white;
        var lrt = labelGO.GetComponent<RectTransform>();
        lrt.anchorMin = Vector2.zero;
        lrt.anchorMax = Vector2.one;
        lrt.offsetMin = Vector2.zero;
        lrt.offsetMax = Vector2.zero;

        return btn;
    }

    // Tag the terrain collider object as "Ground" so CrashHandler.OnTriggerEnter2D fires correctly.
    static void EnsureTerrainTag()
    {
        EnsureTagExists("Ground");
        // SpriteShape terrain: find any object with EdgeCollider2D or SurfaceEffector2D on it
        foreach (var col in Object.FindObjectsByType<Collider2D>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            var go = col.gameObject;
            if (go.tag == "Ground") continue;
            bool isTerrain = (col is EdgeCollider2D) ||
                             go.GetComponent<SurfaceEffector2D>() != null ||
                             go.name.Contains("Level") ||
                             go.name.Contains("Ground") ||
                             go.name.Contains("Slope");
            if (!isTerrain) continue;
            go.tag = "Ground";
            EditorUtility.SetDirty(go);
            Debug.Log($"[Part3] Tagged '{go.name}' as Ground");
        }
    }

    static void EnsureTagExists(string tag)
    {
        var asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if (asset == null || asset.Length == 0) return;
        var so = new SerializedObject(asset[0]);
        var tags = so.FindProperty("tags");
        for (int i = 0; i < tags.arraySize; i++)
            if (tags.GetArrayElementAtIndex(i).stringValue == tag) return;
        tags.InsertArrayElementAtIndex(tags.arraySize);
        tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = tag;
        so.ApplyModifiedProperties();
    }

    static void ApplySharedCamSettings(Camera cam, int depth, Rect rect)
    {
        cam.clearFlags       = CameraClearFlags.SolidColor;
        cam.backgroundColor  = new Color(0.52f, 0.80f, 0.98f, 1f);
        cam.orthographic     = true;
        cam.orthographicSize = 5f;
        cam.depth            = depth;
        cam.rect             = rect;
    }

    static bool SceneExists(string sceneName)
    {
        string path   = $"{SCENES}/{sceneName}.unity";
        bool   exists = System.IO.File.Exists(path.Replace("Assets/", Application.dataPath + "/"));
        if (!exists) Debug.LogWarning($"[Part3] {sceneName}.unity not found — run Part 2 first.");
        return exists;
    }

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

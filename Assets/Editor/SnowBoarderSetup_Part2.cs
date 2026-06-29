using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

public static class SnowBoarderSetup_Part2
{
    const string SCENES  = "Assets/Scenes";
    const string SPRITES = "Assets/Sprites";
    const string P1_COLOR_HEX = "#00AAFF";
    const string P2_COLOR_HEX = "#FF6B35";

    static readonly Color P1Color = new Color(0f,    0.667f, 1f,    1f);
    static readonly Color P2Color = new Color(1f,    0.420f, 0.208f, 1f);

    [MenuItem("SnowBoarder/Setup/Part 2 – Scenes")]
    public static void RunPart2()
    {
        BuildMainMenuScene();
        BuildModeSelectScene();
        BuildScoreSummaryScene();
        BuildPvPSummaryScene();
        BuildLevel1PvPScene();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[Part2] All scenes created.");
    }

    static UnityEngine.SceneManagement.Scene NewScene(string name)
    {
        return EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
    }

    static void SaveScene(UnityEngine.SceneManagement.Scene scene, string name)
    {
        EditorSceneManager.SaveScene(scene, $"{SCENES}/{name}.unity");
        Debug.Log($"[Part2] Saved {name}.unity");
    }

    static GameObject MakeCanvas(string cname = "Canvas")
    {
        var cvGO = new GameObject(cname);
        var cv   = cvGO.AddComponent<Canvas>();
        cv.renderMode = RenderMode.ScreenSpaceOverlay;
        var cs = cvGO.AddComponent<CanvasScaler>();
        cs.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920, 1080);
        cvGO.AddComponent<GraphicRaycaster>();

        var esGO = new GameObject("EventSystem");
        esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
        esGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        return cvGO;
    }

    static GameObject MakeTMP(Transform parent, string text, int fontSize, Vector2 pos, Vector2 size)
    {
        var go = new GameObject(text.Replace(" ","_").Replace("\n",""), typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.raycastTarget = false;
        tmp.text      = text;
        tmp.fontSize  = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color     = Color.white;
        var rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta        = size;
        return go;
    }

    static Button MakeButton(Transform parent, string label, Vector2 pos, Vector2 size = default)
    {
        if (size == default) size = new Vector2(300, 70);
        var go  = new GameObject(label+"_Btn", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        var btn  = go.AddComponent<Button>();
        btn.targetGraphic = img;
        var rt   = go.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta        = size;
        MakeTMP(go.transform, label, 24, Vector2.zero, size);
        return btn;
    }

    static GameObject MakePanel(Transform parent, string name, Color color, Vector2 pos, Vector2 size)
    {
        var go  = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = color;
        var rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta        = size;
        return go;
    }

    static void BuildMainMenuScene()
    {
        var scene = NewScene("MainMenu");
        SetSkyBg();

        var cv   = MakeCanvas();
        var cvTr = cv.transform;

        MakeTMP(cvTr, "Snow Boarder", 72, new Vector2(0, 260), new Vector2(800, 120));
        MakeButton(cvTr, "Start",      new Vector2(0,  90));
        MakeButton(cvTr, "HowToPlay",  new Vector2(0,  10));
        MakeButton(cvTr, "Options",    new Vector2(0, -70));
        MakeButton(cvTr, "Quit",       new Vector2(0,-150));

        // Guide panel — controls reference, inspired by reference project's guide screen
        var guidePanel = MakePanel(cvTr, "GuidePanel", new Color(0.05f,0.05f,0.1f,0.96f), Vector2.zero, new Vector2(700,420));
        guidePanel.SetActive(false);
        MakeTMP(guidePanel.transform, "How to Play",    28, new Vector2(0,  160), new Vector2(600, 50)).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        MakeTMP(guidePanel.transform, "← → Arrow Keys: Rotate",    22, new Vector2(0,  90), new Vector2(600, 40));
        MakeTMP(guidePanel.transform, "Hold direction to accelerate", 20, new Vector2(0,  40), new Vector2(600, 40));
        MakeTMP(guidePanel.transform, "Collect snowflakes for bonus score", 20, new Vector2(0, -10), new Vector2(620, 40));
        MakeTMP(guidePanel.transform, "Avoid crashing into obstacles",      20, new Vector2(0, -60), new Vector2(620, 40));
        MakeTMP(guidePanel.transform, "PvP: P1 = Arrow Keys | P2 = WASD",  20, new Vector2(0,-110), new Vector2(640, 40));
        MakeButton(guidePanel.transform, "Close", new Vector2(0,-175), new Vector2(180, 50));

        var optPanel = MakePanel(cvTr, "OptionsPanel", new Color(0.1f,0.1f,0.1f,0.95f), Vector2.zero, new Vector2(500,300));
        optPanel.SetActive(false);
        MakeTMP(optPanel.transform, "SFX Volume",   20, new Vector2(0,  80), new Vector2(400, 40));
        MakeTMP(optPanel.transform, "Music Volume", 20, new Vector2(0,  20), new Vector2(400, 40));

        new GameObject("MainMenuManager").AddComponent<MainMenuManager>();

        var cam = new GameObject("Main Camera");
        cam.AddComponent<Camera>().backgroundColor = new Color(0.52f, 0.80f, 0.98f);
        cam.AddComponent<AudioListener>();
        cam.tag = "MainCamera";

        SaveScene(scene, "MainMenu");
    }

    static void BuildModeSelectScene()
    {
        var scene = NewScene("ModeSelect");
        SetSkyBg();

        var cvTr = MakeCanvas().transform;
        MakeTMP(cvTr, "Select Mode", 56, new Vector2(0, 200), new Vector2(600, 100));
        MakeButton(cvTr, "Solo", new Vector2(0,  50), new Vector2(320, 80));
        MakeButton(cvTr, "PvP",  new Vector2(0, -50), new Vector2(320, 80));
        MakeButton(cvTr, "Back", new Vector2(0,-150), new Vector2(200, 60));
        new GameObject("ModeSelectManager").AddComponent<ModeSelectManager>();
        AddCamera();
        SaveScene(scene, "ModeSelect");
    }

    static void BuildScoreSummaryScene()
    {
        var scene = NewScene("ScoreSummary");
        SetSkyBg();
        var cvTr = MakeCanvas().transform;
        MakeTMP(cvTr, "Run Complete!",  52, new Vector2(0,  240), new Vector2(700, 100));
        MakeTMP(cvTr, "Your Score: 0", 36, new Vector2(0,  130), new Vector2(600,  70));
        MakeTMP(cvTr, "Best Score: 0", 28, new Vector2(0,   60), new Vector2(600,  60));
        MakeTMP(cvTr, "Time: 00:00",   26, new Vector2(-150, -10), new Vector2(280,  55));
        MakeTMP(cvTr, "Distance: 0m",  26, new Vector2( 150, -10), new Vector2(280,  55));
        MakeButton(cvTr, "Replay",    new Vector2(-180, -130));
        MakeButton(cvTr, "Main Menu", new Vector2( 180, -130));
        new GameObject("ScoreSummaryManager").AddComponent<ScoreSummaryManager>();
        AddCamera();
        SaveScene(scene, "ScoreSummary");
    }

    static void BuildPvPSummaryScene()
    {
        var scene = NewScene("PvPSummary");
        SetSkyBg();
        var cvTr = MakeCanvas().transform;
        MakeTMP(cvTr, "Player 1 Wins!", 48, new Vector2(0,  220), new Vector2(800, 100));
        MakeTMP(cvTr, "P1 Score: 0",    32, new Vector2(-200, 80), new Vector2(350,  70));
        MakeTMP(cvTr, "P2 Score: 0",    32, new Vector2( 200, 80), new Vector2(350,  70));
        MakeButton(cvTr, "Rematch",   new Vector2(-180, -100));
        MakeButton(cvTr, "Main Menu", new Vector2( 180, -100));
        new GameObject("PvPSummaryManager").AddComponent<PvPSummaryManager>();
        AddCamera();
        SaveScene(scene, "PvPSummary");
    }

    static void BuildLevel1PvPScene()
    {
        string level1Path = $"{SCENES}/Level1.unity";
        UnityEngine.SceneManagement.Scene scene;
        if (System.IO.File.Exists(level1Path.Replace("Assets/", Application.dataPath+"/")))
            scene = EditorSceneManager.OpenScene(level1Path, OpenSceneMode.Single);
        else
            scene = NewScene("Level1_PvP");

        var sp1 = new GameObject("SpawnPoint_P1");
        sp1.transform.position = new Vector3(-1f, 2f, 0f);

        var sp2 = new GameObject("SpawnPoint_P2");
        sp2.transform.position = new Vector3(1f, 2f, 0f);

        new GameObject("PvPGameManager").AddComponent<PvPGameManager>();

        var mainCam = Object.FindAnyObjectByType<Camera>();
        if (mainCam == null)
        {
            var camGO = new GameObject("Main Camera");
            mainCam = camGO.AddComponent<Camera>();
            camGO.AddComponent<AudioListener>();
            camGO.tag = "MainCamera";
        }
        ApplySharedCamSettings(mainCam, 0, new Rect(0f, 0f, 0.5f, 1f));

        var cam2GO = new GameObject("Camera2");
        var cam2   = cam2GO.AddComponent<Camera>();
        ApplySharedCamSettings(cam2, 1, new Rect(0.5f, 0f, 0.5f, 1f));

        var divCanvas = new GameObject("Canvas_Divider");
        var dc = divCanvas.AddComponent<Canvas>();
        dc.renderMode   = RenderMode.ScreenSpaceOverlay;
        dc.sortingOrder = 99;
        divCanvas.AddComponent<GraphicRaycaster>();

        var line = new GameObject("SplitLineDivider", typeof(RectTransform));
        line.transform.SetParent(divCanvas.transform, false);
        var lineImg = line.AddComponent<Image>();
        lineImg.color = new Color(1f, 1f, 1f, 0.8f);
        var lineRT = line.GetComponent<RectTransform>();
        lineRT.anchorMin = new Vector2(0.5f, 0f);
        lineRT.anchorMax = new Vector2(0.5f, 1f);
        lineRT.sizeDelta = new Vector2(2f, 0f);
        lineRT.anchoredPosition = Vector2.zero;
        line.AddComponent<SplitScreenDivider>();

        BuildPvPHudCanvas("Canvas_HUD_P1", mainCam, P1Color, "P1", new Vector2(-450, 30));
        BuildPvPHudCanvas("Canvas_HUD_P2", cam2,    P2Color, "P2", new Vector2(-450, 30));

        new GameObject("HUDManager_PvP").AddComponent<HUDManager_PvP>();

        BuildControlsOverlay();

        var fl = new GameObject("FinishLine");
        fl.transform.position = new Vector3(0f, -30f, 0f);
        var flCol = fl.AddComponent<BoxCollider2D>();
        flCol.isTrigger = true;
        flCol.size = new Vector2(20f, 1f);
        var flScript = fl.AddComponent<FinishLine>();
        var so = new SerializedObject(flScript);
        so.FindProperty("isPvP").boolValue = true;
        so.ApplyModifiedProperties();

        SaveScene(scene, "Level1_PvP");
    }

    static void BuildPvPHudCanvas(string name, Camera cam, Color borderColor, string playerLabel, Vector2 pos)
    {
        var cvGO = new GameObject(name);
        var cv   = cvGO.AddComponent<Canvas>();
        cv.renderMode    = RenderMode.ScreenSpaceCamera;
        cv.worldCamera   = cam;
        cv.planeDistance = 1f;
        var cs = cvGO.AddComponent<CanvasScaler>();
        cs.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(960, 1080);
        cvGO.AddComponent<GraphicRaycaster>();

        var panel = new GameObject("HUDPanel", typeof(RectTransform));
        panel.transform.SetParent(cvGO.transform, false);
        var pImg = panel.AddComponent<Image>();
        pImg.color = new Color(borderColor.r, borderColor.g, borderColor.b, 0.15f);
        var pRT = panel.GetComponent<RectTransform>();
        pRT.anchorMin        = new Vector2(0f, 1f);
        pRT.anchorMax        = new Vector2(0f, 1f);
        pRT.pivot            = new Vector2(0f, 1f);
        pRT.anchoredPosition = new Vector2(10f, -10f);
        pRT.sizeDelta        = new Vector2(280f, 160f);

        var border = new GameObject("Border", typeof(RectTransform));
        border.transform.SetParent(panel.transform, false);
        var bImg = border.AddComponent<Image>();
        bImg.color = borderColor;
        var bRT = border.GetComponent<RectTransform>();
        bRT.anchorMin = Vector2.zero; bRT.anchorMax = Vector2.one;
        bRT.offsetMin = new Vector2(-3f, -3f);
        bRT.offsetMax = new Vector2(3f, 3f);
        border.transform.SetAsFirstSibling();

        MakeTMP(panel.transform, playerLabel, 20, new Vector2(0f,  -15f), new Vector2(270, 30));
        MakeTMP(panel.transform, "Score: 0",  16, new Vector2(0f,  -50f), new Vector2(270, 25));
        MakeTMP(panel.transform, "0 km/h",    16, new Vector2(0f,  -75f), new Vector2(270, 25));
        MakeTMP(panel.transform, "×1",        16, new Vector2(0f, -100f), new Vector2(270, 25));

        for (int i = 0; i < 3; i++)
        {
            var h = new GameObject($"Heart_{i}", typeof(RectTransform));
            h.transform.SetParent(panel.transform, false);
            var hImg = h.AddComponent<Image>();
            var hSpr = AssetDatabase.LoadAssetAtPath<Sprite>($"{SPRITES}/UI/heart_icon.png");
            if (hSpr) hImg.sprite = hSpr;
            hImg.color = Color.red;
            var hRT = h.GetComponent<RectTransform>();
            hRT.anchoredPosition = new Vector2(-80f + i * 35f, -130f);
            hRT.sizeDelta        = new Vector2(28f, 28f);
        }
    }

    static void BuildControlsOverlay()
    {
        var cvGO = new GameObject("Canvas_ControlsOverlay");
        var cv   = cvGO.AddComponent<Canvas>();
        cv.renderMode   = RenderMode.ScreenSpaceOverlay;
        cv.sortingOrder = 50;
        cvGO.AddComponent<GraphicRaycaster>();

        var overlay = new GameObject("OverlayRoot", typeof(RectTransform));
        overlay.transform.SetParent(cvGO.transform, false);
        var bg = overlay.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.7f);
        var rt = overlay.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;

        MakeTMP(overlay.transform, "P1 Controls:\n← Rotate Left\n→ Rotate Right\n↑ Thrust",
            28, new Vector2(-480, 0), new Vector2(400, 300));
        MakeTMP(overlay.transform, "P2 Controls:\nA Rotate Left\nD Rotate Right\nW Thrust",
            28, new Vector2( 480, 0), new Vector2(400, 300));
        MakeTMP(overlay.transform, "Press any key to start...", 20, new Vector2(0, -300), new Vector2(600, 50));

        var ctrl = cvGO.AddComponent<ControlsOverlay>();
        var so   = new SerializedObject(ctrl);
        so.FindProperty("overlayRoot").objectReferenceValue = overlay;
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

    static void SetSkyBg()
    {
        RenderSettings.skybox = null;
        Camera.main?.gameObject.GetComponent<Camera>()?.SetTargetBuffers(default(RenderBuffer), default(RenderBuffer));
    }

    static void AddCamera()
    {
        var go  = new GameObject("Main Camera");
        var cam = go.AddComponent<Camera>();
        cam.backgroundColor = new Color(0.52f, 0.80f, 0.98f, 1f);
        cam.clearFlags      = CameraClearFlags.SolidColor;
        go.AddComponent<AudioListener>();
        go.tag = "MainCamera";
    }
}

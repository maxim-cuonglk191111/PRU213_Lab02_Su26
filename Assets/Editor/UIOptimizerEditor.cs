using UnityEngine;
using UnityEditor;
using TMPro;

public class UIOptimizerEditor
{
    [MenuItem("Tools/Optimize UI Text")]
    public static void OptimizeAllText()
    {
        var texts = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
        int c = 0;
        foreach (var t in texts)
        {
            t.fontStyle |= FontStyles.Bold;
            t.outlineWidth = 0.2f;
            t.outlineColor = new Color32(0, 0, 0, 255);
            EditorUtility.SetDirty(t);
            c++;
        }
        Debug.Log($"Optimized {c} TextMeshProUGUI components with Bold and Outlines.");
    }
}

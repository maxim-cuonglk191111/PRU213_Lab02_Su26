using UnityEngine;
using UnityEditor;
using System.Linq;

public class GlobalMissingScriptFinder
{
    [MenuItem("Tools/Find All Missing Scripts")]
    public static void FindMissing()
    {
        string[] allPaths = AssetDatabase.GetAllAssetPaths();
        int totalMissing = 0;
        foreach (string path in allPaths)
        {
            if (path.EndsWith(".prefab"))
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null)
                {
                    Component[] components = prefab.GetComponentsInChildren<Component>(true);
                    for (int i = 0; i < components.Length; i++)
                    {
                        if (components[i] == null)
                        {
                            Debug.Log($"Missing script on prefab: {path}");
                            totalMissing++;
                        }
                    }
                }
            }
            else if (path.EndsWith(".unity"))
            {
                // we can't easily open scenes in batchmode cleanly like this, but let's check
                var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(path, UnityEditor.SceneManagement.OpenSceneMode.Single);
                GameObject[] go = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                foreach (GameObject g in go)
                {
                    Component[] components = g.GetComponents<Component>();
                    for (int i = 0; i < components.Length; i++)
                    {
                        if (components[i] == null)
                        {
                            Debug.Log($"Missing script in scene {path} on GameObject: {g.name}");
                            totalMissing++;
                        }
                    }
                }
            }
        }
        Debug.Log($"Total missing scripts found: {totalMissing}");
    }
}

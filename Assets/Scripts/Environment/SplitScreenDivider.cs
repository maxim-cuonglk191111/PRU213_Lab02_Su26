using UnityEngine;

/// <summary>
/// Keeps the split-screen divider line centred at Screen.width / 2.
/// Attach to the SplitLineDivider UI Image in the PvP scene.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class SplitScreenDivider : MonoBehaviour
{
    private RectTransform _rt;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
    }

    private void Update()
    {
        // Keep the line exactly at the horizontal centre regardless of resolution
        _rt.anchoredPosition = new Vector2(0f, _rt.anchoredPosition.y);
    }
}

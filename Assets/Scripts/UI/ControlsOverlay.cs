using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// PvP-only 3-second controls reminder overlay.
/// Time.timeScale = 0 during display; any key or auto-dismiss resumes.
/// </summary>
public class ControlsOverlay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject overlayRoot;

    [Header("Timing")]
    [SerializeField] private float displayDuration = 3f;

    private void Start()
    {
        if (overlayRoot != null) overlayRoot.SetActive(true);
        Time.timeScale = 0f;
        StartCoroutine(AutoDismiss());
    }

    private void Update()
    {
        // Any key press dismisses early
        if (Input.anyKeyDown)
            Dismiss();
    }

    private IEnumerator AutoDismiss()
    {
        // WaitForSecondsRealtime works even when timeScale = 0
        yield return new WaitForSecondsRealtime(displayDuration);
        Dismiss();
    }

    private void Dismiss()
    {
        if (!gameObject.activeInHierarchy) return;
        Time.timeScale = 1f;
        if (overlayRoot != null) overlayRoot.SetActive(false);
        enabled = false;  // stop Update polling
    }
}

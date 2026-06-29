using System.Collections;
using UnityEngine;

/// <summary>
/// PvP-only 3-second controls reminder overlay.
/// Time.timeScale = 0 during display; any key or auto-dismiss resumes.
/// A 0.3 s grace period blocks input so a residual click from ModeSelect
/// doesn't instantly dismiss the overlay.
/// </summary>
public class ControlsOverlay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject overlayRoot;

    [Header("Timing")]
    [SerializeField] private float displayDuration  = 3f;
    [SerializeField] private float inputGracePeriod = 0.3f;  // ignore input for this long after show

    private bool _inputReady;
    private bool _dismissed;

    private void Start()
    {
        if (overlayRoot != null) overlayRoot.SetActive(true);
        Time.timeScale = 0f;
        _inputReady = false;
        StartCoroutine(RunOverlay());
    }

    private void Update()
    {
        if (_inputReady && !_dismissed && Input.anyKeyDown)
            Dismiss();
    }

    private IEnumerator RunOverlay()
    {
        // Grace period: ignore any input still held from the previous scene
        yield return new WaitForSecondsRealtime(inputGracePeriod);
        _inputReady = true;

        // Auto-dismiss after remaining display time
        yield return new WaitForSecondsRealtime(displayDuration - inputGracePeriod);
        Dismiss();
    }

    private void Dismiss()
    {
        if (_dismissed) return;
        _dismissed = true;
        Time.timeScale = 1f;
        if (overlayRoot != null) overlayRoot.SetActive(false);
        enabled = false;
    }
}

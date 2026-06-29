using System.Collections;
using UnityEngine;

public class ControlsOverlay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject overlayRoot;

    [Header("Timing")]
    [SerializeField] private float displayDuration  = 3f;
    [SerializeField] private float inputGracePeriod = 0.3f;

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
        yield return new WaitForSecondsRealtime(inputGracePeriod);
        _inputReady = true;
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

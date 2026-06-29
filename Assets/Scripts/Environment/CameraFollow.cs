using UnityEngine;

/// <summary>
/// Smoothly follows a target (the player) on X and Y axes.
/// Assign in Inspector or via setup script.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("The transform to follow (player root).")]
    [SerializeField] public Transform target;

    [Header("Follow Settings")]
    [Tooltip("How fast the camera catches up. Higher = snappier.")]
    [SerializeField] private float smoothSpeed = 5f;

    [Tooltip("Offset from target position.")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, -10f);

    [Header("Clamp (optional)")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private Vector2 minBounds = new Vector2(-50f, -50f);
    [SerializeField] private Vector2 maxBounds = new Vector2( 50f,  50f);

    private void Start()
    {
        // Snap to target immediately so the camera isn't panning from world origin on frame 1
        if (target != null)
        {
            Vector3 snap = target.position + offset;
            snap.z = transform.position.z;
            transform.position = snap;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = target.position + offset;

        if (useBounds)
        {
            desired.x = Mathf.Clamp(desired.x, minBounds.x, maxBounds.x);
            desired.y = Mathf.Clamp(desired.y, minBounds.y, maxBounds.y);
        }

        // Keep Z fixed
        desired.z = transform.position.z;

        transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);
    }
}

using UnityEngine;

/// <summary>
/// Detects whether the boarder is touching the ground via a short downward raycast.
/// TrickManager reads IsGrounded to track airtime.
/// </summary>
public class GroundChecker : MonoBehaviour
{
    [Header("Raycast Settings")]
    [Tooltip("Distance of the downward raycast.")]
    [SerializeField] private float rayLength = 0.3f;

    [Tooltip("Layer(s) considered as ground (set to 'Default' or a dedicated Ground layer).")]
    [SerializeField] private LayerMask groundLayer;

    /// <summary>True when the boarder's lower body is touching the ground.</summary>
    public bool IsGrounded { get; private set; }

    private void FixedUpdate()
    {
        // Cast a short ray downward from this transform's position
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            -transform.up,
            rayLength,
            groundLayer
        );
        IsGrounded = hit.collider != null;
    }

    // ── Editor helpers ─────────────────────────────────────────
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (-transform.up * rayLength));
    }
}

using UnityEngine;

/// <summary>
/// Detects whether the boarder is touching the ground via a short downward raycast.
/// TrickManager reads IsGrounded to track airtime.
/// Set groundLayer to "Everything" in Part3 (or manually in Inspector).
/// Trigger colliders (pickups, power-ups) are excluded.
/// </summary>
public class GroundChecker : MonoBehaviour
{
    [Header("Raycast Settings")]
    [Tooltip("Distance of the downward raycast from the board bottom.")]
    [SerializeField] private float rayLength = 0.3f;

    [Tooltip("Layer(s) considered as ground. Set to 'Everything' in Inspector.")]
    [SerializeField] private LayerMask groundLayer;

    /// <summary>True when the boarder's lower body is touching solid (non-trigger) ground.</summary>
    public bool IsGrounded { get; private set; }

    private void FixedUpdate()
    {
        // RaycastAll so we can filter out trigger colliders
        var hits = Physics2D.RaycastAll(transform.position, -transform.up, rayLength, groundLayer);
        IsGrounded = false;
        foreach (var hit in hits)
        {
            if (hit.collider != null && !hit.collider.isTrigger)
            {
                IsGrounded = true;
                break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (-transform.up * rayLength));
    }
}

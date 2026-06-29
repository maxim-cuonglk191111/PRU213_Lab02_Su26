using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] private float rayLength = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    public bool IsGrounded { get; private set; }

    private void FixedUpdate()
    {
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

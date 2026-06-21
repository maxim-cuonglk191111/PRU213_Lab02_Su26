using UnityEngine;

/// <summary>
/// Handles snowboarder movement and input for one player.
/// Assign the correct InputConfig asset (P1 or P2) in the Inspector.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    // ── Inspector ──────────────────────────────────────────────
    [Header("Input")]
    [Tooltip("ScriptableObject defining key bindings for this player.")]
    [SerializeField] private InputConfig inputConfig;

    [Header("Physics")]
    [Tooltip("Torque applied per frame when rotating.")]
    [SerializeField] private float rotationSpeed = 200f;

    [Tooltip("Force applied per frame when thrusting.")]
    [SerializeField] private float thrustForce = 5f;

    // ── Private ────────────────────────────────────────────────
    private Rigidbody2D _rb;
    private bool _isDisabled;  // set true when game is over / paused

    // ── Unity Lifecycle ────────────────────────────────────────
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        if (inputConfig == null)
            Debug.LogError($"[PlayerController] InputConfig not assigned on {gameObject.name}!");
    }

    private void FixedUpdate()
    {
        if (_isDisabled || inputConfig == null) return;
        HandleRotation();
        HandleThrust();
    }

    // ── Movement ───────────────────────────────────────────────
    private void HandleRotation()
    {
        if (Input.GetKey(inputConfig.rotateLeft))
            _rb.AddTorque(rotationSpeed * Time.fixedDeltaTime * 60f);

        if (Input.GetKey(inputConfig.rotateRight))
            _rb.AddTorque(-rotationSpeed * Time.fixedDeltaTime * 60f);
    }

    private void HandleThrust()
    {
        if (Input.GetKey(inputConfig.thrust))
            _rb.AddForce(transform.up * thrustForce, ForceMode2D.Force);
    }

    // ── Public API ─────────────────────────────────────────────
    /// <summary>Freeze / unfreeze this player's input (used by pause / game-over).</summary>
    public void SetInputEnabled(bool enabled)
    {
        _isDisabled = !enabled;
    }
}

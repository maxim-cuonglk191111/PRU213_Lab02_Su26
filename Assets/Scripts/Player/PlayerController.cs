using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputConfig inputConfig;

    [Header("Physics")]
    [SerializeField] private float rotationSpeed   = 250f;
    [SerializeField] private float thrustForce     = 12f;
    [SerializeField] private float maxAngularSpeed = 150f;
    [SerializeField] private float maxSpeed        = 25f;

    private Rigidbody2D _rb;
    private bool _isDisabled;

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
        ClampSpeed();
    }

    private void HandleRotation()
    {
        float dir = 0f;
        if (Input.GetKey(inputConfig.rotateLeft))  dir =  1f;
        if (Input.GetKey(inputConfig.rotateRight)) dir = -1f;

        _rb.angularVelocity = Mathf.MoveTowards(
            _rb.angularVelocity,
            dir * maxAngularSpeed,
            rotationSpeed * Time.fixedDeltaTime * 60f
        );
    }

    private void HandleThrust()
    {
        if (!Input.GetKey(inputConfig.thrust)) return;
        _rb.AddForce(transform.right * thrustForce, ForceMode2D.Force);
    }

    private void ClampSpeed()
    {
        if (_rb.linearVelocity.sqrMagnitude > maxSpeed * maxSpeed)
            _rb.linearVelocity = _rb.linearVelocity.normalized * maxSpeed;
    }

    public void SetInputEnabled(bool enabled)
    {
        _isDisabled = !enabled;
    }
}

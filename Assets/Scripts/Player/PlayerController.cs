using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputConfig inputConfig;

    [Header("Physics")]
    [SerializeField] private float rotationSpeed  = 100f;
    [SerializeField] private float thrustForce    = 40f;
    [SerializeField] private float maxAngularSpeed = 300f;

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
    }

    private void HandleRotation()
    {
        if (Input.GetKey(inputConfig.rotateLeft))
            _rb.AddTorque(rotationSpeed * Time.fixedDeltaTime * 60f);
        if (Input.GetKey(inputConfig.rotateRight))
            _rb.AddTorque(-rotationSpeed * Time.fixedDeltaTime * 60f);
        _rb.angularVelocity = Mathf.Clamp(_rb.angularVelocity, -maxAngularSpeed, maxAngularSpeed);
    }

    private void HandleThrust()
    {
        if (Input.GetKey(inputConfig.thrust))
            _rb.AddForce(transform.up * thrustForce, ForceMode2D.Force);
    }

    public void SetInputEnabled(bool enabled)
    {
        _isDisabled = !enabled;
    }
}

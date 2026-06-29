using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    public int playerIndex = 0; // 0 = P1 (arrows + Space), 1 = P2 (WASD + F)

    [Header("Speed Settings")]
    public float baseSpeed  = 26f;
    public float boostSpeed = 38f;
    public float maxSpeed   = 50f;
    public float moveForce  = 30f;

    [Header("Physics Settings")]
    public float torqueAmount = 25f;
    public float brakeForce   = 60f;
    public float brakeMinSpeed = 0.35f;

    [Header("Jump Settings")]
    public float jumpForce = 10f;

    [Header("References (Auto-populated)")]
    public Rigidbody2D     rb;
    public SpriteRenderer  playerSprite;

    System.Collections.Generic.List<SurfaceEffector2D> surfaceEffectors = new();
    float rotationInput  = 0f;
    bool  isBraking      = false;
    bool  isBoosting     = false;
    bool  canMove        = true;
    bool  isGrounded     = false;
    bool  hasFinished    = false;
    float slopeSpeedBoost = 0f;
    float slopeAngle      = 0f;
    float gravityMult     = 1f;
    float friction        = 0f;

    void Start()
    {
        if (rb == null)           rb = GetComponent<Rigidbody2D>();
        if (playerSprite == null) playerSprite = GetComponentInChildren<SpriteRenderer>();

        surfaceEffectors.Clear();
        foreach (var e in FindObjectsByType<SurfaceEffector2D>(FindObjectsInactive.Exclude))
        {
            if (e.CompareTag("Ground") ||
                e.gameObject.name.Contains("Level") ||
                e.gameObject.name.Contains("Ground") ||
                e.gameObject.name.Contains("Slope"))
                surfaceEffectors.Add(e);
        }
    }

    void Update()
    {
        if (hasFinished || !canMove) return;
        ReadInput();
        HandleJump();
    }

    void ReadInput()
    {
        rotationInput = 0f;
        isBraking     = false;
        isBoosting    = false;

        if (Keyboard.current == null) return;

        if (playerIndex == 0)
        {
            if (Keyboard.current.leftArrowKey.isPressed)  rotationInput =  1f;
            if (Keyboard.current.rightArrowKey.isPressed) rotationInput = -1f;
            if (Keyboard.current.upArrowKey.isPressed)    isBoosting = true;
            if (Keyboard.current.downArrowKey.isPressed)  isBraking  = true;
        }
        else
        {
            if (Keyboard.current.aKey.isPressed) rotationInput =  1f;
            if (Keyboard.current.dKey.isPressed) rotationInput = -1f;
            if (Keyboard.current.wKey.isPressed) isBoosting = true;
            if (Keyboard.current.sKey.isPressed) isBraking  = true;
        }
    }

    void HandleJump()
    {
        if (Keyboard.current == null || !isGrounded) return;
        Key jumpKey = (playerIndex == 0) ? Key.Space : Key.F;
        if (Keyboard.current[jumpKey].wasPressedThisFrame)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        if (hasFinished || !canMove) return;

        UpdateSlopePhysics();

        if (rotationInput != 0f)
        {
            float t = isGrounded ? torqueAmount : torqueAmount * 2.5f;
            rb.AddTorque(rotationInput * t);
        }

        float targetX = isBoosting ? boostSpeed : baseSpeed;
        if (rb.linearVelocity.x < targetX)
            rb.AddForce(Vector2.right * moveForce * 4f);

        if (isBraking && rb.linearVelocity.x > baseSpeed * brakeMinSpeed)
            rb.AddForce(-rb.linearVelocity.normalized * brakeForce);

        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
    }

    void UpdateSlopePhysics()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 3.5f);
        if (hit.collider != null &&
            (hit.collider.CompareTag("Ground") ||
             hit.collider.gameObject.name.Contains("Level") ||
             hit.collider.gameObject.name.Contains("Slope")))
        {
            Vector2 normal = hit.normal;
            slopeAngle = Vector2.Angle(normal, Vector2.up);
            gravityMult = 1f + (slopeAngle / 45f) * 0.5f;

            if (normal.x > 0.02f)       { slopeSpeedBoost =  slopeAngle * 0.4f; friction = 0.01f; }
            else if (normal.x < -0.02f) { slopeSpeedBoost = -slopeAngle * 0.6f; friction = 0.15f; }
            else                        { slopeSpeedBoost = 0f;                  friction = 0.05f; }

            Vector2 tangent = new Vector2(normal.y, -normal.x).normalized;
            float g = Physics2D.gravity.magnitude;
            rb.AddForce(tangent * Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * g * gravityMult * rb.mass * 0.4f);

            if (rb.linearVelocity.magnitude > 0.1f)
                rb.AddForce(-rb.linearVelocity.normalized * friction * rb.mass * 6f);
        }
        else
        {
            slopeAngle = 0f; slopeSpeedBoost = 0f; gravityMult = 1f; friction = 0.02f;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            AudioManager.Instance?.ResumeBoardingSound();
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            AudioManager.Instance?.PauseBoardingSound();
        }
    }

    public bool IsGrounded()             => isGrounded;
    public void DisableControls()        => canMove = false;
    public void EnableControls()         => canMove = true;
    public void SetInputEnabled(bool en) => canMove = en;

    public void SetFinished()
    {
        hasFinished = true;
        DisableControls();
    }

    public void ResetAll()
    {
        canMove     = true;
        isGrounded  = false;
        hasFinished = false;
    }

    public void ApplySpeedBoost(float duration)  => StartCoroutine(SpeedBoostCoroutine(duration));
    public void ApplyInvincibility(float duration) => StartCoroutine(InvincibilityCoroutine(duration));

    IEnumerator SpeedBoostCoroutine(float duration)
    {
        float origBase = baseSpeed, origBoost = boostSpeed;
        baseSpeed *= 1.5f; boostSpeed *= 1.5f;
        if (playerSprite != null) playerSprite.color = Color.cyan;
        yield return new WaitForSeconds(duration);
        baseSpeed = origBase; boostSpeed = origBoost;
        if (playerSprite != null) playerSprite.color = Color.white;
    }

    IEnumerator InvincibilityCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (playerSprite != null) playerSprite.enabled = true;
    }
}

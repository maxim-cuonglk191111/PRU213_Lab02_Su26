using UnityEngine;
using UnityEngine.InputSystem;

public class TrickManager : MonoBehaviour
{
    [Header("Key Bindings")]
    public Key jumpKey = Key.Space;
    public Key indyGrabKey = Key.Q;
    public Key methodGrabKey = Key.E;

    [Header("Trick Settings")]
    public float trickCooldown = 0.5f;
    public float inputBufferTime = 0.2f;
    public float grabInterval = 0.4f;

    PlayerController playerController;
    float startRotationZ;
    float totalRotation;
    int currentCombo = 1;
    float lastTrickTime = -999f;
    float airTime = 0f;
    bool wasInAir = false;
    int pendingScore = 0;

    // Grab & Spin Tracking
    Vector3 originalSpriteScale = Vector3.one;
    bool hasOriginalScale = false;
    float grabTimer = 0f;
    int flipCount = 0;



    void Start()
    {
        playerController = GetComponent<PlayerController>();
        if (playerController != null && playerController.playerSprite != null)
        {
            originalSpriteScale = playerController.playerSprite.transform.localScale;
            hasOriginalScale = true;
        }
    }

    void Update()
    {
        if (playerController == null) return;

        if (!hasOriginalScale && playerController.playerSprite != null)
        {
            originalSpriteScale = playerController.playerSprite.transform.localScale;
            hasOriginalScale = true;
        }

        if (!playerController.IsGrounded())
        {
            // --- MID-AIR LOGIC ---
            float deltaAngle = Mathf.DeltaAngle(startRotationZ, transform.rotation.eulerAngles.z);
            totalRotation += deltaAngle;
            startRotationZ = transform.rotation.eulerAngles.z;

            airTime += Time.deltaTime;

            // 1. Board Grabs Detection
            bool isGrabbingThisFrame = false;
            string grabName = "";
            Vector3 targetScale = originalSpriteScale;

            if (Keyboard.current != null)
            {
                if (Keyboard.current[indyGrabKey].isPressed)
                {
                    isGrabbingThisFrame = true;
                    grabName = "INDY GRAB";
                    targetScale = Vector3.Scale(originalSpriteScale, new Vector3(0.8f, 0.8f, 1f));
                }
                else if (Keyboard.current[methodGrabKey].isPressed)
                {
                    isGrabbingThisFrame = true;
                    grabName = "METHOD GRAB";
                    targetScale = Vector3.Scale(originalSpriteScale, new Vector3(1.2f, 0.7f, 1f));
                }
            }

            if (isGrabbingThisFrame)
            {
                grabTimer += Time.deltaTime;
                if (grabTimer >= grabInterval)
                {
                    grabTimer = 0f;
                    int multiplier = Mathf.Min(currentCombo, 10);
                    int grabPoints = 150 * multiplier;
                    pendingScore += grabPoints;

                    if (playerController != null)
                        playerController.ShowToast($"{grabName}! +{grabPoints} (Pending)");

                    if (AudioManager.Instance != null)
                        AudioManager.Instance.PlayTrickSuccessSound();
                }
            }
            else
            {
                grabTimer = 0f;
            }

            if (playerController.playerSprite != null)
            {
                playerController.playerSprite.transform.localScale = Vector3.Lerp(playerController.playerSprite.transform.localScale, targetScale, Time.deltaTime * 10f);
            }

            // 2. Directional Flips Detection
            if (Mathf.Abs(totalRotation) >= 360f)
            {
                flipCount++;
                bool isBackflip = totalRotation > 0f;
                totalRotation = totalRotation > 0f ? totalRotation - 360f : totalRotation + 360f;

                if (Time.time - lastTrickTime > trickCooldown)
                {
                    lastTrickTime = Time.time;
                    int multiplier = Mathf.Min(currentCombo, 10);

                    int flipPoints = 300;
                    if (flipCount == 2) flipPoints = 400;
                    else if (flipCount == 3) flipPoints = 500;
                    else if (flipCount > 3) flipPoints = 600;

                    pendingScore += flipPoints * multiplier;

                    string flipText = "";
                    if (flipCount == 1) flipText = isBackflip ? "BACKFLIP!" : "FRONTFLIP!";
                    else if (flipCount == 2) flipText = isBackflip ? "DOUBLE BACKFLIP!" : "DOUBLE FRONTFLIP!";
                    else if (flipCount == 3) flipText = isBackflip ? "TRIPLE BACKFLIP!" : "TRIPLE FRONTFLIP!";
                    else flipText = $"{flipCount}x {(isBackflip ? "BACKFLIP!" : "FRONTFLIP!")}";

                    if (playerController != null)
                        playerController.ShowToast($"{flipText} x{multiplier} (Pending)");

                    if (AudioManager.Instance != null)
                        AudioManager.Instance.PlayTrickSuccessSound();

                    currentCombo++;
                }
            }
        }
        else
        {
            // --- GROUNDED LOGIC ---
            if (hasOriginalScale && playerController.playerSprite != null)
            {
                playerController.playerSprite.transform.localScale = Vector3.Lerp(playerController.playerSprite.transform.localScale, originalSpriteScale, Time.deltaTime * 10f);
            }

            if (wasInAir)
            {
                if (pendingScore > 0)
                {
                    var sm = GetComponent<ScoreManager>();
                    if (sm != null)
                        sm.AddTrickScore(pendingScore, 1);

                    if (playerController != null)
                        playerController.ShowToast($"+{pendingScore} LANDED!");

                    if (AudioManager.Instance != null)
                        AudioManager.Instance.PlayTrickSuccessSound();

                    pendingScore = 0;
                }

                float groundAngle = GetGroundAngle();
                float playerAngle = transform.rotation.eulerAngles.z;
                float angleDiff = Mathf.Abs(Mathf.DeltaAngle(playerAngle, groundAngle));

                if (angleDiff > playerController.crashRotationThreshold)
                {
                    var crashHandler = GetComponent<CrashHandler>();
                    if (crashHandler != null)
                    {
                        crashHandler.ForceCrash();
                        if (playerController != null) playerController.ShowToast("BAD LANDING!");
                    }
                    pendingScore = 0;
                }
                else if (angleDiff <= 15f)
                {
                    int perfectLandingBonus = 200;
                    var sm = GetComponent<ScoreManager>();
                    if (sm != null)
                        sm.AddTrickScore(perfectLandingBonus, 1);

                    if (playerController != null)
                        playerController.ShowToast("PERFECT LANDING! +200");

                    if (AudioManager.Instance != null)
                        AudioManager.Instance.PlayTrickSuccessSound();
                }

                if (airTime > 1.5f)
                {
                    int bigAirBonus = Mathf.RoundToInt((airTime - 1.0f) * 250f);
                    var sm = GetComponent<ScoreManager>();
                    if (sm != null)
                        sm.AddTrickScore(bigAirBonus, 1);

                    if (playerController != null)
                        playerController.ShowToast($"BIG AIR! +{bigAirBonus}");
                }

                if (airTime > 0.5f)
                {
                    currentCombo = 1;
                }
            }

            startRotationZ = transform.rotation.eulerAngles.z;
            totalRotation = 0f;
            airTime = 0f;
            flipCount = 0;
            grabTimer = 0f;
        }

        wasInAir = !playerController.IsGrounded();
    }

    float GetGroundAngle()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 5f);
        if (hit.collider != null && (hit.collider.CompareTag("Ground") || hit.collider.gameObject.name.Contains("Level") || hit.collider.gameObject.name.Contains("Slope")))
        {
            Vector2 normal = hit.normal;
            return Mathf.Atan2(-normal.x, normal.y) * Mathf.Rad2Deg;
        }
        return 0f;
    }

    public void ResetCombo()
    {
        currentCombo = 1;
        totalRotation = 0f;
        airTime = 0f;
        pendingScore = 0;
        flipCount = 0;
        grabTimer = 0f;
        if (hasOriginalScale && playerController != null && playerController.playerSprite != null)
        {
            playerController.playerSprite.transform.localScale = originalSpriteScale;
        }
    }

    public void ResetAll()
    {
        ResetCombo();
        lastTrickTime = -999f;
        wasInAir = false;
    }

    // Legacy alias for CrashHandler compatibility
    public void OnCrash() => ResetCombo();
}

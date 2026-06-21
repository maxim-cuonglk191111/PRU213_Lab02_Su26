using UnityEngine;

/// <summary>
/// ScriptableObject holding key bindings for a single player.
/// Create two instances in Assets/Scripts/Player/:
///   - InputConfig_P1.asset  (LeftArrow / RightArrow / UpArrow)
///   - InputConfig_P2.asset  (A / D / W)
/// </summary>
[CreateAssetMenu(fileName = "InputConfig", menuName = "SnowBoarder/InputConfig")]
public class InputConfig : ScriptableObject
{
    [Tooltip("Key to rotate the boarder counter-clockwise (left).")]
    public KeyCode rotateLeft  = KeyCode.LeftArrow;

    [Tooltip("Key to rotate the boarder clockwise (right).")]
    public KeyCode rotateRight = KeyCode.RightArrow;

    [Tooltip("Key to apply forward thrust.")]
    public KeyCode thrust      = KeyCode.UpArrow;
}

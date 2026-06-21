using UnityEngine;

/// <summary>
/// Scrolls cloud sprites left at a constant speed and loops them.
/// Attach to each cloud sprite's parent; set loopWidth to the total horizontal span.
/// </summary>
public class CloudScroller : MonoBehaviour
{
    [Tooltip("Pixels per second (world units). PRD specifies 20 px/s.")]
    [SerializeField] private float scrollSpeed = 0.2f;  // 20 px at 100 PPU = 0.2 world units/s

    [Tooltip("Horizontal distance to travel before looping back to start.")]
    [SerializeField] private float loopWidth = 25f;

    private Vector3 _startPosition;

    private void Awake()
    {
        _startPosition = transform.position;
    }

    private void Update()
    {
        transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);

        // Loop when fully off-screen to the left
        if (transform.position.x < _startPosition.x - loopWidth)
            transform.position = _startPosition;
    }
}

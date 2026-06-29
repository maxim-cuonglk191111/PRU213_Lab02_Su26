using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Follow Settings")]
    public Vector3 offset = new Vector3(0f, 0f, -10f);
    public float smoothSpeed = 5f;

    void Start()
    {
        if (player == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag(gameObject.name.Contains("2") ? "Player2" : "Player");
            if (go != null) player = go.transform;
        }

        Camera cam = GetComponent<Camera>();
        if (cam != null && cam.orthographic)
        {
            cam.orthographicSize = 10f;
        }
    }

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 desiredPosition = player.position + offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        }
    }

    public void SnapToPlayer()
    {
        if (player != null)
        {
            transform.position = player.position + offset;
        }
    }
}

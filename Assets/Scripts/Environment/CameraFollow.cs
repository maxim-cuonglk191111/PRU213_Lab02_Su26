using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] public Transform target;
    [SerializeField] public string    playerTag = "Player";

    [Header("Follow Settings")]
    [SerializeField] public  Vector3 offset      = new Vector3(0f, 2f, -10f);
    [SerializeField] private float   smoothSpeed = 8f;

    void Start()
    {
        if (target == null) TryFindTarget();
        if (target != null)
        {
            Vector3 snap = target.position + offset;
            snap.z = transform.position.z;
            transform.position = snap;
        }
    }

    void LateUpdate()
    {
        if (target == null) { TryFindTarget(); return; }
        Vector3 desired = target.position + offset;
        desired.z = transform.position.z;
        transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);
    }

    void TryFindTarget()
    {
        if (string.IsNullOrEmpty(playerTag)) return;
        var go = GameObject.FindGameObjectWithTag(playerTag);
        if (go != null) target = go.transform;
    }
}

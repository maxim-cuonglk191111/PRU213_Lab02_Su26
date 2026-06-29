using UnityEngine; using TMPro; public class TestTMP : MonoBehaviour { void Start() { var t = GetComponent<TextMeshProUGUI>(); t.outlineWidth = 0.2f; t.outlineColor = Color.black; } }

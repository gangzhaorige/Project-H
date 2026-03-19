using UnityEngine;

public class PulseEffect : MonoBehaviour
{
    [Header("Pulse Settings")]
    [SerializeField] private float pulseSpeed = 4f;
    [SerializeField] private float minScale = 0.85f;
    [SerializeField] private float maxScale = 1.15f;
    [SerializeField] private bool useUnscaledTime = false;

    private Vector3 initialScale;
    private bool initialized = false;

    private void Awake()
    {
        InitializeScale();
    }

    private void InitializeScale()
    {
        if (initialized) return;
        initialScale = transform.localScale;
        
        // Safety check: if object starts with 0 scale, assume it should be 1
        if (initialScale.sqrMagnitude < 0.0001f)
        {
            initialScale = Vector3.one;
        }
        initialized = true;
    }

    private void OnEnable()
    {
        InitializeScale();
        transform.localScale = initialScale;
    }

    private void Update()
    {
        float time = useUnscaledTime ? Time.unscaledTime : Time.time;
        
        // Calculate pulse factor using sine wave (ranges from -1 to 1)
        // We map it to 0 to 1 for easier lerping
        float sine = Mathf.Sin(time * pulseSpeed);
        float t = (sine + 1f) * 0.5f;

        // Interpolate between min and max scale
        float currentScale = Mathf.Lerp(minScale, maxScale, t);
        transform.localScale = initialScale * currentScale;
    }

    private void OnDisable()
    {
        // Return to initial scale when disabled
        transform.localScale = initialScale;
    }
}

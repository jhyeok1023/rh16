using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class FlowerRotator : MonoBehaviour
{
    private const float FullTurnDegrees = 360f;

    [SerializeField, Min(0f)] private float degreesPerSecond = 180f;
    [SerializeField] private bool clockwise = true;
    [SerializeField] private bool useUnscaledTime = true;

    private RectTransform _rectTransform;
    private float _currentAngle;

    private void Awake()
    {
        _rectTransform = (RectTransform)transform;
        _currentAngle = _rectTransform.localEulerAngles.z;
    }

    private void Update()
    {
        float deltaTime = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        float direction = clockwise ? -1f : 1f;

        _currentAngle = Mathf.Repeat(_currentAngle + direction * degreesPerSecond * deltaTime, FullTurnDegrees);
        _rectTransform.localRotation = Quaternion.Euler(0f, 0f, _currentAngle);
    }
}

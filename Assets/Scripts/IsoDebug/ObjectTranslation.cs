using UnityEngine;

[ExecuteAlways]
public class ObjectTranslation : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private Vector3 rotationSpeed = Vector3.one;
    [SerializeField] private float uniformRotationSpeed = 1.0f;

    [Header("Translation")]
    [SerializeField] private float translationSpeed = 1.0f;
    [SerializeField] private Vector3 translateFrom = Vector3.left;
    [SerializeField] private Vector3 translateTo = Vector3.right;

    [Header("Scale")]
    [SerializeField] private float scaleSpeed = 1.0f;
    [SerializeField] private Vector3 scaleFrom = Vector3.one;
    [SerializeField] private Vector3 scaleTo = Vector3.one;

    private void Update()
    {
        UpdateRotation();
        UpdateTranslation();
        UpdateScale();
    }
    private void UpdateRotation()
    {
        transform.localRotation = Quaternion.Euler(
            Time.realtimeSinceStartup * rotationSpeed.x * uniformRotationSpeed,
            Time.realtimeSinceStartup * rotationSpeed.y * uniformRotationSpeed,
            Time.realtimeSinceStartup * rotationSpeed.z * uniformRotationSpeed
        );
    }

    private void UpdateTranslation()
    {
        transform.localPosition = new Vector3(
            PingPong(translationSpeed, translateFrom.x, translateTo.x),
            PingPong(translationSpeed, translateFrom.y, translateTo.y),
            PingPong(translationSpeed, translateFrom.z, translateTo.z)
        );
    }

    private void UpdateScale()
    {
        transform.localScale = new Vector3(
            PingPong(scaleSpeed, scaleFrom.x, scaleTo.x),
            PingPong(scaleSpeed, scaleFrom.y, scaleTo.y),
            PingPong(scaleSpeed, scaleFrom.z, scaleTo.z)
        );
    }

    private float PingPong(float speed, float from, float to)
    {
        if (from == to)
            return from;
        if (from > to)
            return Mathf.PingPong(Time.time * speed, from - to) - from;

        return Mathf.PingPong(Time.time * speed, to - from) + from;
    }
}
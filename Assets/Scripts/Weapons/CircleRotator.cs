using UnityEngine;

public class CircleRotator : MonoBehaviour
{
    private float rotationSpeed;
    private float currentAngle;
    private float radius;

    public void Initialize(float radius, float speed, float angleOffset = 0f)
    {
        currentAngle = angleOffset;
        rotationSpeed = speed;
        this.radius = radius;
    }

    private void Update()
    {
        currentAngle += rotationSpeed * Time.deltaTime;
        float radianAngle = currentAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle), 0f) * radius;
        transform.position = transform.parent.position + offset;
    }
}
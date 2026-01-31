using UnityEngine;

public class CanvasTiltFx : MonoBehaviour
{
    private Quaternion initRotation;
    public float impactMultiplier = 500f; // 충격량 계수
    public float spring = 100.0f; // 복원력(스프링)
    public float damper = 15.0f; // 감쇠(저항)

    private Vector3 angularVelocity;
    private Quaternion currentRotation;

    void Awake()
    {
        initRotation = transform.rotation;
        currentRotation = initRotation;
    }

    void Update() {
        Quaternion diff = initRotation * Quaternion.Inverse(currentRotation);
        diff.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180) angle -= 360;

        Vector3 torque = axis.normalized * (angle * spring);
        angularVelocity += (torque - angularVelocity * damper) * Time.deltaTime;
        currentRotation *= Quaternion.Euler(angularVelocity * Time.deltaTime);

        transform.rotation = currentRotation;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("BulletL") || collision.gameObject.CompareTag("BulletR"))
        {
            Vector3 torqueDirection = Vector3.Cross(collision.relativeVelocity.normalized, (transform.position - collision.contacts[0].point).normalized);
            angularVelocity += torqueDirection * impactMultiplier * Time.deltaTime;
        }
    }
}

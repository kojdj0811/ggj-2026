using System.Collections.Generic;
using Freya;
using MyBox;
using UnityEngine;

public class Aimer : MonoBehaviour
{

    public static Dictionary<int, Aimer> Aimers = new Dictionary<int, Aimer>();
    public int aimerId;

    public GameObject bulletPrefab;
    public Transform shootAngleTransform;
    public Transform planeTransform;
    public LayerMask planeLayer;
    [Range(0.001f, 0.999f)]
    public float triggerRate = 0.5f;
    public AnimationCurve forceMultiplierCurve = AnimationCurve.Linear(0f, 0.5f, 1f, 1f);
    public AnimationCurve spreadRadiusCurve = AnimationCurve.Linear(0f, 0.0f, 1f, 4f);


    private void Awake() {
        Aimers[aimerId] = this;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 1000f, planeLayer))
            {
                ShootBullet(hit.point, 1f, 0);
            }
        }
    }

    public void ShootBullet(Vector3 arrivalPoint, float triggerValue, int userId)
    {
        if(GameTimer.TimeRemained == false)
        {
            return;
        }

        (float forceMultiplier, float spreadRadius) = SplitTriggerValuse(triggerValue);

        if(spreadRadius > 0f)
        {
            Vector2 rundomOffset = UnityEngine.Random.insideUnitCircle;
            Vector2 spreadOffset = rundomOffset.Sign() * rundomOffset.Pow(2f) * spreadRadius;
            arrivalPoint += planeTransform.right * spreadOffset.x + planeTransform.up * spreadOffset.y;
        }

        transform.LookAt(arrivalPoint);
        float force = GetForceToArrivalPoint(shootAngleTransform.forward, arrivalPoint - planeTransform.forward * 0.5f);

        GameObject bulletGo = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Bullet bullet = bulletGo.GetComponent<Bullet>();

        ColorUtility.TryParseHtmlString(
            GameManager.Instance.ColorDataSO.GetColorCodeByIndex(GameManager.Instance.Players[aimerId].ColorID),
            out Color color
        );
        bullet.material.SetColor("_BaseColor", color);

        Rigidbody bulletRb = bulletGo.GetComponent<Rigidbody>();
        bulletRb.position = transform.position;
        bulletRb.linearVelocity = shootAngleTransform.forward * force * forceMultiplier;
    }


    private (float, float) SplitTriggerValuse(float triggerValue)
    {
        float forceMultiplierT = Mathf.Clamp01(triggerValue / triggerRate);
        float forceMultiplier = forceMultiplierCurve.Evaluate(forceMultiplierT);

        float spreadRate = Mathf.Clamp01((triggerValue - triggerRate) / (1f - triggerRate));
        float spreadRadius = spreadRadiusCurve.Evaluate(spreadRate);

        return (forceMultiplier, spreadRadius);        
    }



    private float GetForceToArrivalPoint(Vector3 dir, Vector3 arrivalPoint)
    {
        // 1. 수직 높이 차이를 계산합니다.
        float yDisplacement = arrivalPoint.y - transform.position.y;

        // 2. 수평 거리를 계산합니다.
        Vector3 startPos = transform.position;
        startPos.y = 0;
        Vector3 arrivalPos = arrivalPoint;
        arrivalPos.y = 0;
        float distance = Vector3.Distance(startPos, arrivalPos);

        // 3. 중력 가속도의 크기를 가져옵니다.
        float g = Physics.gravity.magnitude;

        // 4. 발사각(θ)을 라디안으로 구합니다.
        Vector3 dirOnPlane = Vector3.ProjectOnPlane(dir, Vector3.up);

        // 발사 방향이 거의 수직인 경우(수평 성분이 없는 경우) 계산을 중단합니다.
        if (dirOnPlane.sqrMagnitude < 0.001f)
        {
            return 0f; // 도달 불가능
        }

        // dir.y의 부호를 확인하여 발사각의 부호를 결정합니다.
        float angle = Vector3.Angle(dirOnPlane, dir);
        if (dir.y < 0)
        {
            angle = -angle; // 아래로 발사하면 각도를 음수로 만듭니다.
        }
        float angleInRad = angle * Mathf.Deg2Rad;

        // 5. 포물선 궤적 공식을 이용해 초기 속도(force)를 계산합니다.
        //    v = sqrt( (g * x^2) / (2 * cos^2(θ) * (x*tan(θ) - y)) )
        float cosAngle = Mathf.Cos(angleInRad);
        
        // cos(θ)가 0에 가까운 경우(발사각이 90도에 가까운 경우) 계산 오류를 방지합니다.
        if (Mathf.Abs(cosAngle) < 0.001f)
        {
            return 0f; // 도달 불가능
        }
        
        float tanAngle = Mathf.Tan(angleInRad);

        float denominator = 2 * cosAngle * cosAngle * (distance * tanAngle - yDisplacement);
        if (denominator <= 0f)
        {
            return 0f; // 도달 불가능한 경우
        }

        float forceSqr = (g * distance * distance) / denominator;
        
        // 제곱근 안의 값이 음수가 되는 경우를 방지합니다. (이론적으로는 위에서 걸러지지만 안전장치)
        if (forceSqr < 0f)
        {
            return 0f; // 도달 불가능
        }

        float force = Mathf.Sqrt(forceSqr);
        return force;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1000f, planeLayer))
        {
            Gizmos.DrawLine(transform.position, hit.point);
            Gizmos.DrawSphere(hit.point, 0.1f);

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(hit.point - planeTransform.forward * 0.5f, 0.1f);
        }
    }
}

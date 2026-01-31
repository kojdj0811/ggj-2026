using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class ShootingData
{
    public InputController Owner;
    public Vector2 ReticlePosition;
    public float TriggerValue;
}

public class InputController : MonoBehaviour
{
    [SerializeField]
    private GameObject _reticle;
    private Vector2 _reticleInputValue;
    private float _triggerInputValue;
    private float _prevTriggerInputValue;
    private readonly float[] _triggerValueBuffer = new float[10];
    private int _triggerValueBufferIndex = 0;

    public ShootingData ShootingData;

    void Update()
    {
        // 트리거 값 버퍼에 저장 (순환)
        _triggerValueBuffer[_triggerValueBufferIndex] = _triggerInputValue;
        _triggerValueBufferIndex = (_triggerValueBufferIndex + 1) % _triggerValueBuffer.Length;

        // 트리거가 0이 되는 순간, 최근 10프레임 중 최대값을 힘으로 사용
        if (_prevTriggerInputValue > 0f && _triggerInputValue == 0f)
        {
            float maxForce = 0f;
            for (int i = 0; i < _triggerValueBuffer.Length; i++)
            {
                if (_triggerValueBuffer[i] > maxForce)
                    maxForce = _triggerValueBuffer[i];
            }
            OnTriggerReleased(maxForce);
        }
        _prevTriggerInputValue = _triggerInputValue;


        Vector3 currentReticlePos = _reticle.transform.position;
        Vector3 nextReticlePos = currentReticlePos + new Vector3(_reticleInputValue.x, _reticleInputValue.y, 0) * Time.deltaTime;
        Vector3 nextReticlePosV = Camera.main.WorldToViewportPoint(nextReticlePos);

        nextReticlePos.x = nextReticlePosV.x > 1f ? currentReticlePos.x :
                             nextReticlePosV.x < 0f ? currentReticlePos.x :
                             nextReticlePos.x;
        nextReticlePos.y = nextReticlePosV.y > 1f ? currentReticlePos.y :
                             nextReticlePosV.y < 0f ? currentReticlePos.y :
                             nextReticlePos.y;
    
        _reticle.transform.position = nextReticlePos;
    }

    private void OnTriggerReleased(float releasedForce)
    {
        ShootingData.Owner = this;
        ShootingData.ReticlePosition = _reticle.transform.position;
        ShootingData.TriggerValue = releasedForce;
        
        Ray ray = Camera.main.ViewportPointToRay(Camera.main.WorldToViewportPoint(_reticle.transform.position));
        // 0 is temp id for test. replcae this with actual player id later
        if(Aimer.Aimers != null && Aimer.Aimers.ContainsKey(0) && Physics.Raycast(ray, out RaycastHit hit, 100f, Aimer.Aimers[0].planeLayer))
        {
            Aimer.Aimers[0].ShootBullet(hit.point, releasedForce, 0);
        }
    }

    private void OnStickInput(InputValue value)
    {
        _reticleInputValue = value.Get<Vector2>();
    }

    // private void OnDpadUp(InputValue value)
    // {
    //     _reticleInput.y = value.isPressed ? 1 : 0;
    // }

    // private void OnDpadDown(InputValue value)
    // {
    //     _reticleInput.y = value.isPressed ? -1 : 0;
    // }

    // private void OnDpadLeft(InputValue value)
    // {
    //     _reticleInput.x = value.isPressed ? -1 : 0;
    // }

    // private void OnDpadRight(InputValue value)
    // {
    //     _reticleInput.x = value.isPressed ? 1 : 0;
    // }

    private void OnTriggerInput(InputValue value)
    {
        _triggerInputValue = value.Get<float>();
    }
}
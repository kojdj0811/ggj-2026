using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

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
    private EntrySelector _entrySelector;
    [SerializeField]
    private GameObject _reticle;
    private Vector2 _reticleInputValue;
    private float _triggerInputValue;
    private float _prevTriggerInputValue;
    private readonly float[] _triggerValueBuffer = new float[10];
    private int _triggerValueBufferIndex = 0;

    public ShootingData ShootingData;

    Player player;

    private Renderer _reticleRenderer;

    private void Awake() {
        player = GetComponent<Player>();
        _reticleRenderer = _reticle.GetComponent<Renderer>();
    }

    void LateUpdate()
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

    private async Task OnTriggerReleased(float releasedForce)
    {
        // 컨트롤러 바이브레이션 (진동) 예시
        if (UnityEngine.InputSystem.Gamepad.current != null)
        {
            // releasedForce를 진동 세기로 활용 (0~1)
            float vibration = Mathf.Clamp01(releasedForce);
            UnityEngine.InputSystem.Gamepad.current.SetMotorSpeeds(0f, vibration);
            // 진동을 잠시 후 멈추는 예시 (0.5초)
            await UniTask.Delay(500);
            UnityEngine.InputSystem.Gamepad.current.SetMotorSpeeds(0, 0);
        }


        ShootingData.Owner = this;
        ShootingData.ReticlePosition = _reticle.transform.position;
        ShootingData.TriggerValue = releasedForce;

                // 현재 레이캐스트에 맞고있는 Toggle/버튼 처리
        if (_entrySelector != null)
        {
            _entrySelector.ClickCurrentButton();
            _entrySelector.SelectCurrentToggle();
        }
        
        Ray ray = Camera.main.ViewportPointToRay(Camera.main.WorldToViewportPoint(_reticle.transform.position));
        // 0 is temp id for test. replcae this with actual player id later
        if(Aimer.Aimers != null && Aimer.Aimers.ContainsKey(0) && Physics.Raycast(ray, out RaycastHit hit, 1000f, Aimer.Aimers[0].planeLayer))
        {
            Aimer.Aimers[player.PlayerID].ShootBullet(hit.point, releasedForce, 0);
        }
    }

    private void OnStickInput(InputValue value)
    {
        _reticleInputValue = value.Get<Vector2>();
    }

    private void OnTriggerInput(InputValue value)
    {
        _triggerInputValue = value.Get<float>();

        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        _reticleRenderer.GetPropertyBlock(materialPropertyBlock);

        float t = (_triggerInputValue / 0.7f) * 0.5f + Mathf.Clamp((_triggerInputValue - 0.7f) / 0.3f, 0f, 1f) * 0.3f;
        materialPropertyBlock.SetFloat("_Control", _triggerInputValue);
        _reticleRenderer.SetPropertyBlock(materialPropertyBlock);
    }
}
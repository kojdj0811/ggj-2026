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
    private GameObject _canvas;
    [SerializeField]
    private GameObject _reticle;
    private Vector2 _reticleInputValue;
    private float _triggerInputValue;
    private float _prevTriggerInputValue;

    public ShootingData ShootingData;

    void Awake()
    {
        Debug.Log("New Player Joined");
        _canvas = GameManager.Instance.Canvas;
        GameManager.Instance.Players.Add(this);
    }

    void Update()
    {
        Debug.Log("Trigger Input Value: " + _triggerInputValue);

        if (_prevTriggerInputValue > 0f && _triggerInputValue == 0f)
        {
            OnTriggerReleased(_prevTriggerInputValue);
        }
        _prevTriggerInputValue = _triggerInputValue;

        _reticle.transform.position += new Vector3(_reticleInputValue.x, _reticleInputValue.y, 0) * Time.deltaTime;

        Vector3 canvasCenter = _canvas.transform.position;
        Vector3 canvasScale = _canvas.transform.localScale;
        Vector3 reticlePos = _reticle.transform.position;

        float reticleHalfWidth = 0f;
        float reticleHalfHeight = 0f;
        var sr = _reticle.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            reticleHalfWidth = sr.bounds.size.x * 0.5f;
            reticleHalfHeight = sr.bounds.size.y * 0.5f;
        }
        else
        {
            var renderer = _reticle.GetComponent<Renderer>();
            if (renderer != null)
            {
                reticleHalfWidth = renderer.bounds.size.x * 0.5f;
                reticleHalfHeight = renderer.bounds.size.y * 0.5f;
            }
        }

        float minX = canvasCenter.x - canvasScale.x * 0.5f + reticleHalfWidth;
        float maxX = canvasCenter.x + canvasScale.x * 0.5f - reticleHalfWidth;
        float minY = canvasCenter.y - canvasScale.y * 0.5f + reticleHalfHeight;
        float maxY = canvasCenter.y + canvasScale.y * 0.5f - reticleHalfHeight;

        reticlePos.x = Mathf.Clamp(reticlePos.x, minX, maxX);
        reticlePos.y = Mathf.Clamp(reticlePos.y, minY, maxY);
        _reticle.transform.position = reticlePos;
    }

    private void OnTriggerReleased(float releasedForce)
    {
        Debug.Log($"Trigger Released! Force: {releasedForce}");
        ShootingData.Owner = this;
        ShootingData.ReticlePosition = _reticle.transform.position;
        ShootingData.TriggerValue = releasedForce;
        
        //이 부분에 실제로 발사하는 쪽에 데이터 전달 후 데이터 초기화 필요
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
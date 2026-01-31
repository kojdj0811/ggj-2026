using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _canvas;
    [SerializeField]
    private GameObject _reticle;
    private Vector2 _reticleInput;
    private float _triggerInput;

    void Update()
    {
        Debug.Log("Trigger Input Value: " + _triggerInput);

        _reticle.transform.position += new Vector3(_reticleInput.x, _reticleInput.y, 0) * Time.deltaTime;

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

    private void OnStickInput(InputValue value)
    {
        _reticleInput = value.Get<Vector2>();
    }

    private void OnDpadUp(InputValue value)
    {
        _reticleInput.y = value.isPressed ? 1 : 0;
    }

    private void OnDpadDown(InputValue value)
    {
        _reticleInput.y = value.isPressed ? -1 : 0;
    }

    private void OnDpadLeft(InputValue value)
    {
        _reticleInput.x = value.isPressed ? -1 : 0;
    }

    private void OnDpadRight(InputValue value)
    {
        _reticleInput.x = value.isPressed ? 1 : 0;
    }

    private void OnTriggerInput(InputValue value)
    {
        _triggerInput = value.Get<float>();
    }
}
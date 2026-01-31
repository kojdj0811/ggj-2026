
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;


public class EntrySelector : MonoBehaviour
{
    [SerializeField]
    private InputController _inputController;
    public EventSystem eventSystem;
    public GraphicRaycaster graphicRaycaster;

    private Toggle _currentHitToggle;
    private Button _currentHitButton;
    private int _playerId;

    private void Start()
    {
        if (eventSystem == null)
        {
            eventSystem = EventSystem.current;
        }

        if (graphicRaycaster == null)
        {
            graphicRaycaster = FindFirstObjectByType<GraphicRaycaster>();
        }

        if (_inputController == null)
        {
            _inputController = GetComponent<InputController>();
        }

        _playerId = GetComponentInParent<Player>().PlayerID;
    }

    public void RaycastAtPosition(Vector2 screenPosition)
    {
        PointerEventData eventData = new PointerEventData(eventSystem);
        eventData.position = screenPosition;

        // Raycast using the GraphicRaycaster
        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(eventData, results);

        // 첫 번째 Button/Toggle hit만 캐싱
        _currentHitToggle = null;
        _currentHitButton = null;
        foreach (var result in results)
        {
            var toggle = result.gameObject.GetComponentInParent<Toggle>();
            var button = result.gameObject.GetComponent<Button>();
            if (button != null)
            {
                if (button.gameObject.GetComponentInParent<PlayerUIPanel>()?.PlayerID != _playerId)
                {
                    continue; // 다른 플레이어의 버튼이면 무시
                }
                button.Select(); // 하이라이트만
                _currentHitButton = button;
                break;
            }
            if (toggle != null)
            {
                if (toggle.gameObject.GetComponentInParent<PlayerUIPanel>()?.PlayerID != _playerId)
                {
                    continue; // 다른 플레이어의 토글이면 무시
                }
                toggle.Select();
                _currentHitToggle = toggle;
                break;
            }
        }
    }
    // InputController에서 호출할 수 있도록 public 메서드 추가 (버튼 클릭)
    public void ClickCurrentButton()
    {
        if (_currentHitButton != null)
        {
            _currentHitButton.onClick.Invoke();
            Debug.Log($"Button Clicked: {_currentHitButton.gameObject.name}");
        }
    }
    
    // InputController에서 호출할 수 있도록 public 메서드 추가
    public void SelectCurrentToggle()
    {
        if (_currentHitToggle != null)
        {
            _currentHitToggle.isOn = true;
            Debug.Log($"Toggle Selected: {_currentHitToggle.gameObject.name}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (graphicRaycaster == null || eventSystem == null)
        {
            return;
        }

        var objectPos = transform.position;
        Camera cam = Camera.main;
        Vector3 screenPos = cam.WorldToScreenPoint(objectPos);
        RaycastAtPosition(screenPos);
    }
    
    //     // 에디터에서 Raycast 방향을 기즈모로 시각화
    // private void OnDrawGizmos()
    // {
    //     Camera cam = Camera.main;
    //     if (cam == null) return;
    //     Vector3 worldPos = transform.position;
    //     Vector3 screenPos = cam.WorldToScreenPoint(worldPos);
    //     Ray ray = cam.ScreenPointToRay(screenPos);
    //     Gizmos.color = Color.cyan;
    //     Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * 100f);
    //     Gizmos.DrawSphere(ray.origin + ray.direction * 100f, 0.2f);
    // }
}

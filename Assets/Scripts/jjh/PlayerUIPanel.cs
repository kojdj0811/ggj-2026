using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIPanel : MonoBehaviour
{
    public int PlayerID = 0;

    [SerializeField]
    private Toggle[] _colorToggles;
    [SerializeField]
    private Image _imageCharacter;


    private void Awake()
    {
        int start = PlayerID == 0 ? 0 : 7;
        int end = PlayerID == 0 ? 6 : 13;
        for (int i = start; i <= end && i < _colorToggles.Length; i++)
        {
            var toggle = _colorToggles[i];
            toggle.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    OnToggleSelected(toggle);
                }
            });
        }
    }

    private void Start()
    {
        if (_imageCharacter != null)
        {
            Vector3 startPos = _imageCharacter.rectTransform.localPosition;
            startPos.y = -1000f;
            _imageCharacter.rectTransform.localPosition = startPos;
            _imageCharacter.rectTransform.DOLocalMoveY(0f, 0.7f)
                .SetEase(Ease.OutBounce);
        }
    }

    public void DeactivateToggles(int playerId)
    {
        UIManager.Instance.PlayUISound(UIManager.Instance.ColorSelectClip);

        int start = playerId == 0 ? 0 : 7;
        int end = playerId == 0 ? 6 : 13;
        for (int i = start; i <= end && i < _colorToggles.Length; i++)
        {
            _colorToggles[i].interactable = false;
            _colorToggles[i].gameObject.SetActive(false);
        }
    }


    // Toggle이 선택될 때 호출되는 함수
    private void OnToggleSelected(Toggle selectedToggle)
    {
        int colorIndex = System.Array.IndexOf(_colorToggles, selectedToggle);
        if (colorIndex <= 6)
        {
            Debug.Log($"Player 0 selected color {colorIndex}");
            GameManager.Instance.Players[0].SelectColor(colorIndex);
            // 선택된 색상에 해당하는 캐릭터 스프라이트 업데이트
            UpdateCharacterSprite(colorIndex);
        }
        else
        {
            Debug.Log($"Player 1 selected color {colorIndex}");
            GameManager.Instance.Players[1].SelectColor(colorIndex);
            // 선택된 색상에 해당하는 캐릭터 스프라이트 업데이트
            UpdateCharacterSprite(colorIndex + 14);
        }
    }

    private void UpdateCharacterSprite(int colorIndex)
    {
        Sprite characterSprite = GameManager.Instance.ColorDataSO.GetCharacterSpriteByIndex(colorIndex);
        if (characterSprite != null)
        {
            _imageCharacter.sprite = characterSprite;
            // 짧고 귀여운 트윈 효과 (예: scale punch)
            _imageCharacter.rectTransform.DOKill(); // 기존 트윈 중복 방지
            _imageCharacter.rectTransform.DOPunchScale(new Vector3(0.2f, 0.15f, 0), 0.25f, 10, 1f);
        }
    }
}


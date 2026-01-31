
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
        }
    }
}


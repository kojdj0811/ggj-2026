
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
        foreach (var toggle in _colorToggles)
        {
            toggle.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    OnToggleSelected(toggle);
                }
            });
        }
    }

    public void DeactivateToggles()
    {
        foreach (var toggle in _colorToggles)
        {
            toggle.interactable = false;
        }
    }


    // Toggle이 선택될 때 호출되는 함수
    private void OnToggleSelected(Toggle selectedToggle)
    {
        int colorIndex = System.Array.IndexOf(_colorToggles, selectedToggle);
        if (colorIndex <= 6)
        {
            GameManager.Instance.Players[0].SelectColor(colorIndex);
        }
        else
        {
            GameManager.Instance.Players[1].SelectColor(colorIndex);
        }

        // 선택된 색상에 해당하는 캐릭터 스프라이트 업데이트
        UpdateCharacterSprite(colorIndex);
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



using UnityEngine;
using UnityEngine.UI;

public class PlayerUIPanel : MonoBehaviour
{
    public int PlayerID = 0;

    [SerializeField]
    private Toggle[] _colorToggles;


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
    }
}

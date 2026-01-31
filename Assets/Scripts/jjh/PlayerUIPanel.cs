using UnityEngine;
using UnityEngine.UI;

public class PlayerUIPanel : MonoBehaviour
{
    public int PlayerID = 0;

    [SerializeField]
    private Toggle[] _colorToggles;

    public void DeactivateToggles()
    {
        foreach (var toggle in _colorToggles)
        {
            toggle.interactable = false;
        }
    }
}

using UnityEngine;

public class ColorPaletteController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SelectColor(int colorID)
    {
        if (colorID <= 6)
        {
            GameManager.Instance.Players[0].SelectColor(colorID);
        }
        else
        {
            GameManager.Instance.Players[1].SelectColor(colorID);
        }
    }
}

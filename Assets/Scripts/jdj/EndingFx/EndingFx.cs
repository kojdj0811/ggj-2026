using MyBox;
using UnityEngine;
using UnityEngine.UI;

public class EndingFx : MonoBehaviour
{
    public Image[] fxImages;
    public float fxPositionWidth = 750;
    public Vector2 fxPositionHeightMinMax;


    [ButtonMethod]
    void ReplaceFxImages()
    {
        for (int i = 0; i < fxImages.Length; i++)
        {
            fxImages[i].rectTransform.anchoredPosition = Vector2.right * Random.Range(-fxPositionWidth, fxPositionWidth);
            fxImages[i].rectTransform.anchoredPosition += Vector2.up * Random.Range(fxPositionHeightMinMax.x, fxPositionHeightMinMax.y);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}

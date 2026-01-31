using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public static (float, float) playerScores = (0f, 0f);

    public Image timerFillImage;
    public SpriteRenderer portraitRenderer;
    public float gameDuration = 30f;
    public GameObject gameOverUI;
    public static bool TimeRemained;
    private float startTime;


    void Start()
    {
        TimeRemained = true;
        startTime = Time.timeSinceLevelLoad;
        BgmManager.Instance.PlayBGM(GameManager.Instance.BGM);
        Debug.Log("Game Start!");
    }

    void Update()
    {
        bool currentTimeRemained = (Time.timeSinceLevelLoad - startTime) < gameDuration;
        timerFillImage.fillAmount = (Time.timeSinceLevelLoad - startTime) / gameDuration;
        if(currentTimeRemained == false && TimeRemained == true)
        {
            OnGameOver();
        }
    }

    void OnGameOver()
    {
        BgmManager.Instance.StopBGM();
        TimeRemained = false;
        playerScores = TextureDiscriminator.Instance.GetPlyersPixelPercentages();
        gameOverUI.SetActive(true);
        Debug.LogWarning($"Game Over! Player 1 Score: {playerScores.Item1}, Player 2 Score: {playerScores.Item2}");
    }
}

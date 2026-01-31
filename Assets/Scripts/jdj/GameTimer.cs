using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public static (float, float) playerScores = (0f, 0f);

    public Image timerFillImage;
    public SpriteRenderer portraitRenderer;
    public float gameDuration = 30f;
    public TMP_Text gameOverUI;
    public static bool TimeRemained;
    private float startTime;
    public ItemSpawner ItemSpawner;


    void Start()
    {
        TimeRemained = true;
        startTime = Time.timeSinceLevelLoad;
        BgmManager.Instance.PlayBGM(GameManager.Instance.BGM);
        ItemSpawner.Spawn();
        Debug.Log("Game Start!");
    }

    void Update()
    {
        bool currentTimeRemained = (Time.timeSinceLevelLoad - startTime) < gameDuration;
        timerFillImage.fillAmount = (Time.timeSinceLevelLoad - startTime) / gameDuration;

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            currentTimeRemained = false;
        }

        if(currentTimeRemained == false && TimeRemained == true)
        {
            OnGameOver();
        }
    }

    void OnGameOver()
    {
        ItemSpawner.StopSpawn();
        BgmManager.Instance.StopBGM();
        BgmManager.Instance.PlayBGM(GameManager.Instance.EndingBGM);
        TimeRemained = false;
        playerScores = TextureDiscriminator.Instance.GetPlyersPixelPercentages();
        gameOverUI.gameObject.SetActive(true);
        if(playerScores.Item1 > playerScores.Item2)
        {
            ColorUtility.TryParseHtmlString(
                GameManager.Instance.ColorDataSO.GetColorCodeByIndex(GameManager.Instance.Players[0].ColorID),
                out Color color
            );
            gameOverUI.color = color;
            portraitRenderer.sprite = GameManager.Instance.ColorDataSO.GetCharacterSpriteByIndex(GameManager.Instance.Players[0].ColorID);
        }
        else
        {
            ColorUtility.TryParseHtmlString(
                GameManager.Instance.ColorDataSO.GetColorCodeByIndex(GameManager.Instance.Players[1].ColorID + 14),
                out Color color
            );
            gameOverUI.color = color;
            portraitRenderer.sprite = GameManager.Instance.ColorDataSO.GetCharacterSpriteByIndex(GameManager.Instance.Players[1].ColorID + 14);
        }

        Debug.LogWarning($"Game Over! Player 1 Score: {playerScores.Item1}, Player 2 Score: {playerScores.Item2}");
    }
}

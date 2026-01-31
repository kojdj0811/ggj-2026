using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    public PlayerUIPanel Player1Panel;
    public PlayerUIPanel Player2Panel;

    public Transform Canvas;
    public TextMeshProUGUI CountdownText;
    public GameObject VSLogo;
    [SerializeField]
    private float maxCountdownTime = 3f; // 인스펙터에서 설정 가능

    private bool _isPlayer1Ready = false;
    private bool _isPlayer2Ready = false;

    [SerializeField]
    private AudioSource _audioSource;
    [Header("UI Sound Clips")]
    public AudioClip BGM;
    public AudioClip ColorSwitchClip;
    public AudioClip ColorSelectClip;
    public AudioClip CountDownClip;

    public void PlayerReady(int playerId)
    {
        if (playerId == 0)
        {
            _isPlayer1Ready = true;
        }
        else if (playerId == 1)
        {
            _isPlayer2Ready = true;
        }

        if (_isPlayer1Ready && _isPlayer2Ready)
        {
            BgmManager.Instance.StopBGM();
            // 두 플레이어가 준비되었을 때의 로직
            PlayUISound(CountDownClip);
            CountdownAsync().Forget();
        }
    }

    // Unitask 기반 카운트다운 함수
    public async UniTask CountdownAsync()
    {
        float time = maxCountdownTime;

        while (time > 0f)
        {
            if (CountdownText != null)
                CountdownText.text = Mathf.CeilToInt(time).ToString();
            await UniTask.DelayFrame(1);
            time -= Time.deltaTime;
        }
        if (CountdownText != null)
            CountdownText.text = "GO!";

        SceneManager.LoadScene("Game");
        
    }

    public void PlayUISound(AudioClip clip)
    {
        if (_audioSource != null && clip != null)
        {
            _audioSource.PlayOneShot(clip);
        }
    }
}

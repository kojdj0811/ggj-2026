using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using Cysharp.Threading.Tasks;

public class GameManager : Singleton<GameManager>
{
    public List<Player> Players = new List<Player>();
    public ColorDataSO ColorDataSO;
    public Player PlayerPrefab;

    [SerializeField]
    private AudioSource _audioSource;
    [Header("Game Sound Clips")]
    public AudioClip BGM;
    public AudioClip EndingCutClip;
    public AudioClip EndingBGM;
    public AudioClip BombClip;
    public AudioClip ShootClip;
    public AudioClip HitClip;

    public bool IsDebugMode = false;
    public bool Player1ManualInput = false;
    public bool Player2ManualInput = false;

    public void OnJoinedPlayer(Player player)
    {
        if (UIManager.Instance.TitlePanel.activeInHierarchy)
        {
            UIManager.Instance.TitlePanel.SetActive(false);
        }

        player.transform.SetParent(transform);
        Players.Add(player);

        if (Players.Count == 1)
        {
            UIManager.Instance.Player1Panel.gameObject.SetActive(true);
        }
        else if (Players.Count == 2)
        {
            UIManager.Instance.Player2Panel.gameObject.SetActive(true);
            AllPlayerJoined();
        }
    }

    public void AllPlayerJoined()
    {
        UIManager.Instance.VSLogo.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (!Player1ManualInput)
            {
                Player player = Instantiate(PlayerPrefab);
                Player1ManualInput = true;
                IsDebugMode = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightShift))
        {
            if (!Player2ManualInput)
            {
                Player player = Instantiate(PlayerPrefab);
                Player2ManualInput = true;
                IsDebugMode = true;
            }
        }
    }

    public void PlayGameSound(AudioClip clip)
    {
        if (_audioSource == null)
        {
            _audioSource = FindFirstObjectByType<AudioSource>();
        }

        if (clip != null)
        {
            _audioSource.PlayOneShot(clip);
        }
    }

    public void RemoveAllPlayersData()
    {
        foreach (var player in Players)
        {
            Destroy(player.gameObject);
        }
        Players.Clear();
        Player1ManualInput = false;
        Player2ManualInput = false;
    }
}

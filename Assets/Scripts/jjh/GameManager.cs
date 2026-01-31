using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using Cysharp.Threading.Tasks;

public class GameManager : Singleton<GameManager>
{
    public List<Player> Players = new List<Player>();
    public ColorDataSO ColorDataSO;

    [SerializeField]
    private AudioSource _audioSource;
    [Header("Game Sound Clips")]
    public AudioClip BGM;
    public AudioClip EndingCutClip;
    public AudioClip EndingBGM;
    public AudioClip ItemGainClip;
    public AudioClip ShootClip;
    public AudioClip HitClip;

    public void OnJoinedPlayer(Player player)
    {
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
        if (Input.GetKeyDown(KeyCode.S))
        {
            UIManager.Instance.CountdownAsync().Forget();
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
}

using UnityEngine;

public class BgmManager : Singleton<BgmManager>
{
    [SerializeField]
    private AudioSource _audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayBGM(UIManager.Instance.BGM);
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip != null)
        {
            _audioSource.clip = clip;
            _audioSource.Play();
        }
    }

    public void StopBGM()
    {
        if (_audioSource != null)
        {
            _audioSource.Stop();
        }
    }
}

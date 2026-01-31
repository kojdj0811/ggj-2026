using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class CameraShaker : MonoBehaviour
{
    static public CameraShaker Instance;

    public float duration = 0.3f;
    public float magnitude = 0.2f;
    private Vector3 originalPos;

    private CancellationTokenSource cts = null;

    void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        originalPos = transform.position;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            ShakeCamera().Forget();
        }
    }

    public async UniTask ShakeCamera()
    {
        if(cts != null)
        {
            cts.Cancel();
            cts.Dispose();
        }

        cts = new CancellationTokenSource();

        float elapsed = 0.0f;
        float magnitude = this.magnitude;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.position = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;
            magnitude -= (magnitude / duration) * Time.deltaTime;

            await UniTask.Yield(cts.Token);
        }

        transform.localPosition = originalPos;
        cts = null;
    }
}

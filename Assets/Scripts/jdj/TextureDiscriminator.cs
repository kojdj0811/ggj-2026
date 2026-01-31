using UnityEngine;

public class TextureDiscriminator : MonoBehaviour
{
    public static TextureDiscriminator Instance;

    [ColorUsage(false, false)]
    public Color playerColor1;
    [ColorUsage(false, false)]
    public Color playerColor2;

    public RenderTexture canvas;

    [SerializeField]
    private ComputeShader computeShader;
    private ComputeBuffer _computeShaderBuffer_player1;
    private ComputeBuffer _computeShaderBuffer_player2;
    private int _kernelHandle;

    public void Initialize()
    {
        _kernelHandle = computeShader.FindKernel("CSMain");
        _computeShaderBuffer_player1 = new ComputeBuffer(1, sizeof(int));
        _computeShaderBuffer_player2 = new ComputeBuffer(1, sizeof(int));
        computeShader.SetTexture(_kernelHandle, "inputTexture", canvas);
        computeShader.SetBuffer(_kernelHandle, "Player1outputInt", _computeShaderBuffer_player1);
        computeShader.SetBuffer(_kernelHandle, "Player2outputInt", _computeShaderBuffer_player2);
        computeShader.SetVector("PlayerColor1", playerColor1);
        computeShader.SetVector("PlayerColor2", playerColor2);
    }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Initialize();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            // uncomment to test with different texture
            // computeShader.SetTexture(_kernelHandle, "inputTexture", TestImgae);
            // computeShader.SetVector("PlayerColor1", playerColor1);
            // computeShader.SetVector("PlayerColor2", playerColor2);

            var (player1Percentage, player2Percentage) = GetPlyersPixelPercentages();
            Debug.Log($"Player 1 Percentage: {player1Percentage}");
            Debug.Log($"Player 2 Percentage: {player2Percentage}");
        }
    }


    public (float, float) GetPlyersPixelPercentages()
    {
        int[] playerOutput1 = new int[1];
        int[] playerOutput2 = new int[1];
        _computeShaderBuffer_player1.SetData(playerOutput1);
        _computeShaderBuffer_player2.SetData(playerOutput2);
        computeShader.Dispatch(_kernelHandle, canvas.width / 8, canvas.height / 8, 1);
        _computeShaderBuffer_player1.GetData(playerOutput1);
        _computeShaderBuffer_player2.GetData(playerOutput2);

        float totalPixels = canvas.width * canvas.height;
        float player1Percentage = playerOutput1[0] / totalPixels;
        float player2Percentage = playerOutput2[0] / totalPixels;

        return (player1Percentage, player2Percentage);
    }

    void OnDestroy()
    {
        if (_computeShaderBuffer_player1 != null)
        {
            _computeShaderBuffer_player1.Release();
        }
        if (_computeShaderBuffer_player2 != null)
        {
            _computeShaderBuffer_player2.Release();
        }
    }
}

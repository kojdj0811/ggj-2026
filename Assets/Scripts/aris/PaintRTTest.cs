using UnityEngine;

public class PaintRTTest : MonoBehaviour
{
    public static PaintRTTest Instance;

    public Camera cam;
    public RenderTexture paintRT;
    public Texture2D A_brushTex; // 발바닥 텍스처
    public Texture2D B_brushTex; // 발바닥 텍스처
    private Texture2D _brushTex; // 발바닥 텍스처
    public Color A_paintColor = Color.red; // User A = red
    public Color B_paintColor = Color.blue; // User A = red
    public float brushSize = 0.2f;
    private RenderTexture targetTexture;

    Material drawMat;

    public enum PaintUser
    {
        UserA,
        UserB
    }

    public PaintUser user;

    void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
       drawMat = new Material(Shader.Find("Hidden/RTDraw"));

        // paintRT가 인스펙터에서 할당되어 있다고 가정
        if (paintRT == null)
        {
            Debug.LogError("paintRT is null");
            return;
        }

        if (!paintRT.IsCreated())
            paintRT.Create();

        // 렌더 텍스처를 흰색으로 초기화
        var prev = RenderTexture.active;
        RenderTexture.active = paintRT;
        GL.Clear(true, true, Color.white);   // (1,1,1,1)
        RenderTexture.active = prev;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector2 uv = hit.textureCoord;
                DrawAtUV(uv);
            }
        }
    }

    
    public void DrawAtUV(Vector2 uv)
    {
        //drawMat.SetTexture("_BrushTex", A_brushTex);
        //drawMat.SetColor("_Color", paintColor);
        drawMat.SetVector("_UV", uv);
        drawMat.SetFloat("_Size", brushSize);

        Color channelColor = Color.black;

        if (user == PaintUser.UserA){
            channelColor = A_paintColor;   // R 채널
            _brushTex = A_brushTex;
        }
        else{
            channelColor = B_paintColor;  // B 채널
            _brushTex = B_brushTex;
        }

        drawMat.SetColor("_Color", channelColor);
        drawMat.SetTexture("_BrushTex", _brushTex);


        RenderTexture temp = RenderTexture.GetTemporary(
            paintRT.width,
            paintRT.height,
            0,
            paintRT.format
        );

        Graphics.Blit(paintRT, temp);
        Graphics.Blit(temp, paintRT, drawMat);

        RenderTexture.ReleaseTemporary(temp);
    }
    
    /*
    void DrawAtUV(Vector2 uv)
    {
        drawMat.SetTexture("_BrushTex", brushTex);
        drawMat.SetVector("_UV", uv);
        drawMat.SetFloat("_Size", brushSize);

        RenderTexture temp = RenderTexture.GetTemporary(
            paintRT.width,
            paintRT.height,
            0,
            paintRT.format
        );

        // 1️⃣ 기존 RT를 temp로 복사
        Graphics.Blit(paintRT, temp);

        // 2️⃣ temp를 source로, paintRT를 destination으로
        Graphics.Blit(temp, paintRT, drawMat);

        RenderTexture.ReleaseTemporary(temp);
    }
    */
}

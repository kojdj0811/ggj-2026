using UnityEngine;

public class PaintRTTest : MonoBehaviour
{
    public static PaintRTTest Instance;

    public Camera cam;

    [Header("RTs")]
    public RenderTexture paintRT;
    public RenderTexture normalRT; // ✅ 추가

    [Header("User A")]
    public Texture2D[] A_brushVars = new Texture2D[3]; // ✅ 3 variants (foot+splatter mask)
    public Texture2D A_footMask;                       // ✅ foot only mask
    public Texture2D A_footNormal;                     // ✅ one normal for all variants
    public Color A_paintColor = Color.red;

    [Header("User B")]
    public Texture2D[] B_brushVars = new Texture2D[3];
    public Texture2D B_footMask;
    public Texture2D B_footNormal;
    public Color B_paintColor = Color.blue;

    [Header("Stamp Settings")]
    public float brushSize = 0.2f;
    public float paintRotationMinMax = 45f;
    [Range(0f, 1f)] public float normalThreshold = 0.5f;
    [Range(0f, 2f)] public float normalStrength = 1.0f;

    Material drawMat;        // Hidden/RTDraw
    Material drawNormalMat;  // Hidden/RTDrawNormal

    public enum PaintUser { UserA, UserB }
    public PaintUser user;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        drawMat = new Material(Shader.Find("Hidden/RTDraw"));
        drawNormalMat = new Material(Shader.Find("Hidden/RTDrawNormal"));

        if (paintRT == null)
        {
            Debug.LogError("paintRT is null");
            return;
        }

        if (!paintRT.IsCreated()) paintRT.Create();
        if (normalRT != null && !normalRT.IsCreated()) normalRT.Create();

        // paintRT white
        ClearRT(paintRT, Color.white);

        // normalRT flat normal
        if (normalRT != null)
            ClearRT(normalRT, new Color(0.5f, 0.5f, 1f, 1f));

        // shader 존재 체크(디버그용)
        if (drawNormalMat.shader == null)
            Debug.LogError("Hidden/RTDrawNormal shader not found (compile failed or file missing).");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                DrawAtUV(hit.textureCoord);
            }
        }
    }

    public void DrawAtUV(Vector2 uv)
    {
        float deg = Random.Range(-paintRotationMinMax, paintRotationMinMax);
        float rot = deg * Mathf.Deg2Rad;

        Color col;
        Texture2D stampVar;
        Texture2D footMask;
        Texture2D footNormal;

        if (user == PaintUser.UserA)
        {
            col = A_paintColor;
            stampVar = PickVar(A_brushVars);
            footMask = A_footMask;
            footNormal = A_footNormal;
        }
        else
        {
            col = B_paintColor;
            stampVar = PickVar(B_brushVars);
            footMask = B_footMask;
            footNormal = B_footNormal;
        }

        // 1) Color/Mask stamp (foot + splatter)
        if (stampVar != null)
        {
            drawMat.SetVector("_UV", uv);
            drawMat.SetFloat("_Size", brushSize);
            drawMat.SetFloat("_Rotation", rot);
            drawMat.SetColor("_Color", col);
            drawMat.SetTexture("_BrushTex", stampVar);

            BlitStamp(paintRT, drawMat);
        }

        // 2) Normal stamp (foot only)
        if (normalRT != null && footMask != null && footNormal != null)
        {
            drawNormalMat.SetVector("_UV", uv);
            drawNormalMat.SetFloat("_Size", brushSize);
            drawNormalMat.SetFloat("_Rotation", rot);
            drawNormalMat.SetFloat("_Threshold", normalThreshold);
            drawNormalMat.SetFloat("_Strength", normalStrength);
            drawNormalMat.SetTexture("_MaskTex", footMask);
            drawNormalMat.SetTexture("_NormalTex", footNormal);

            BlitStamp(normalRT, drawNormalMat);
        }
    }

    // --- helpers ---
    static Texture2D PickVar(Texture2D[] vars)
    {
        if (vars == null || vars.Length == 0) return null;

        for (int i = 0; i < 6; i++)
        {
            var t = vars[Random.Range(0, vars.Length)];
            if (t != null) return t;
        }
        return vars[0];
    }

    static void BlitStamp(RenderTexture target, Material mat)
    {
        var temp = RenderTexture.GetTemporary(target.width, target.height, 0, target.format);
        Graphics.Blit(target, temp);
        Graphics.Blit(temp, target, mat);
        RenderTexture.ReleaseTemporary(temp);
    }

    static void ClearRT(RenderTexture rt, Color c)
    {
        var prev = RenderTexture.active;
        RenderTexture.active = rt;
        GL.Clear(true, true, c);
        RenderTexture.active = prev;
    }
}

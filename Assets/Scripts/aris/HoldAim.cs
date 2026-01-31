using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class HoldAim : MonoBehaviour
{
    [Header("Hold Timings (seconds)")]
    [Tooltip("이 시간 동안 누르면 Arc 0.5->1, Color 0->1 진행")]
    public float phase1Duration = 0.8f;

    [Tooltip("phase1 이후 계속 누르면 Scale 0.15->0.444 진행되는 시간")]
    public float phase2ScaleDuration = 0.6f;

    [Header("Defaults")]
    public float baseArcProgress = 0.5f;
    public float baseColorProgress = 0.0f;
    public float baseScale = 1.2f;

    [Header("Max")]
    public float maxScale = 2.95f;

    [Header("Optional Colors (only if your shader uses these props)")]
    public bool setColors = false;
    public Color baseColor = Color.white;
    public Color targetColor = Color.red;
    public Color outlineColor = Color.black;

    // Shader property names (맞게 바꿔도 됨)
    private static readonly int ArcProgressID   = Shader.PropertyToID("_ArcProgress");
    private static readonly int ColorProgressID = Shader.PropertyToID("_ColorProgress");
    private static readonly int ScaleID         = Shader.PropertyToID("_Scale");

    // 색상 프로퍼티 이름은 네 그래프에 맞춰 수정
    private static readonly int BaseColorID     = Shader.PropertyToID("_BaseColor");
    private static readonly int TargetColorID   = Shader.PropertyToID("_TargetColor");
    private static readonly int OutlineColorID  = Shader.PropertyToID("_OutlineColor");

    private Renderer _renderer;
    private MaterialPropertyBlock _mpb;

    private float _holdTime;
    private bool _isHolding;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _mpb = new MaterialPropertyBlock();

        ApplyDefaults(); // 시작 시 기본값 적용
    }

    private void Update()
    {
        bool spaceDown = Input.GetKeyDown(KeyCode.Space);
        bool spaceHeld = Input.GetKey(KeyCode.Space);
        bool spaceUp   = Input.GetKeyUp(KeyCode.Space);

        if (spaceDown)
        {
            _isHolding = true;
            _holdTime = 0f;

            // 시작 프레임에도 기본을 보장하고 싶으면(선택)
            ApplyDefaults();
        }

        if (_isHolding && spaceHeld)
        {
            _holdTime += Time.deltaTime;
            ApplyHoldProgress(_holdTime);
        }

        if (spaceUp)
        {
            _isHolding = false;
            _holdTime = 0f;

            // 스페이스바를 떼면 즉시 기본으로 복귀
            ApplyDefaults();
        }
    }

    private void ApplyHoldProgress(float t)
    {
        float arc;
        float colorProg;
        float scale;

        // Phase 1: Arc 0.5->1, Color 0->1, Scale 유지
        if (t < phase1Duration)
        {
            float u = Safe01(t / phase1Duration);
            arc = Mathf.Lerp(baseArcProgress, 1f, u);
            colorProg = Mathf.Lerp(0f, 1f, u);
            scale = baseScale;
        }
        // Phase 2: Arc=1 고정, Color는 즉시 기본으로, Scale base->max 진행(최대 유지)
        else
        {
            arc = 1f;

            // "컬러는 기본 색상으로 바로 변경" 이라면:
            // shader가 _ColorProgress로 Lerp 한다는 전제에서 0으로 즉시 돌리면 기본색으로 즉시 복귀
            colorProg = 0f;

            float v = Safe01((t - phase1Duration) / Mathf.Max(phase2ScaleDuration, 0.0001f));
            scale = Mathf.Lerp(baseScale, maxScale, v);
        }

        _renderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat(ArcProgressID, arc);
        _mpb.SetFloat(ColorProgressID, colorProg);
        _mpb.SetFloat(ScaleID, scale);

        if (setColors)
        {
            _mpb.SetColor(BaseColorID, baseColor);
            _mpb.SetColor(TargetColorID, targetColor);
            _mpb.SetColor(OutlineColorID, outlineColor);
        }

        _renderer.SetPropertyBlock(_mpb);
    }

    private void ApplyDefaults()
    {
        _renderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat(ArcProgressID, baseArcProgress);
        _mpb.SetFloat(ColorProgressID, baseColorProgress);
        _mpb.SetFloat(ScaleID, baseScale);

        if (setColors)
        {
            _mpb.SetColor(BaseColorID, baseColor);
            _mpb.SetColor(TargetColorID, targetColor);
            _mpb.SetColor(OutlineColorID, outlineColor);
        }

        _renderer.SetPropertyBlock(_mpb);
    }

    private static float Safe01(float x) => Mathf.Clamp01(float.IsFinite(x) ? x : 0f);
}

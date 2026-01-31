Shader "Custom/Reticle_BlurStepOutline"
{
    Properties
    {
        // ===== Colors =====
        _Control        ("Control", Range(0,1)) = 0
        _BaseColor      ("Base Color", Color) = (1,1,1,1)
        _TargetColor    ("Target Color", Color) = (0,0.7,1,1)
        _TargetBlend    ("Target Blend (0~1)", Range(0,1)) = 0
        _OutlineColor   ("Outline Color", Color) = (0,0.7,1,1)

        // ===== Reticle Shape =====
        _Scale      ("Scale", Range(0.2,0.95)) = 0.5
        _Radius     ("Radius", Range(0,1)) = 0.28
        _Thickness  ("Thickness", Range(0,0.2)) = 0.012

        // Dashes
        _SegmentCount ("Segment Count", Range(1,16)) = 4
        _DashRatio    ("Dash Ratio (0~1)", Range(0.5,1)) = 0.5
        _DashOffset01 ("Dash Offset (0~1)", Range(0,1)) = 0.2

        // Arc progress (grow)
        _ArcProgress ("Arc Progress (0~1)", Range(0,1)) = 1
        _ArcOffset01 ("Arc Offset (0~1)", Range(0,1)) = 0

        // ===== Outline via Blur->Step (reticle) =====
        _OutlineWidth     ("Outline Width (UV)", Range(0,0.05)) = 0.003
        _OutlineThreshold ("Outline Threshold", Range(0,1)) = 0.01

        // ===== Center Dot =====
        _DotRadius ("Dot Radius", Range(0,0.2)) = 0.011
        _DotOutlineWidth ("Dot Outline Width (UV)", Range(0,0.05)) = 0.002
        _DotOutlineThreshold ("Dot Outline Threshold", Range(0,1)) = 0.02
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            float _Control;

            float4 _BaseColor;
            float4 _TargetColor;
            float  _TargetBlend;      // (kept as property, overridden by _Control logic)
            float4 _OutlineColor;

            float _Scale;             // (kept as property, overridden by _Control logic)
            float _Radius;
            float _Thickness;

            float _SegmentCount;
            float _DashRatio;         // (kept as property, overridden by _Control logic)
            float _DashOffset01;

            float _ArcProgress;
            float _ArcOffset01;

            float _OutlineWidth;
            float _OutlineThreshold;

            float _DotRadius;
            float _DotOutlineWidth;
            float _DotOutlineThreshold;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // --------------------------
            // Control mapping (requested)
            // --------------------------
            void EvalControl(out float dashRatioEff, out float targetBlendEff, out float scaleEff)
            {
                float c = saturate(_Control);

                // DashRatio: 0..0.5 => 0.5 -> 1, then stay 1
                float t01 = saturate(c / 0.5);
                dashRatioEff = lerp(0.5, 1.0, t01);

                // TargetBlend: 0..0.5 => 0 -> 1, then instantly 0
                if (c <= 0.5)
                    targetBlendEff = saturate(c / 0.5);
                else
                    targetBlendEff = 0.0;

                // Scale: <=0.5 => 0.2 fixed, >0.5 => 0.2 -> 0.95
                if (c <= 0.5)
                    scaleEff = 0.2;
                else
                {
                    float t = saturate((c - 0.5) / 0.5);
                    scaleEff = lerp(0.2, 0.95, t);
                }
            }

            float Angle01(float2 uv)
            {
                float2 p = uv - 0.5;
                float a = atan2(p.y, p.x);
                float a01 = (a + UNITY_PI) / (2.0 * UNITY_PI);
                return frac(a01);
            }

            float RingMask(float2 uv, float scaleOverride)
            {
                float2 p = uv - 0.5;
                float dist = length(p);
                float R = _Radius * scaleOverride;

                float aa = max(fwidth(dist), 1e-4);

                float band = abs(dist - R);
                float halfT = _Thickness * 0.5;
                float m = 1.0 - smoothstep(halfT, halfT + aa, band);
                return saturate(m);
            }

            float DashMask(float2 uv, float dashRatioOverride)
            {
                float a01 = Angle01(uv);
                a01 = frac(a01 + _DashOffset01);

                float u = a01 * _SegmentCount;
                float f = frac(u);

                // f < dashRatio => 1 else 0
                float m = 1.0 - step(dashRatioOverride, f);
                return m;
            }

            float ArcProgressMask(float2 uv)
            {
                float a01 = Angle01(uv);
                a01 = frac(a01 + _ArcOffset01);

                // keep [0..progress]
                float m = 1.0 - step(_ArcProgress, a01);
                return m;
            }

            float ReticleMask(float2 uv, float scaleOverride, float dashRatioOverride)
            {
                float ring = RingMask(uv, scaleOverride);
                float dash = DashMask(uv, dashRatioOverride);
                float arc  = ArcProgressMask(uv);
                return saturate(ring * dash * arc);
            }

            float Blur9_Reticle(float2 uv, float w, float scaleOverride, float dashRatioOverride)
            {
                float2 dx = float2(w, 0);
                float2 dy = float2(0, w);

                float s = 0;
                s += ReticleMask(uv,               scaleOverride, dashRatioOverride);
                s += ReticleMask(uv + dx,          scaleOverride, dashRatioOverride);
                s += ReticleMask(uv - dx,          scaleOverride, dashRatioOverride);
                s += ReticleMask(uv + dy,          scaleOverride, dashRatioOverride);
                s += ReticleMask(uv - dy,          scaleOverride, dashRatioOverride);
                s += ReticleMask(uv + dx + dy,     scaleOverride, dashRatioOverride);
                s += ReticleMask(uv - dx + dy,     scaleOverride, dashRatioOverride);
                s += ReticleMask(uv + dx - dy,     scaleOverride, dashRatioOverride);
                s += ReticleMask(uv - dx - dy,     scaleOverride, dashRatioOverride);

                return s * (1.0 / 9.0);
            }

            // ===== Center dot mask (always visible, always BaseColor) =====
            float DotMask(float2 uv)
            {
                float2 p = uv - 0.5;
                float dist = length(p);

                float aa = max(fwidth(dist), 1e-4);
                float m = 1.0 - smoothstep(_DotRadius, _DotRadius + aa, dist);
                return saturate(m);
            }

            float Blur9_Dot(float2 uv, float w)
            {
                float2 dx = float2(w, 0);
                float2 dy = float2(0, w);

                float s = 0;
                s += DotMask(uv);
                s += DotMask(uv + dx);
                s += DotMask(uv - dx);
                s += DotMask(uv + dy);
                s += DotMask(uv - dy);
                s += DotMask(uv + dx + dy);
                s += DotMask(uv - dx + dy);
                s += DotMask(uv + dx - dy);
                s += DotMask(uv - dx - dy);

                return s * (1.0 / 9.0);
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // Evaluate _Control-driven values
                float dashRatioEff, targetBlendEff, scaleEff;
                EvalControl(dashRatioEff, targetBlendEff, scaleEff);

                // ===== Reticle (dash ring + progress) =====
                float m = ReticleMask(uv, scaleEff, dashRatioEff);

                float blurredR = Blur9_Reticle(uv, _OutlineWidth, scaleEff, dashRatioEff);
                float dilatedR = step(_OutlineThreshold, blurredR);
                float origBinR = step(_OutlineThreshold, m);
                float outlineR = saturate(dilatedR - origBinR);

                // Inner reticle color: Base <-> Target (driven by Control mapping)
                float4 innerReticleColor = lerp(_BaseColor, _TargetColor, targetBlendEff);

                float3 col = 0;
                col += m * innerReticleColor.rgb;
                col += outlineR * _OutlineColor.rgb;

                // ===== Center Dot (always BaseColor) =====
                float dot = DotMask(uv);

                float blurredD = Blur9_Dot(uv, _DotOutlineWidth);
                float dilatedD = step(_DotOutlineThreshold, blurredD);
                float origBinD = step(_DotOutlineThreshold, dot);
                float outlineD = saturate(dilatedD - origBinD);

                col += dot * _BaseColor.rgb;
                col += outlineD * _OutlineColor.rgb;

                // alpha
                float a = 0;
                a = max(a, m * innerReticleColor.a);
                a = max(a, outlineR * _OutlineColor.a);
                a = max(a, dot * _BaseColor.a);
                a = max(a, outlineD * _OutlineColor.a);
                a = saturate(a);

                return float4(col, a);
            }

            ENDHLSL
        }
    }
}

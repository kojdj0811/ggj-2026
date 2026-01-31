Shader "Hidden/RTDraw"
{
    Properties
    {
        _MainTex ("Base", 2D) = "black" {}
        _BrushTex ("Brush", 2D) = "white" {}
        _Color ("Color", Color) = (1,0,0,1)
        _UV ("UV", Vector) = (0,0,0,0)
        _Size ("Size", Float) = 0.1
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_BrushTex);
            SAMPLER(sampler_BrushTex);

            float4 _Color;
            float2 _UV;
            float _Size;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                // 기존 RT 내용
                half4 baseCol =
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                // 브러시 UV
                float2 diff = (i.uv - _UV) / _Size + 0.5;

                // 브러시 범위 체크
                if (diff.x < 0 || diff.x > 1 || diff.y < 0 || diff.y > 1)
                    return baseCol;

                half brush = SAMPLE_TEXTURE2D(_BrushTex, sampler_BrushTex, diff).r;
                brush = step(0.5, brush);   // 0 or 1

                half mask = step(0.5, brush);

                half4 paintCol;
                paintCol.rgb = _Color.rgb;
                paintCol.a   = mask;

                // 누적
                return lerp(baseCol, paintCol, mask);
            }
            ENDHLSL
        }
    }
}

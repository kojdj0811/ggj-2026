Shader "Hidden/RTDrawNormal"
{
    Properties
    {
        _MainTex ("Base NormalRT", 2D) = "bump" {}
        _NormalTex ("Foot Normal", 2D) = "bump" {}
        _MaskTex ("Foot Mask", 2D) = "white" {}
        _UV ("UV", Vector) = (0,0,0,0)
        _Size ("Size", Float) = 0.1
        _Rotation ("Rotation", Float) = 0
        _Threshold ("Threshold", Range(0,1)) = 0.5
        _Strength ("Strength", Range(0,2)) = 1
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

            TEXTURE2D(_MainTex);   SAMPLER(sampler_MainTex);
            TEXTURE2D(_NormalTex); SAMPLER(sampler_NormalTex);
            TEXTURE2D(_MaskTex);   SAMPLER(sampler_MaskTex);

            float2 _UV;
            float _Size;
            float _Rotation;
            float _Threshold;
            float _Strength;

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings   { float4 positionHCS : SV_POSITION; float2 uv : TEXCOORD0; };

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            float3 UnpackN(float3 enc) { return normalize(enc * 2.0 - 1.0); }
            float3 PackN(float3 n)     { return n * 0.5 + 0.5; }

            half4 frag (Varyings i) : SV_Target
            {
                float3 baseEnc = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv).rgb;
                float3 baseN   = UnpackN(baseEnc);

                float2 p = (i.uv - _UV) / _Size;
                float s = sin(_Rotation);
                float c = cos(_Rotation);
                p = float2(p.x * c - p.y * s, p.x * s + p.y * c);
                float2 diff = p + 0.5;

                if (diff.x < 0 || diff.x > 1 || diff.y < 0 || diff.y > 1)
                    return half4(PackN(baseN), 1);

                half m = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, diff).r;
                half mask = step(_Threshold, m);

                float3 footEnc = SAMPLE_TEXTURE2D(_NormalTex, sampler_NormalTex, diff).rgb;
                float3 footN   = UnpackN(footEnc);

                float t = saturate(mask * _Strength);
                float3 outN = normalize(lerp(baseN, footN, t));

                return half4(PackN(outN), 1);
            }
            ENDHLSL
        }
    }
}

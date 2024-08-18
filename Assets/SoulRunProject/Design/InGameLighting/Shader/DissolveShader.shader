Shader "Custom/CustomDissolveShader"
{
    Properties
    {
        [HDR] _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        [HDR] _DamageColor ("Damage Color", Color) = (1, 1, 1, 1)
        [HDR] _EdgeColor ("Dissolve Color", Color) = (0, 0, 0, 1)
        [Toggle(_Boolean)] _Boolean ("Damage Boolean", Float) = 0.0
        _MainTex ("Texture", 2D) = "white" {}
        _DissolveTex ("Dissolve Texture", 2D) = "white" {}
        _AlphaClipThreshold ("Alpha Clip Threshold", Range(0, 1)) = 0.5
        _EdgeWidth ("Disolve Margin Width", Range(0, 1)) = 0.01
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "RenderPipeline"="UniversalPipeline"
        }
        LOD 100
        AlphaToMask On

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            //#pragma shader_feature _Boolean

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            half4 _BaseColor;
            half4 _DamageColor;
            half4 _EdgeColor;
            half _AlphaClipThreshold;
            half _EdgeWidth;
            float _Boolean;

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _DissolveTex;
            float4 _DissolveTex_ST;

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.vertex = TransformObjectToHClip(input.vertex);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 edgeCol = half4(1, 1, 1, 1);

                // noise textureからalpha値を取得
                half4 dissolve = tex2D(_DissolveTex, input.uv);
                float alpha = dissolve.r * 0.2 + dissolve.g * 0.7 + dissolve.b * 0.1;

                // dissolveを段階的な色変化によって実現する
                if (alpha < _AlphaClipThreshold + _EdgeWidth && _AlphaClipThreshold > 0)
                {
                    edgeCol = _EdgeColor;
                }
                if (alpha < _AlphaClipThreshold)
                {
                    discard;
                }

                half4 col;
                if (_Boolean)
                {
                    col = tex2D(_MainTex, input.uv) * _DamageColor * edgeCol;
                }
                else
                {
                    col = tex2D(_MainTex, input.uv) * _BaseColor * edgeCol;
                }

                return col;
            }
            ENDHLSL
        }
    }
}
Shader "Custom/CustomDissolveShader"
{
    Properties
    {
        [HDR] _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        [HDR] _HighlightColor ("Highlight Color", Color) = (1, 1, 1, 1)
        [HDR] _DamageColor ("Damage Color", Color) = (1, 1, 1, 1)
        [HDR] _EdgeColor ("Dissolve Color", Color) = (0, 0, 0, 1)
        [Toggle(_Boolean)] _Boolean ("Damage Boolean", Float) = 0.0
        _MainTex ("Texture", 2D) = "white" {}
        _Highlight("Highlight Texture", 2D) = "black" {}
        _DissolveTex ("Dissolve Texture", 2D) = "white" {}
        _AlphaClipThreshold ("Alpha Clip Threshold", Range(0, 1)) = 0.5
        _HighlightOpacity ("Highlight Opacity", Range(0, 1)) = 1.0
        _HeightScale ("Height Scale", Float) = 20.0
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
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 viewDirTS : TEXCOORD1;
                float3 lightDir : TEXCOORD2;
                float3 worldPos : TEXCOORD3;
                float3 lightColor : COLOR;
            };

            half4 _BaseColor;
            half4 _HighlightColor;
            half4 _DamageColor;
            half4 _EdgeColor;
            half _AlphaClipThreshold;
            half _EdgeWidth;
            float _Boolean;

            half _HighlightIntensity;
            half _HighlightOpacity;

            half _HeightScale;

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _Highlight;
            float4 _Highlight_ST;

            sampler2D _DissolveTex;
            float4 _DissolveTex_ST;

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.vertex = TransformObjectToHClip(input.vertex);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);

                float3 binormal = cross(normalize(input.normal), normalize(input.tangent.xyz)) * input.tangent.w;
                float3x3 rotation = float3x3(input.tangent.xyz, binormal, input.normal);

                Light light = GetMainLight();
                /*
                 * ピクセルシェーダーに受け渡される光源ベクトルや視線ベクトルを
                 * 法線マップを適用するポリゴン基準の座標系とテクスチャの座標系が合うように変換する
                 * ピクセルシェーダーで座標変換すると全ピクセルにおいて、取り出した法線ベクトルに対して座標変換するので負荷が重い
                 */
                output.lightDir = mul(rotation, light.direction);
                output.viewDirTS = mul(rotation, GetObjectSpaceNormalizeViewDir(input.vertex));
                output.lightColor = light.color;

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

                //_HighlightColor

                half4 highlight = tex2D(_Highlight, input.uv);
                highlight.a *= _HighlightOpacity;
                col += half4(highlight.rgb * _HighlightColor * highlight.a, 0.0); // highlightの色を加算

                input.lightDir = normalize(input.lightDir);
                //input.viewDirTS = normalize(input.viewDirTS);
                //float3 halfVec = normalize(input.lightDir + input.viewDirTS);

                // Diffuse

                //単純平均法で完全なグレースケール化した値を取得
                half gray = (col.r + col.g + col.b) / 3;
                half4 grayCol = half4(gray, gray, gray, col.a);

                col = half4(col.rgb * (grayCol * input.lightDir.r) * _HeightScale, col.a);

                return col;
            }
            ENDHLSL
        }
    }
}
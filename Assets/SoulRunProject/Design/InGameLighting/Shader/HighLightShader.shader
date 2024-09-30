Shader "Custom/HighLightShader"
{
    Properties
    {
        [HDR] _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        [HDR] _HighlightColor ("Highlight Color", Color) = (1, 1, 1, 1)
        _MainTexture ("Main Texture", 2D) = "white" {}
        _EmissionTexture ("Emission Texture", 2D) = "white" {}
        _HighlightOpacity ("Highlight Opacity", Range(0, 1)) = 1.0
        _HeightScale ("Height Scale", Float) = 20.0
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
            // make fog work
            #pragma multi_compile_fog

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
                float4 lightColor : COLOR;
                float3 lightDir : TEXCOORD2;
                float3 viewDirTS : TEXCOORD3;
                half fogFactor : TEXCOORD4;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half4 _HighlightColor;

                half _HighlightIntensity;
                half _HighlightOpacity;
                half _HeightScale;

                sampler2D _MainTexture;
                float4 _MainTexture_ST;

                sampler2D _EmissionTexture;
                float4 _EmissionTexture_ST;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;

                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.vertex.xyz);
                output.vertex = TransformObjectToHClip(input.vertex);
                output.uv = TRANSFORM_TEX(input.uv, _MainTexture);

                output.fogFactor = ComputeFogFactor(positionInputs.positionCS.z);

                float3 binormal = cross(normalize(input.normal), normalize(input.tangent.xyz)) * input.tangent.w;
                float3x3 rotation = float3x3(input.tangent.xyz, binormal, input.normal);

                Light light = GetMainLight();
                output.lightDir = mul(rotation, light.direction);
                output.viewDirTS = mul(rotation, GetObjectSpaceNormalizeViewDir(input.vertex));
                output.lightColor = half4(light.color, 1);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 color = tex2D(_MainTexture, input.uv) * _BaseColor;
                color.rgb = MixFog(color.rgb, input.fogFactor);

                //_HighlightColor
                half4 highlight = tex2D(_EmissionTexture, input.uv);
                highlight.a *= _HighlightOpacity;
                color += half4(highlight.rgb * _HighlightColor * highlight.a, 0.0); // highlightの色を加算

                color *= input.lightColor;
                input.lightDir = normalize(input.lightDir);
                //input.viewDirTS = normalize(input.viewDirTS);
                half4(color.rgb * input.lightDir.b * input.lightColor * _HeightScale, color.a);
                //color = half4(color.rgb * dot(input.lightDir.b, input.viewDirTS) * input.lightColor * _HeightScale, color.a);

                //Specular計算
                //float4 specular = half4(pow(max(0.0, dot(reflect(-input.lightDir, 1), input.lightDir)), _Shininess) * _SpecularColor.xyz, color.a);  // reflection

                //color += specular * input.lightColor;

                //clip(tex2D(_MainTexture, input.uv).a);

                return color;
            }
            ENDHLSL
        }
    }
}
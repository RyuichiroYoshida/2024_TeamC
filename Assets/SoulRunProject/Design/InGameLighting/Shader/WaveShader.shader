Shader "Custom/WaveShader"
{
    Properties
    {
        [MainTexture] _BaseMap("MainTexture", 2D) = "white" {}
        _SubMap("SubTexture", 2D) = "white" {}
        _Blend("Blend",Range (0, 1)) = 0
        [HDR] [MainColor] _BaseColor("BaseColor", Color) = (1, 1, 1, 1)
        [HDR] [SubColor] _SubColor("SubColor", Color) = (1, 1, 1, 1)
        _PatternScale ("PatternScale", Range(0.0, 10.0)) = 1
        _ScrollSpeed ("ScrollSpeed", Range(0.0, 100.0)) = 1
        _PerlinNoise ("PerlinNoise", Range(0.0, 300.0)) = 0.02
        _NoiseHeight ("NoiseHeight", Range(0.0, 3.0)) = 0.02
        _NoiseThreshold ("NoiseThreshold", Range(0.0, 1.0)) = 0.00
        //_F0 ("F0", Range(0.0, 1.0)) = 0.02
        //_SpecPower ("Specular Power", Range(0,100)) = 3
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent+1"
            "IgnoreProjector" = "True"
            "RenderPipeline" = "UniversalPipeline"
            "ShaderModel"="4.5"
        }
        LOD 100

        //Blend One Zero
        ZWrite On
        Cull Back
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            //Name "FowerdLit"
            //            Tags
            //            {
            //                "RenderType" = "Transparent"
            //                "Queue" = "Transparent"
            //                "RenderPipeline"="UniversalPipeline"
            //                "LightMode" = "UniversalForward"
            //            }

            HLSLPROGRAM
            //#pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            #pragma vertex vert
            #pragma fragment frag

            //#pragma alpha:fade

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_BaseMap);
            TEXTURE2D(_SubMap);
            SAMPLER(sampler_BaseMap);
            SAMPLER(sampler_SubMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _SubMap_ST;
                half4 _BaseColor;
                half4 _SubColor;
                float _Blend;
                half _PatternScale;
                half _ScrollSpeed;
                half _F0;
                half _SpecPower;
                half _PerlinNoise;
                half _NoiseHeight;
                half _NoiseThreshold;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
                float fogCoord : TEXCOORD3;
                float4 positionCS : SV_POSITION;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            half2 Random2(half2 st)
            {
                st = half2(dot(st, half2(127.1, 311.7)),
                           dot(st, half2(269.5, 183.3)));
                return -1.0 + 2.0 * frac(sin(st) * 43758.5453123);
            }

            float PerlinNoise(half2 st)
            {
                half2 p = floor(st);
                half2 f = frac(st);
                half2 u = f * f * (3.0 - 2.0 * f);

                float v00 = Random2(p + half2(0, 0));
                float v10 = Random2(p + half2(1, 0));
                float v01 = Random2(p + half2(0, 1));
                float v11 = Random2(p + half2(1, 1));

                return lerp(lerp(dot(v00, f - half2(0, 0)), dot(v10, f - half2(1, 0)), u.x),
                            lerp(dot(v01, f - half2(0, 1)), dot(v11, f - half2(1, 1)), u.x),
                            u.y) + 0.5f;
            }

            half4 BlendTexture(float2 uv)
            {
                half4 baseColor = _BaseColor;
                half4 subColor = _SubColor;

                half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);
                half4 subMap = SAMPLE_TEXTURE2D(_SubMap, sampler_SubMap, uv);

                half4 color = baseColor * baseMap * (1 - _Blend) + subColor * subMap * _Blend;

                return color;
            }

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.uv *= _PatternScale;
                output.fogCoord = ComputeFogFactor(vertexInput.positionCS.z);
                output.viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);

                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);
                output.normalWS = normalInput.normalWS;

                if (input.uv.g > _NoiseThreshold)
                {
                    float noise = PerlinNoise(input.uv);
                    output.positionCS.g += sin(noise * _PerlinNoise * input.positionOS + _Time
                        * 100) * _NoiseHeight;
                }

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                //くり抜く
                //clip(1 - input.uv.y - _NoiseThreshold);

                float4 shadowCoord = TransformWorldToShadowCoord(input.positionCS);
                half2 uv = input.uv;
                uv.g -= (_Time.x * _ScrollSpeed);
                //half4 texColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);
                half4 texColor = BlendTexture(uv);
                half3 color = texColor.rgb * _BaseColor.rgb;
                half alpha = texColor.a * _BaseColor.a;

                //                Light mainLight = GetMainLight(shadowCoord);
                //                float3 halfVector = normalize(normalize(mainLight.direction) + normalize(input.viewDirWS));
                //                float NdotH = saturate(dot(input.normalWS, halfVector));
                //                float spec = pow(NdotH, _SpecPower);
                //
                //                color += mainLight.color.rgb * spec * mainLight.distanceAttenuation;
                //
                //                half frenel = _F0 + (1.0 - _F0) * pow(1 - dot(normalize(input.viewDirWS), input.normalWS), 5);
                //                color += frenel;
                //                alpha += frenel;

                color = MixFog(color, input.fogCoord);

                return half4(color, alpha);
            }
            ENDHLSL
        }
    }
    FallBack "Transparent/Diffuse"
}
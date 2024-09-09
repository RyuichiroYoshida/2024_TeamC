Shader "Custom/CustomLitShader"
{
    Properties
    {
        _BaseMap ("Base Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (255, 255, 255, 1)
        _ShadowColor ("Shadow Color", Color) = (0, 0, 0, 1)
        _ShadowStrength ("Shadow Strength", Range(0, 1)) = 1
        _DitherLevel("Dither Level", Range(0, 16)) = 0

        [Toggle(_ALPHATEST_ON)] _EnableAlphaTest("Enable Alpha Cutoff", Float) = 0.0
        _Cutoff ("Alpha Cutoff", Float) = 0.5

        [Toggle(_NORMALMAP)] _EnableBumpMap("Enable Normal/Bump Map", Float) = 0.0
        [NoScaleOffset] _BumpMap ("Normal/Bump Texture", 2D) = "bump" {}
        _BumpScale ("Bump Scale", Float) = 1

        [Toggle(_METALLIC)] _EnableMetallic("Enable Metallic", Float) = 0.0
        [NoScaleOffset] _MetallicGlossMap ("Metallic Gloss Map", 2D) = "black" {}
        _Metallic ("Metallic", Range(0, 1)) = 0.5

        [KeywordEnum(None, Smoothness, Roughness)] _SmoothnessType("Smoothness Type", Int) = 0
        [NoScaleOffset] _SmoothnessRoughnessMap ("Smoothness/Roughness Map", 2D) = "white" {}
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5

        [Toggle(_EMISSION)] _EnableEmission("Enable Emission", Float) = 0.0
        [NoScaleOffset] _EmissionMap ("Emission Texture", 2D) = "white" {}
        [HDR] _EmissionColor ("Emission Color", Color) = (0, 0, 0, 0)

        [Toggle(_OUTLINE)] _EnableOutLine("Enable OutLine", Float) = 0.0
        [HDR] _OutLineColor ("OutLine Color", Color) = (0, 0, 0, 1)
        _OutlineWidth ("Outline Width", Range(0, 100)) = 0
    }
    SubShader
    {
        //Cull Front

        Tags
        {
            "RenderType"="Opaque"
            "Queue" = "Geometry-1"
            "RenderPipeline"="UniversalPipeline"
            "UniversalMaterialType" = "Lit"
            "IgnoreProjector" = "True"
        }

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        CBUFFER_START(UnityPerMaterial)
            float4 _BaseMap_ST;
            float4 _BaseColor;
            float4 _ShadowColor;
            float _ShadowStrength;
            float _BumpScale;
            float4 _EmissionColor;
            float _Metallic;
            float _Smoothness;
            float _Cutoff;
            half _DitherLevel;

            sampler2D _MetallicGlossMap;
            sampler2D _SmoothnessRoughnessMap;
        CBUFFER_END
        ENDHLSL

        Pass
        {
            Name "Main"
            Tags
            {
                "LightMode"="UniversalForward"
            }

            HLSLPROGRAM
            // 標準 SRP ライブラリを使用して gles 2.0 をコンパイルするために必要です
            // すべてのシェーダーは HLSLcc でコンパイルする必要があり、現在デフォルトで HLSLcc を使用していないのは gles だけです
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x gles

            //#pragma target 4.5 // https://docs.unity3d.com/Manual/SL-ShaderCompileTargets.html

            #pragma vertex vert
            #pragma fragment frag

            // Materialのキーワード
            #pragma shader_feature _NORMALMAP
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _METALLIC
            //#pragma shader_feature _SMOOTHNESS
            //#pragma shader_feature _ROUGHNESS
            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICSPECGLOSSMAP
            //#pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            //#pragma shader_feature _OCCLUSIONMAP
            //#pragma shader_feature _ _CLEARCOAT _CLEARCOATMAP // URP v10+

            //#pragma shader_feature _SPECULARHIGHLIGHTS_OFF
            //#pragma shader_feature _ENVIRONMENTREFLECTIONS_OFF
            //#pragma shader_feature _SPECULAR_SETUP
            #pragma shader_feature _RECEIVE_SHADOWS_OFF

            #pragma multi_compile _SMOOTHNESSTYPE_NONE _SMOOTHNESSTYPE_SMOOTHNESS _SMOOTHNESSTYPE_ROUGHNESS

            // URPのキーワード
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

            // Unityで定義されたキーワード
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3

            // Includes
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            //#include "Packages/com.unity.render-pipelines.universal/Shaders/LitForwardPass.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
            //#include "OutLine.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float2 lightmapUV : TEXCOORD1;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 1);

                #ifdef REQUIRES_WORLD_SPACE_POS_INTERPOLATOR
                float3 positionWS : TEXCOORD2;
                #endif

                float3 normalWS : TEXCOORD3;
                #ifdef _NORMALMAP
					float4 tangentWS 			: TEXCOORD4;
                #endif

                float3 viewDirWS : TEXCOORD5;
                half4 fogFactorAndVertexLight : TEXCOORD6; // x: fogFactor, yzw: vertex light

                #ifdef REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
					float4 shadowCoord			: TEXCOORD7;
                #endif

                float4 positionSS : TEXCOORD8;
            };

            // struct CustomFragOutput
            // {
            //     // half4 GBuffer0 : SV_Target0;
            //     // half4 GBuffer1 : SV_Target1;
            //     // half4 GBuffer2 : SV_Target2;
            //     // half4 GBuffer3 : SV_Target3;
            //     #ifdef GBUFFER_OPTIONAL_SLOT_1
            //     GBUFFER_OPTIONAL_SLOT_1_TYPE GBuffer4 : SV_Target4;
            //     #endif
            //     #ifdef GBUFFER_OPTIONAL_SLOT_2
            //     half4 GBuffer5 : SV_Target5;
            //     #endif
            //     #ifdef GBUFFER_OPTIONAL_SLOT_3
            //      half4 GBuffer6 : SV_Target6;
            //     #endif
            //     float depth : SV_Depth;
            // };

            // struct MyDecalSurfaceData
            // {
            //     float3 baseColor;
            //     float3 worldPos;
            //     float3 worldNormal;
            //     float3 viewDir;
            //     float3 bentNormal;
            //     float4 lightmapUV;
            //     float4 shadowCoord;
            //     float4 fogFactorAndVertexLight;
            //     float3 specColor;
            //     half occlusion;
            //     float perceptualRoughness;
            //     float smoothness;
            //     half metallic;
            // };

            // SurfaceInput.hlslで自動的に定義される。
            //TEXTURE2D(_BaseMap);
            //SAMPLER(sampler_BaseMap);

            #if SHADER_LIBRARY_VERSION_MAJOR < 9
			// この関数は URP v9.xx バージョンで追加されました。以前のバージョンの URP をサポートしたい場合は、代わりにそれを処理する必要があります。
			// ワールド空間のビューの方向 (ビューアの方向を指す) を計算します。
			float3 GetWorldSpaceViewDir(float3 positionWS) {
				if (unity_OrthoParams.w == 0) {
					// 視点
					return _WorldSpaceCameraPos - positionWS;
				} else {
					// 正投影法
					float4x4 viewMat = GetWorldToViewMatrix();
					return viewMat[2].xyz;
				}
			}
            #endif

            Varyings vert(Attributes input)
            {
                Varyings output;

                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = positionInputs.positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.color = input.color;

                #ifdef REQUIRES_WORLD_SPACE_POS_INTERPOLATOR
                output.positionWS = positionInputs.positionWS;
                #endif

                output.positionSS = ComputeScreenPos(output.positionCS);

                output.viewDirWS = GetWorldSpaceViewDir(positionInputs.positionWS);

                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                output.normalWS = normalInputs.normalWS;
                #ifdef _NORMALMAP
					real sign = input.tangentOS.w * GetOddNegativeScale();
					output.tangentWS = half4(normalInputs.tangentWS.xyz, sign);
                #endif

                half3 vertexLight = VertexLighting(positionInputs.positionWS, normalInputs.normalWS);
                half fogFactor = ComputeFogFactor(positionInputs.positionCS.z);

                output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

                OUTPUT_LIGHTMAP_UV(IN.lightmapUV, unity_LightmapST, OUT.lightmapUV);
                OUTPUT_SH(output.normalWS.xyz, output.vertexSH);

                #ifdef REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
					output.shadowCoord = GetShadowCoord(positionInputs);
                #endif

                return output;
            }

            InputData InitializeInputData(Varyings input, half3 normalTS)
            {
                InputData inputData = (InputData)0;

                #if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
                inputData.positionWS = input.positionWS;
                #endif

                half3 viewDirWS = SafeNormalize(input.viewDirWS);
                #ifdef _NORMALMAP
					float sgn = input.tangentWS.w; // +1 または -1 のいずれかでなければなりません。
					float3 bitangent = sgn * cross(input.normalWS.xyz, input.tangentWS.xyz);
					inputData.normalWS = TransformTangentToWorld(normalTS, half3x3(input.tangentWS.xyz, bitangent.xyz, input.normalWS.xyz));
                #else
                inputData.normalWS = input.normalWS;
                #endif

                inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
                inputData.viewDirectionWS = viewDirWS;

                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					inputData.shadowCoord = input.shadowCoord;
                #elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
                #else
                inputData.shadowCoord = float4(0, 0, 0, 0);
                #endif

                inputData.fogCoord = input.fogFactorAndVertexLight.x;
                inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
                inputData.bakedGI = SAMPLE_GI(IN.lightmapUV, input.vertexSH, inputData.normalWS);
                return inputData;
            }

            SurfaceData InitializeSurfaceData(Varyings input)
            {
                SurfaceData surfaceData = (SurfaceData)0;
                // 注意: SurfaceData surfaceData を使用するだけです。ここでは設定しません。
                // ただし、戻る前に構造体のすべての値が設定されていることを確認する必要があります。
                // SurfaceData に 0 をキャストすることで、すべての内容が自動的に 0 に設定されます。

                half4 albedoAlpha = SampleAlbedoAlpha(input.uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap));
                surfaceData.alpha = Alpha(albedoAlpha.a, _BaseColor, _Cutoff);
                surfaceData.albedo = albedoAlpha.rgb * _BaseColor.rgb * input.color.rgb;

                // 簡単にするために、メタリック/スペキュラー マップまたはオクルージョン マップはサポートしていません。
                // その例については、https://github.com/Unity-Technologies/Graphics/blob/master/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl を参照してください。

                half4 glossmap = 0;

                #ifdef _METALLIC
			    glossmap = 1 - tex2D(_MetallicGlossMap, input.uv);
                surfaceData.metallic =  glossmap.r * _Metallic;
                #else
                surfaceData.metallic = glossmap;
                #endif

                half4 smoothness = 0;

                #ifdef _SMOOTHNESSTYPE_NONE
                smoothness = 0.5;
                #elif _SMOOTHNESSTYPE_SMOOTHNESS
                smoothness = _Smoothness * tex2D(_SmoothnessRoughnessMap, input.uv);
                #elif _SMOOTHNESSTYPE_ROUGHNESS
			    smoothness = _Smoothness * (1 - tex2D(_SmoothnessRoughnessMap, input.uv));
                #endif

                surfaceData.smoothness = smoothness;

                surfaceData.normalTS = SampleNormal(input.uv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap), _BumpScale);
                surfaceData.emission = SampleEmission(input.uv, _EmissionColor.rgb,
                                                      TEXTURE2D_ARGS(
                                                          _EmissionMap, sampler_EmissionMap));

                surfaceData.occlusion = 1;

                return surfaceData;
            }

            Light MyGetMainLight(float4 shadowCoord)
            {
                Light light = GetMainLight();

                /// RealTimeShadowの計算 ///
                half4 shadowParams = GetMainLightShadowParams();
                half shadowStrength = shadowParams.x * _ShadowStrength;
                ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData();

                half attenuation;
                attenuation = SAMPLE_TEXTURE2D_SHADOW(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture,
                                                      shadowCoord.xyz);
                attenuation = SampleShadowmapFiltered(
                    TEXTURE2D_SHADOW_ARGS(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture), shadowCoord,
                    shadowSamplingData);;
                attenuation = LerpWhiteTo(attenuation, shadowStrength);

                half shadowAttenuation = BEYOND_SHADOW_FAR(shadowCoord) ? 1.0 : attenuation;
                ///

                light.shadowAttenuation = shadowAttenuation;

                return light;
            }

            // DecalSurfaceData PopulateDecalSurfaceData(Varyings input)
            // {
            //     DecalSurfaceData decalData;
            //     decalData.baseColor = input.color;
            //     decalData.normalWS = half4(normalize(input.normalWS), 1.0);
            //     decalData.emissive = half3(0.0, 0.0, 0.0); // 必要に応じて変更
            //     decalData.metallic = _Metallic;
            //     decalData.occlusion = 1.0; // 適切な値に設定
            //     decalData.smoothness = _Smoothness;
            //     decalData.MAOSAlpha = 1.0; // 必要に応じて変更
            //     return decalData;
            // }

            // 閾値マップ
            static const float4x4 pattern =
            {
                0, 8, 2, 10,
                12, 4, 14, 6,
                3, 11, 1, 9,
                15, 7, 13, 565
            };
            static const int PATTERN_ROW_SIZE = 4;

            half4 frag(Varyings input) : SV_Target
            {
                SurfaceData surfaceData = InitializeSurfaceData(input);
                InputData inputData = InitializeInputData(input, surfaceData.normalTS);
                Light mainLight = MyGetMainLight(inputData.shadowCoord);

                // デカールを適用
                #ifdef _DBUFFER
                ApplyDecalToSurfaceData(input.positionCS, surfaceData, inputData);
                #endif

                // URP v10+ バージョンでは、これを使用できます。
                // half4 color = UniversalFragmentPBR(inputData, surfaceData);

                // ただし、他のバージョンでは、代わりにこれを使用する必要があります。
                // SurfaceData 構造体の使用を完全に避けることもできますが、整理するのに役立ちます。
                half4 color = UniversalFragmentPBR(inputData, surfaceData.albedo, surfaceData.metallic,
                                                   surfaceData.specular,
                                                   surfaceData.smoothness,
                                                   surfaceData.occlusion,
                                                   surfaceData.emission, surfaceData.alpha);

                color.rgb = lerp(_ShadowColor.rgb, color, mainLight.shadowAttenuation);
                color.rgb = MixFog(color.rgb, inputData.fogCoord);

                // color.a = OutputAlpha(color.a);
                // これが本当に重要かどうかはわかりません。それは次のように実装されます。
                // saturate(outputAlpha + _DrawObjectPassData.a);
                // ここで、_DrawObjectPassData.a は、不透明オブジェクトの場合は 1、アルファ ブレンドの場合は 0 です。
                // しかし、これは URP v8 で追加されたもので、それより前のバージョンにはありませんでした。
                // ただし、アルファが 0 ～ 1 の範囲を超えないようにするために、アルファを飽和させることもできます。
                color.a = saturate(color.a);

                // スクリーン座標
                float2 screenPos = input.positionSS.xy / input.positionSS.w;
                // 画面サイズを乗算して、ピクセル単位に
                float2 screenPosInPixel = screenPos.xy * _ScreenParams.xy;

                // ディザリングテクスチャ用のUVを作成
                int ditherUV_x = (int)fmod(screenPosInPixel.x, PATTERN_ROW_SIZE);
                int ditherUV_y = (int)fmod(screenPosInPixel.y, PATTERN_ROW_SIZE);
                float dither = pattern[ditherUV_x, ditherUV_y];

                // 閾値が0以下なら描画しない
                clip(dither - _DitherLevel);

                return color; // float4(inputData.bakedGI,1);
            }
            ENDHLSL
        }

        // UsePass "Universal Render Pipeline/Lit/ShadowCaster"
        // これを行うことはできますが、CBUFFER が同じではないため、現在 SRP Batcher でのバッチ処理が中断されることに注意してください。
        // したがって、代わりにパスを手動で定義します。
        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode"="ShadowCaster"
            }

            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            // 標準 srp ライブラリを使用して gles 2.0 をコンパイルするために必要です
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x gles
            //#pragma target 4.5

            // Materialのキーワード
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            // GPUのインスタンス化
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"

            // 頂点の移動を行いたい場合は、頂点関数を変更する必要があることに注意してください。
            /*
            // 例: 
            #pragma vertex vert
 
            Varyings vert(Attributes input) {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
 
                // 変位の例
                input.positionOS += float4(0, _SinTime.y, 0, 0);
 
                output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
                output.positionCS = GetShadowPositionHClip(input);
                return output;
            }*/

            // ShadowCasterPass を使用するということは、_BaseMap、_BaseColor、_Cutoff シェーダ プロパティも必要であることを意味します。
            // テクスチャである _BaseMap を除いて、それらも cbuffer に含まれます。
            ENDHLSL
        }

        // 同様に、DepthOnly パスが必要です。
        // UsePass "Universal Render Pipeline/Lit/DepthOnly"
        // 繰り返しますが、cbuffer が異なるため、SRP Batcher によるバッチ処理が中断されます。

        // DepthOnly パスは ShadowCaster に非常に似ていますが、シャドウ バイアス オフセットは含まれません。
        // Unity はシーンビューでオブジェクトの深度をレンダリングするときにこのパスを使用すると思います。
        // ただし、ゲームビュー/実際のカメラの深度テクスチャの場合は、それなしでも問題なくレンダリングされます。
        // ただし、Forward Renderer 機能で使用できる可能性があるため、おそらく引き続き含める必要があります。
        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode"="DepthOnly"
            }

            ZWrite On
            ColorMask 0

            HLSLPROGRAM
            // 標準 srp ライブラリを使用して gles 2.0 をコンパイルするために必要です
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x gles
            //#pragma target 4.5

            // Materialのキーワード
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            // GPUのインスタンス化
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            //#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            // URP が提供する Lit シェーダはこれを使用しますが、すでにある cbuffer も処理することに注意してください。
            // cbuffer を使用するようにシェーダーを変更することもできますが、単にこれを行うこともできます。
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"

            // 繰り返しますが、DepthOnlyPass を使用するということは、_BaseMap、_BaseColor、_Cutoff シェーダープロパティも必要であることを意味します。
            // テクスチャである _BaseMap を除いて、それらも cbuffer に含まれます。
            ENDHLSL
        }

        // URP には、ライトマップをベイクするときに使用される「メタ」パスもあります。
        // UsePass "Universal Render Pipeline/Lit/Meta"
        // これはまだ SRP Batcher を壊しますが、それが重要なのかどうか興味があります。
        // メタ パスはライトマップのベイク処理にのみ使用されるため、エディターでのみ使用されるのでしょうか?
        // とにかく、独自のメタパスを作成したい場合は、URP がサンプルとして提供するシェーダを見てください。

        Pass
        {
            Name "DepthNormals"
            Tags
            {
                "LightMode" = "DepthNormals"
            }

            ZWrite On
            //Cull [_Cull]

            HLSLPROGRAM
            #pragma shader_feature _ALPHATEST_ON
            #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
            #pragma multi_compile_instancing

            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma vertex Vert
            //#pragma fragment Frag

            int _Loop;
            float _MinDistance;
            float4 _Color;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 positionSS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            struct FragOutput
            {
                float4 normal : SV_Target;
                float depth : SV_Depth;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;

                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);

                output.positionSS.z = -TransformWorldToView(output.positionWS).z;

                return output;
            }

            // FragOutput Frag(Varyings input)
            // {
            //     UNITY_SETUP_INSTANCE_ID(input);
            //     UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
            // }
            ENDHLSL
        }

        Pass
        {
            Name "OutLine"
            Cull Front
            ZWrite On

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma shader_feature _OUTLINE

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            float _OutlineWidth;
            half4 _OutLineColor;

            struct Attributes
            {
                float4 positionOS: POSITION;
                float4 normalOS: NORMAL;
                float4 tangentOS: TANGENT;
            };

            struct Varyings
            {
                float4 positionCS: SV_POSITION;
                float4 positionSS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
            };

            // 閾値マップ
            static const float4x4 pattern =
            {
                0, 8, 2, 10,
                12, 4, 14, 6,
                3, 11, 1, 9,
                15, 7, 13, 565
            };
            static const int PATTERN_ROW_SIZE = 4;

            Varyings vert(Attributes v)
            {
                Varyings OUT;
                //#ifdef _OUTLINE

                VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(v.normalOS, v.tangentOS);

                float3 normalWS = vertexNormalInput.normalWS;
                float3 normalCS = TransformWorldToHClipDir(normalWS);

                VertexPositionInputs positionInputs = GetVertexPositionInputs(v.positionOS.xyz);
                OUT.positionCS = positionInputs.positionCS + float4(normalCS.xy * 0.001 * _OutlineWidth, 0, 0);

                OUT.positionSS = ComputeScreenPos(OUT.positionCS);

                //#endif
                return OUT;
            }

            half4 frag(Varyings IN): SV_Target
            {
                float4 col = _OutLineColor;

                // スクリーン座標
                float2 screenPos = IN.positionSS.xy / IN.positionSS.w;
                // 画面サイズを乗算して、ピクセル単位に
                float2 screenPosInPixel = screenPos.xy * _ScreenParams.xy;

                // ディザリングテクスチャ用のUVを作成
                int ditherUV_x = (int)fmod(screenPosInPixel.x, PATTERN_ROW_SIZE);
                int ditherUV_y = (int)fmod(screenPosInPixel.y, PATTERN_ROW_SIZE);
                float dither = pattern[ditherUV_x, ditherUV_y];

                // 閾値が0以下なら描画しない
                #ifdef _OUTLINE
                clip(dither - _DitherLevel);
                return col;
                #else
                clip(dither - 16);
                return 0;
                #endif
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "CustomLitShaderGUI"
}